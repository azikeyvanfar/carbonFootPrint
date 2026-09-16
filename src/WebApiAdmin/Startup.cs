using System;
using System.Globalization;
using ContractorBackend.Application;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Common.MiddleWare;
using ContractorBackend.Infrastructure;
using ContractorBackend.Infrastructure.DependencyInjectionExtensions;
using ContractorBackend.Persistence;
using ContractorBackend.WebApiAdmin.Extensions;
using ContractorBackend.WebApiAdmin.Helpers;
using ContractorBackend.WebApiAdmin.MiddleWare;
using ContractorBackend.WebApiAdmin.Services;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
namespace ContractorBackend.WebApiAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Add Localization 
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            #endregion

            services.AddCustomOptions(Configuration);

            services.AddApplication();

            services.AddPersistence(Configuration);

            services.AddInfrastructure(Configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddCustomSwagger();
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 100_000_000;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 100_000_000;
            });

            services.AddCustomCors(Configuration);
            //services.AddCustomAntiforgery();
            services.AddCustomHangfire(Configuration);

            services.AddCustomMvc();

            services.AddDNTCommonWeb();
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = false;
                // If you need to add support for XML
                // options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });

            services.AddSession(options =>
            {
                options.IOTimeout = TimeSpan.FromSeconds(50);
                options.Cookie.HttpOnly = true;
            });

            services.AddHsts(Options =>
            {
                Options.MaxAge = TimeSpan.FromDays(365);
                Options.IncludeSubDomains = true;
                Options.Preload = true;
            });

            #region service for admin didnt login for more than 90 days
            //RecurringJob.AddOrUpdate<ServiceUserManagerConfirm>(x => x.UserDeActive(), Cron.Daily, TimeZoneInfo.Local);
            #endregion

            #region service for Register New Users to System and Update them
            //RecurringJob.AddOrUpdate<IHangFireSyncUserService>("RegisterNewUsersDaily",x => x.SyncRegisterUsers(), Cron.Daily(1,0), TimeZoneInfo.Local); 
            //RecurringJob.AddOrUpdate<IHangFireSyncUserService>("UpdateUsersDaily",x => x.SyncUpdateUsers(), Cron.Daily(2,0), TimeZoneInfo.Local); 
            #endregion

        }

        private void ReloadConfirmLegalLoan()
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomSwagger(Configuration);
            }
            else
            {
                app.UseAllowedHttpMethods();
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseCors("CorsPolicy");
            
            app.UseSession();

            //app.UserCustomStaticFileStorage(Configuration);


            app.UseSerilogRequestLogging(options =>
            {
                //use enrichment to log user ip and browser name or any other elements if not logged yet
                //options.EnrichDiagnosticContext = SerilogConfig.SetShadowPropertiesToSerilog;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var myEnricher = new SerilogConfig(Configuration);
                    myEnricher.SetShadowPropertiesToSerilog(diagnosticContext, httpContext);
                };
            });

            if (!env.IsDevelopment())
            {
                app.UseSecurityHeaders();
                app.UseSecurityHeadersRun();
                app.UseSpaFallback();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SerilogCustomMiddleware>();
            #region Use Localization
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            #endregion

            app.UseCustomHangfireDashboard();

            app.UseMiddleware<CustomRateLimitMiddleware>();

            app.AddRateLimitOptionsToConfiguration(Configuration, true);

            // app.ApplyReactUrlRewiteMapper();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapCustomHangfireDashboard();
            });

            app.AddActionList();


        }
    }
}
