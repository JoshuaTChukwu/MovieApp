using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Contracts.Common;
using MovieApp.Handlers;
using MovieApp.Helpers;
using MovieApp.Repository.Interface;
using static MovieApp.Contracts.Common.AuxillaryObjs;
using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;
using static MovieApp.Contracts.User_Identity_Obj.UsersOperationalObjs;

namespace GOSBackend.Controllers.v1
{
    [AuthorizationMethod]
    public class UsersController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersInterface _service;
    
        public UsersController(IHttpContextAccessor httpContextAccessor, ILogger<UsersController> logger, IUsersInterface usersInterface)
        {
          _contextAccessor = httpContextAccessor;
            _logger = logger;
            _service = usersInterface;
            
        }
        [HttpGet(APIRoutes.Users.SEARCH_MOVIE)]
        public ActionResult<SearchResponseObj> SearchMovie([FromQuery] SearchParams model)
        {
            try
            {
                var response =  _service.seachMovie(model);
                if (response.Status.IsSuccess)
                    return Ok(response);
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                var errorCode = ErrorID.Generate(5);
                _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
                return BadRequest(new SearchResponseObj
                {
                    Status = new ApiResponse { IsSuccess = false,  FriendlyMessage = ex?.Message ??"", TechnicalMessage = ex?.InnerException?.Message ?? ex?.Message ?? "", ErrorCode = errorCode  }
                });
            }
        }

    }
}
