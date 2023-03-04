using GOSBackend.SqlTables;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Identity;
using static GOSBackend.Contracts.Common.AuxillaryObjs;
using static GOSBackend.Contracts.User_Identity_Obj.AdminIdentityObjs;

namespace GOSBackend.IdentityServices
{
    public class ProfileService  : IProfileService
    {
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<ProfileService> _logger;
        private readonly IHttpContextAccessor _cotextAccessor;
        public ProfileService(UserManager<Users> userManager, ILogger<ProfileService> logger, IHttpContextAccessor cotextAccessor)
        {
            _userManager = userManager;
            _logger = logger;
            _cotextAccessor = cotextAccessor;
        }
        public async Task<UserAdminProfileResObj> GetAdminProfile()
        {
            var response = new UserAdminProfileResObj
            {
               
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = ""
                    }
                }
            };
            
            try
            {
                string userId = _cotextAccessor?.HttpContext?.User?.FindFirst("userId")?.Value ?? string.Empty;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.Status.Message.FriendlyMessage = "User not found";
                    return response;
                }
                var profile = new UserAdminProfileObj
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Gender = ((Gender)user.Gender).ToString()
                };
                response.Profile = profile;
                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = "Sucessful";
                return response;
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.Message.FriendlyMessage = ex?.Message ?? ex?.InnerException?.Message;
                response.Status.Message.TechnicalMessage = ex?.ToString();
                return response;
            }
        }
    }
}
