using GOSBackend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GOSBackend.Controllers.v1
{
    [AuthorizationMethod]
    public class AdminController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AdminController> _logger;
    
        public AdminController(IHttpContextAccessor httpContextAccessor, ILogger<AdminController> logger)
        {
          _contextAccessor = httpContextAccessor;
            _logger = logger;
            
        }
        //[HttpPost(APIRoutes.Admin.ADD_HELP)]
        //public async Task<ActionResult<HelpAddResObj>> AddUpdateHelpContent([FromBody] HelpAddModel model)
        //{
        //    try
        //    {
        //        var response = await _service.AddUpdateHelpContent(model);
        //        if (response.Status.IsSuccessful)
        //            return Ok(response);
        //        return BadRequest(response);
        //    }
        //    catch (Exception ex)
        //    {

        //        var errorCode = ErrorID.Generate(5);
        //        _logger.LogError($"ErrorID : {errorCode} Ex : {ex?.InnerException?.Message ?? ex?.Message} ErrorStack : {ex?.StackTrace}");
        //        return BadRequest(new HelpAddResObj
        //        {
        //            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
        //        });
        //    }
        //}
     
    }
}
