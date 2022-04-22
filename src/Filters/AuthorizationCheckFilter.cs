using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using src.Interfaces;
using src.Models;

namespace src.Filters
{
    public class AuthorizationCheckFilter : Attribute, IAsyncAuthorizationFilter
    {
        private int[] _filteredUserLevel = { };
        public AuthorizationCheckFilter()
        {
        }

        public AuthorizationCheckFilter(int l1 = -1, int l2 = -1, int l3 = -1, int l4 = -1)
        {
            List<int> temp = new List<int>();
            if (l1 != -1) temp.Add(l1);
            if (l2 != -1) temp.Add(l2);
            if (l3 != -1) temp.Add(l3);
            if (l4 != -1) temp.Add(l4);
            _filteredUserLevel = temp.ToArray();
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var _tm = context.HttpContext.RequestServices.GetService(typeof(ITokenManager)) as ITokenManager;
            if (_tm == null)
            {
                SendUnauthorized(context);
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                SendUnauthorized(context);
                return;
            }

            try
            {
                string token = context.HttpContext.Request.Headers.First(x => x.Key == "Authorization").Value;
                User user = await _tm.VerifyToken(token);
                if (!user.IsConfirmed)
                {
                    SendUnauthorized(context, "not-confirmed");
                    return;
                }
                if (!IsQualified(user))
                {
                    SendUnauthorized(context);
                    return;
                }
                context.HttpContext.Items["user"] = user;
            }
            catch (System.Exception)
            {
                SendUnauthorized(context);
            }

        }

        private bool IsQualified(User user)
        {
            if (_filteredUserLevel.Length == 0)
            {
                return true;
            }

            return _filteredUserLevel.Contains(user.Level) ? true : false;
        }

        public void SendUnauthorized(AuthorizationFilterContext context, string? msg = null)
        {
            context.HttpContext.Response.StatusCode = 401;
            object? data = null;
            context.Result = new JsonResult(new
            {
                success = false,
                message = msg ?? "Unauthorized",
                data
            });
        }
    }
}