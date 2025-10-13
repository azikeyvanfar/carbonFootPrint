using System;
using ContractorBackend.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ContractorBackend.WebApiClient.Helpers
{
    public class SerilogConfig
    {
        private readonly IConfiguration _configuration;

        public SerilogConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SetShadowPropertiesToSerilog(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            long userId = 0;
            try
            {
                userId = httpContext.GetUserId();
            }
            catch (System.Exception)
            {

            }

            var UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString();
            var createdByIP = httpContext?.Connection?.RemoteIpAddress?.ToString();
            var now = DateTimeOffset.UtcNow;

            diagnosticContext.Set("UserId", userId);
            diagnosticContext.Set("CreatedDateTime", now);
            diagnosticContext.Set("CreatedByIP", createdByIP);
            diagnosticContext.Set("CreatedByBrowserName", UserAgent);


            var projectname = _configuration.GetSection("SystemName").Value;
            diagnosticContext.Set("SystemName", projectname);

        }
    }

    public class ShadowPropertyEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        public ShadowPropertyEnricher(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _accessor = accessor;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.ContainsKey("UserId"))
            {

            }
            else
            {
                long userId = 0;
                try
                {
                    userId = _accessor.HttpContext.GetUserId();

                }
                catch (Exception)
                {
                }
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("userId", userId));

            }



            var UserAgent = _accessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            var UserAgentSpecific = _accessor.HttpContext?.Request?.Headers["sec-ch-ua"].ToString();

            var createdByIP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var now = DateTimeOffset.UtcNow;


            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CreatedDateTime", now));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CreatedByIP", createdByIP));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CreatedByBrowserName", UserAgent));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CreatedByBrowserNameSpecific", UserAgentSpecific));

            var projectname = _configuration.GetSection("SystemName").Value;
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SystemName", projectname));

        }
    }
}
