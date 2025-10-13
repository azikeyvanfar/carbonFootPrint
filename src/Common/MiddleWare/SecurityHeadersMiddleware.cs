
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Common.MiddleWare
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var corsOrigins = _configuration.GetSection("CorsOrigins:AllowOrigins").Get<string[]>();
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("Referrer-Policy", "Origin-When-Cross-Origin");
            context.Response.Headers.Add("Content-Security-Policy", $"default-src 'self'; connect-src 'self' {corsOrigins};");
            context.Response.Headers.Add("Access-Control-Allow-origin", $"{corsOrigins};");
            context.Response.Headers.Add("Permissions-Policy", "geolocation=(self), microphone=(), camera=()");

            context.Response.Headers.Add("Content-Security-Policy", $"default-src 'self'; connect-src 'self' {corsOrigins};");
            context.Response.Headers.Add("Access-Control-Allow-origin", $"{corsOrigins};");

            context.Response.Cookies.Delete("cookiesession1");

            await _next(context);
        }
    }
}
