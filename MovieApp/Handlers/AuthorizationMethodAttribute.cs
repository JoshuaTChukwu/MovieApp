
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace MovieApp.Handlers
{
    public class AuthorizationMethodAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var response = new MiddlewareResponse { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            string userId = context.HttpContext.User?.FindFirst("userId")?.Value ?? string.Empty;
            StringValues authHeader = context.HttpContext.Request.Headers["Authorization"];

            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (context == null || hasAllowAnonymous)
            {
                await next();
                return;
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(authHeader))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }
            string token = authHeader.ToString().Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var tokena = handler.ReadJwtToken(token);
            var FromDate = tokena.IssuedAt.AddHours(1);
            var EndDate = tokena.ValidTo.AddHours(1);

            var expieryMatch = DateTime.UtcNow.AddHours(1);
            if (expieryMatch > EndDate)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }
            await next();
            return;
        }
    }
}
