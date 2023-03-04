using MovieApp.SqlTables;
using Microsoft.AspNetCore.Identity;
using static MovieApp.Contracts.User_Identity_Obj.IdentityObjs;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using MovieApp.Helpers;

namespace MovieApp.IdentityServices
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
        public async Task<UserProfileResObj> GetAdminProfile()
        {
            var response = new UserProfileResObj
            {
               
                Status = new ApiResponse
                {
                    IsSuccess = false,
                   
                   FriendlyMessage = ""
                    
                }
            };
            
            try
            {
                string userId = _cotextAccessor?.HttpContext?.User?.FindFirst("userId")?.Value ?? string.Empty;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.Status.FriendlyMessage = "User not found";
                    return response;
                }
                var profile = new UserProfileObj
                {
                    FullName = user.FullName,
                    Email = user.Email,
                   
                };
                response.Profile = profile;
                response.Status.IsSuccess = true;
                response.Status.FriendlyMessage = "Sucessful";
                return response;
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                response.Status.FriendlyMessage = $"An error occured, Kindly submit this issue with id {errorCode} to the Admin";
                response.Status.TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "";
                response.Status.ErrorCode = errorCode;
                return response;
            }
        }
    }
}
