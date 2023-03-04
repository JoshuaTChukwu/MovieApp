using GOSBackend.Handlers;
using GOSBackend.IdentityServices;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Contracts.Common;
using MovieApp.Helpers;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;

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

        [HttpPost(APIRoutes.Users.USER_REGISTER)]
        public async Task<ActionResult<AuthenticationResult>> RegisterAdmin([FromBody] RegisterObj model)
        {
            try
            {
                var response = await _auth.RegisterAdmin(model);
                if(response.Status.IsSuccess)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : { ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthenticationResult
                {
                    Status = new ApiResponse { IsSuccess = false,  FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.InnerException?.Message ??ex?.Message??"" }
                });
            }
        }

        [HttpPost(APIRoutes.Users.USER_VERIFY)]
        public async Task<ActionResult<AuthResponse>> VerifyAdmin([FromBody] VerifyObj model)
        {
            try
            {
                var response = await _auth.VerifyAdminRegister(model);
                if (response.Status.IsSuccess)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthResponse
                {
                    Status = new ApiResponse { IsSuccess = false,  FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "", ErrorCode = errorCode  }
                });
            }
        }


        [HttpPost(APIRoutes.Users.USER_LOGIN)]
        public async Task<ActionResult<AuthenticationResult>> LoginAdmin([FromBody] LoginCommand model)
        {
            try
            {
                var response = await _auth.LoginAdmin(model);
                if (response.Status.IsSuccess)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new AuthenticationResult
                {
                    Status = new ApiResponse { IsSuccess = false,  FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "", ErrorCode = errorCode  }
                });
            }
        }
        [AuthorizationMethod]
        [HttpGet(APIRoutes.Users.USER_PROFILE_GET)]
        public async Task<ActionResult<UserProfileResObj>> GetAdminProfile()
        {
            try
            {
                var response = await _profile.GetAdminProfile();
                if (response.Status.IsSuccess)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new UserProfileResObj
                {
                    Status = new ApiResponse { IsSuccess = false,  FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "", ErrorCode = errorCode }
                });
            }
        }
    }
}