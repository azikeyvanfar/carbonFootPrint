using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace ContractorBackend.WebApiClient.MiddleWare
{
    public class SerilogCustomMiddleware
    {
        private readonly RequestDelegate next;

        public SerilogCustomMiddleware(RequestDelegate next)
        {
            this.next = next;

        }
        public Task Invoke(HttpContext context)
        {

            if (context.User.Claims.Any())
            {
                var displayName = context.User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value ?? "";
                var userId = context.User.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata")?.Value ?? "";
                LogContext.PushProperty("UserDisplayName", displayName);
                LogContext.PushProperty("UserName", context.User.Identity.Name);
                LogContext.PushProperty("UserId", userId);

            }
            var clientIp = context.Connection.RemoteIpAddress + ":" + context.Connection.LocalPort;
            LogContext.PushProperty("CreatedByIp", clientIp);
            //admin
            LogContext.PushProperty("ProjectType", 0);




            return next(context);
        }
    }
}
