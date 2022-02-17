using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.Utils;

namespace src.Filters
{
    public class AuthorizationCheckFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _tm = context.HttpContext.RequestServices.GetService(typeof(ITokenManager)) as ITokenManager;

            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.ModelState.AddModelError("Unauthorized", "Not Authorized");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
            }
            else
            {
                string token = context.HttpContext.Request.Headers.First(x => x.Key == "Authorization").Value;

                // TODO: Verify Token
                try
                {
                    var user = _tm?.VerifyToken(token);
                    context.HttpContext.Items["user"] = user;
                }
                catch (System.Exception e)
                {
                    context.ModelState.AddModelError("Unauthorized", e.Message);
                    context.Result = new UnauthorizedObjectResult(context.ModelState);
                }
            }
        }
    }
}
