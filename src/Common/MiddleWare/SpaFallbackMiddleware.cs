using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ContractorBackend.Common.Middlewares
{
    public class SpaFallbackMiddleware
    {
        private RequestDelegate _next;

        public SpaFallbackMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            await _next(context);

            if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
            {
                context.Request.Path = "/index.html";
                await _next(context);
            }
        }
    }
}
