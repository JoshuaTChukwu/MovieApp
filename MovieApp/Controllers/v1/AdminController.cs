using GOSBackend.Contracts.Common;
using GOSBackend.Handlers;
using GOSBackend.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GOSBackend.Contracts.Admin_Objs.HelpObjs;
using static GOSBackend.Contracts.Common.AuxillaryObjs;
using static GOSBackend.Contracts.User_Identity_Obj.AdminIdentityObjs;

namespace GOSBackend.Controllers.v1
{
    [AuthorizationMethod]
    public class AdminController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminServices _service;
        public AdminController(IAdminServices adminServices, IHttpContextAccessor httpContextAccessor, ILogger<AdminController> logger)
        {
          _contextAccessor = httpContextAccessor;
            _logger = logger;
            _service = adminServices;
        }
        [HttpPost(APIRoutes.Admin.ADD_HELP)]
        public async Task<ActionResult<HelpAddResObj>> AddUpdateHelpContent([FromBody] HelpAddModel model)
        {
            try
            {
                var response = await _service.AddUpdateHelpContent(model);
                if (response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new HelpAddResObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }
        [HttpPost(APIRoutes.Admin.DELETE_HELP)]
        public async Task<ActionResult<DeleteResObj>> DeleteHelpContent([FromBody] DeleteIds model)
        {
            try
            {
                var response = await _service.DeleteHelpContents(model);
                if (response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new DeleteResObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }
        [HttpGet(APIRoutes.Admin.GET_ALL_HELP)]
        public ActionResult<HelpAddResObjs> GetHelpContent()
        {
            try
            {
                var response =  _service.GetAllContents();
                
                return Ok(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new HelpAddResObjs
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }

        [HttpGet(APIRoutes.Admin.GET_HELP)]
        public ActionResult<HelpAddResObj> GetHelpContentById([FromQuery] int HelpId)
        {
            try
            {
                var response = _service.GetContent(HelpId);

                return Ok(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new HelpAddResObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }
    }
}
