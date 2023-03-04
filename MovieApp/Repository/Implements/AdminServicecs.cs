using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GOSBackend.Data;
using GOSBackend.Repository.Interface;
using GOSBackend.SqlTables;
using GOSLibraries.GOS_API_Response;
using Microsoft.Extensions.Configuration;
using static GOSBackend.Contracts.Admin_Objs.HelpObjs;
using static GOSBackend.Contracts.Common.AuxillaryObjs;

namespace GOSBackend.Repository.Implements
{
    public class AdminServicecs : IAdminServices
    {
        private readonly DataBaseContext _dbContext;
        private Cloudinary _cloudinary;
        public AdminServicecs(DataBaseContext dataBaseContext, IConfiguration configuration)
        {
            _dbContext = dataBaseContext;
            var cloudinaryAcc = new CloudinarySettings();
            configuration.GetSection(nameof(CloudinarySettings)).Bind(cloudinaryAcc);

            Account acc = new Account(
                cloudinaryAcc.CloudName,
                cloudinaryAcc.ApiKey,
                cloudinaryAcc.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<HelpAddResObj> AddUpdateHelpContent(HelpAddModel model)
        {
            var response = new HelpAddResObj() { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { } } };
            try
            {
               if(model == null)
                {
                    response.Status.Message.FriendlyMessage = "Invalid request";
                    return response;
                }
               foreach(var item in model.Tasks)
                {
                    if (string.IsNullOrEmpty(item.ImageBase64) == false)
                    {
                        if (string.IsNullOrEmpty(item.PublicId) == false)
                        {
                            var deleteResource = await _cloudinary.DeleteResourcesAsync(CloudinaryDotNet.Actions.ResourceType.Image, item.PublicId);
                        }
                        var fileByte = Convert.FromBase64String(item.ImageBase64);
                        Stream stream = new MemoryStream(fileByte);
                        var fileName = $"{model.Application.ToString()}-{model.Module}-{model.OperationType.ToString()}-Task{Guid.NewGuid().ToString()}";
                        var fileNameExt = $"{fileName}.{item.ImageExtension}";
                        var uploadResult = new ImageUploadResult();
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(fileNameExt, stream),
                            Transformation = new Transformation().Width(1000).Height(500).Crop("fill").Gravity("face"),
                            PublicId = $"GOS Software/Help-Image/{fileName}",
                            Overwrite = true,
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        if(uploadResult.Error == null)
                        {
                            item.ImageUrl = uploadResult.SecureUrl.ToString();
                            item.PublicId = uploadResult.PublicId;
                            item.HasImage = true;
                        }
                    }
                }
               if(model.HelpId > 0)
                {
                    var help = _dbContext.HelpContents.Find(model.HelpId);
                    if(help != null)
                    {
                        help.Application = (int)model.Application;
                        help.Module = model.Module;
                        help.ProcessTitle = model.ProcessTitle;
                        help.ProcessDescription = model.ProcessDescription;
                        foreach(var item in model.Tasks)
                        {
                            var chKTask = _dbContext.HelpContentTasks.FirstOrDefault(x => x.HelpContentsId == model.HelpId && x.Description.Trim().ToLower() == item.Instructition.Trim().ToLower());
                            if(chKTask != null)
                            {
                                chKTask.ImageUrl = item.ImageUrl;
                                chKTask.PublicId = item.PublicId;
                            }
                            else
                            {
                                chKTask = new HelpContentTasks
                                {
                                    HasImage = item.HasImage,
                                    ImageUrl = item.ImageUrl,
                                    Description = item.Instructition,
                                    PublicId = item.PublicId,
                                    HelpContentsId = model.HelpId,
                                };
                                _dbContext.HelpContentTasks.Add(chKTask);
                            }
                        }
                    }
                }
                else
                {
                    var help = new HelpContents
                    {
                        Application = (int)model.Application,
                        Module = model.Module,
                        Type = (int)model.OperationType,
                        ProcessTitle = model.ProcessTitle,
                        ProcessDescription = model.ProcessDescription,
                        HelpContentTasks = model.Tasks.Select(x => new HelpContentTasks
                        {
                            HasImage = x.HasImage,
                            ImageUrl = x.ImageUrl,
                            Description = x.Instructition,
                            PublicId = x.PublicId,
                        }).ToList()
                    };
                    _dbContext.HelpContents.Add(help);
                }
               var res = await _dbContext.SaveChangesAsync() >0;
                response.Status.IsSuccessful = res;
                response.Status.Message.FriendlyMessage = res ? "Sucessful" : "Unsuccessful";
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public HelpAddResObjs GetAllContents()
        {
            var result = (from a in _dbContext.HelpContents
                          select new HelpAddModel
                          {
                              Application = (Application)a.Application,
                              OperationType = (HelpOperationType)a.Type,
                              HelpId = a.HelpContentsId,
                              Module = a.Module,
                              ProcessTitle = a.ProcessTitle,
                              ProcessDescription = a.ProcessDescription,
                          }).ToList();
            var response = new HelpAddResObjs
            {
                Helps = result,
                Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "successful" } }
            };
            return response;
        }
        public HelpAddResObj GetContent(int HelpId)
        {
            var result = (from a in _dbContext.HelpContents
                          where a.HelpContentsId == HelpId
                          select new HelpAddModel
                          {
                              Application = (Application)a.Application,
                              OperationType = (HelpOperationType)a.Type,
                              HelpId = a.HelpContentsId,
                              Module = a.Module,
                              ProcessTitle = a.ProcessTitle,
                              ProcessDescription = a.ProcessDescription,
                              Tasks = _dbContext.HelpContentTasks.Where(x=> x.HelpContentsId == a.HelpContentsId).Select(x=> new HelpTaskObj
                              {
                                  HasImage = x.HasImage,
                                  ImageUrl = x.ImageUrl,
                                  Instructition = x.Description,
                                  PublicId = x.PublicId,
                              }).ToList(),
                          }).FirstOrDefault();
            var response = new HelpAddResObj
            {
                Help = result ?? new HelpAddModel(),
                Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "successful" } }
            };
            return response;
        }

        public async Task<DeleteResObj> DeleteHelpContents(DeleteIds model)
        {
            try
            {
                var response = new DeleteResObj { IsDeleted = false, Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { } } };
                var helpToDelete = _dbContext.HelpContents.Where(x => model.Ids.Contains(x.HelpContentsId)).ToList();
                var helpTaskToDelete = _dbContext.HelpContentTasks.Where(x => model.Ids.Contains(x.HelpContentsId)).ToList();
                _dbContext.HelpContents.RemoveRange(helpToDelete);
                if (helpTaskToDelete.Any())
                {
                    _dbContext.HelpContentTasks.RemoveRange(helpTaskToDelete);
                }
                var res = await _dbContext.SaveChangesAsync() > 0;
                response.IsDeleted = res;
                response.Status.IsSuccessful = res;
                response.Status.Message.FriendlyMessage = res ? "Deleted Sucessfully" : "Unsucessful";
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
