using ContractorBackend.Common.Middlewares;
using EmploymentBackend.Common.MiddleWare;
using Microsoft.AspNetCore.Builder;

namespace ContractorBackend.Common.MiddleWare
{
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }

        public static IApplicationBuilder UseAllowedHttpMethods(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AllowedHttpMethodsMiddleware>();
        }

        public static IApplicationBuilder UseSpaFallback(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SpaFallbackMiddleware>();
        }
    }
}
