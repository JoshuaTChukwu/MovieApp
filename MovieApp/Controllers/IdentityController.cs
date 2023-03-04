using GOSBackend.Contracts.Common;
using GOSBackend.Contracts.User_Identity_Obj;
using GOSBackend.Handlers;
using GOSBackend.IdentityServices;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using Microsoft.AspNetCore.Mvc;
using static GOSBackend.Contracts.User_Identity_Obj.AdminIdentityObjs;

namespace GOSBackend.Controllers
{
    
    public class IdentityController : ControllerBase
    {
       

        private readonly ILogger<IdentityController> _logger;
        private readonly IAuthService _auth;
        private readonly IProfileService _profile;

        public IdentityController(ILogger<IdentityController> logger, IAuthService authService, IProfileService profile)
        {
            _logger = logger;
            _auth = authService;
            _profile = profile;
        }

        [HttpPost(APIRoutes.Admin.ADMIN_REGISTER)]
        public async Task<ActionResult<AuthenticationResult>> RegisterAdmin([FromBody] AdminRegisterObj model)
        {
            try
            {
                var response = await _auth.RegisterAdmin(model);
                if(response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : { ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthenticationResult
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }

        [HttpPost(APIRoutes.Admin.ADMIN_VERIFY)]
        public async Task<ActionResult<AuthResponse>> VerifyAdmin([FromBody] AdminVerifyObj model)
        {
            try
            {
                var response = await _auth.VerifyAdminRegister(model);
                if (response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthResponse
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }


        [HttpPost(APIRoutes.Admin.ADMIN_LOGIN)]
        public async Task<ActionResult<AuthenticationResult>> LoginAdmin([FromBody] LoginCommand model)
        {
            try
            {
                var response = await _auth.LoginAdmin(model);
                if (response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthenticationResult
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }
        [AuthorizationMethod]
        [HttpGet(APIRoutes.Admin.ADMIN_PROFILE_GET)]
        public async Task<ActionResult<UserAdminProfileResObj>> GetAdminProfile()
        {
            try
            {
                var response = await _profile.GetAdminProfile();
                if (response.Status.IsSuccessful)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new UserAdminProfileResObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                });
            }
        }
    }
}