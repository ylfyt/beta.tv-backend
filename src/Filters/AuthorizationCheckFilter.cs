using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.Interfaces;
using src.Dtos;

namespace src.Filters
{
    public class AuthorizationCheckFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _tm = context.HttpContext.RequestServices.GetService(typeof(ITokenManager)) as ITokenManager;

            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.HttpContext.Response.StatusCode = 401;
                object? data = null;
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = "Unauthorized",
                    data
                });
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
                catch (System.Exception)
                {
                    context.HttpContext.Response.StatusCode = 401;
                    object? data = null;
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = "Unauthorized",
                        data
                    });
                }
            }
        }
    }
}