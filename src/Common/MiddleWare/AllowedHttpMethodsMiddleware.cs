using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EmploymentBackend.Common.MiddleWare
{
    public class AllowedHttpMethodsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedMethods = new[] { "GET", "POST" };

        public AllowedHttpMethodsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_allowedMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                await context.Response.WriteAsync("HTTP method not allowed.");
                return;
            }

            await _next(context);
        }
    }
}
