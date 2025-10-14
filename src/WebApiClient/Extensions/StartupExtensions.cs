using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Common.Models.SiteSettings;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Filters.SwaggerFilters;
using DNTCommon.Web.Core;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
//using DocumentFormat.OpenXml.Office2021.DocumentTasks;

namespace ContractorBackend.WebApiClient.Extensions
{
    public static class StartupExtensions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {


            services.Configure<SiteSettings>(options => configuration.Bind(options));

            services.AddOptions<BearerTokensSettings>()
                .Bind(configuration.GetSection("BearerTokensSettings"))
                .Validate(bearerTokens =>
                {
                    return bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes;
                }, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");

            //services.AddOptions<ApiSettings>()
            //    .Bind(configuration.GetSection("ApiSettings"));
        }

        public static void AddCustomAntiforgery(this IServiceCollection services)
        {
            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
        }

        public static void AddCustomMvc(this IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.UseYeKeModelBinder();

                    //از حالت‌های امنی مانند GET و HEAD صرفنظر می‌کند
                    //به تمام اکشن متدهای HttpPost برنامه به صورت خودکار اعمال میشود
                    //AutoValidateAntiforgeryTokenAttribute allows to apply Anti-forgery token validation
                    //globally to all unsafe methods e.g. POST, PUT, PATCH and DELETE.
                    //Thus you don't need to add [ValidateAntiForgeryToken] attribute to each and every action that requires it.
                    //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                    //options.Filters.Add(new AuthorizeFilter());
                    options.Filters.Add(typeof(DynamicAuthorizeFilter));

                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    //options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    //options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                    options.Filters.Add(new ProducesDefaultResponseTypeAttribute());
                    //options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                    options.ReturnHttpNotAcceptable = true; // Status406NotAcceptable

                    // remove formatter that turns nulls into 204 - No Content responses
                    // this formatter breaks SPA's Http response JSON parsing
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter
                    {
                        TreatNullValueAsNoContent = false
                    });

                    //options.Filters.Add(typeof(HttpResponseExceptionFilter));
                    options.Filters.Add<ApiExceptionFilterAttribute>();
                })
                .AddFluentValidation()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            // Customise default API behaviour
            // override modelstate
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebAPI Client",
                    Description = "CPM project",
                    TermsOfService = new Uri("https://my.msc.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "MSC TEAM",
                        Url = new Uri("https://msc.com/")
                    }
                });
                c.DocumentFilter<IgnoreControllerDocumentFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
              {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));

                //c.SwaggerDoc(
                // name: "LibraryOpenAPISpecification",
                // info: new OpenApiInfo()
                // {
                //     Title = "ContractorBackend : Admin",
                //     Version = "1",
                //     Description = "WebApiClient Swagger : Through this API you can access the site's capabilities.",
                //     Contact = new OpenApiContact()
                //     {
                //         Email = "name@site.com",
                //         Name = "ContractorBackend",
                //     },
                //     License = new OpenApiLicense()
                //     {
                //         Name = "MIT License",
                //         Url = new Uri("https://opensource.org/licenses/MIT")
                //     }
                // });

            });

            //services.AddSwaggerGen(setupAction =>
            //{
            //    setupAction.SupportNonNullableReferenceTypes();
            //    setupAction.DocumentFilter<IgnoreControllerDocumentFilter>();
            //    setupAction.SchemaFilter<RequiredButNullableSchemaFilter>();
            //    setupAction.SchemaFilter<RequiredNotNullableSchemaFilter>();
            //    setupAction.SchemaFilter<EnumSchemaFilter>();
            //    setupAction.SwaggerDoc(
            //       name: "LibraryOpenAPISpecification",
            //       info: new OpenApiInfo()
            //       {
            //           Title = "ContractorBackend : Admin",
            //           Version = "1",
            //           Description = "WebApiClient Swagger : Through this API you can access the site's capabilities.",
            //           Contact = new OpenApiContact()
            //           {
            //               Email = "name@site.com",
            //               Name = "ContractorBackend",
            //           },
            //           License = new OpenApiLicense()
            //           {
            //               Name = "MIT License",
            //               Url = new Uri("https://opensource.org/licenses/MIT")
            //           }
            //       });
            //    // setupAction.AddSecurityDefinition(
            //    //"LibraryOpenAPISpecification", CreateSecurityScheme());

            //    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            //    xmlFiles.ForEach(xmlFile => setupAction.IncludeXmlComments(xmlFile));

            //    //setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    //{
            //    //    Type = SecuritySchemeType.ApiKey,
            //    //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            //    //    In = ParameterLocation.Header,
            //    //    Name = "Authorization",
            //    //    Scheme = JwtBearerDefaults.AuthenticationScheme,
            //    //    BearerFormat = "JWT"
            //    //});

            //    setupAction.OperationFilter<AuthorizationOperationFilter>();

            //    setupAction.CustomSchemaIds(x => x.FullName);//Use fully qualified object names
            //    setupAction.SchemaFilter<NamespaceSchemaFilter>();//Makes the namespaces hidden for the schemas
            //});

        }



        public static void UserCustomStaticFileStorage(this IApplicationBuilder app,
            IConfiguration configuration)
        {
            string localStorage = configuration["localStaticStoragePath"];
            var path1 = Path.Combine(localStorage, "Educations");
            var path2 = Path.Combine(localStorage, "Eligibilities");
            var path3 = Path.Combine(localStorage, "Exworkhistories");
            var path4 = Path.Combine(localStorage, "Honors");
            var path5 = Path.Combine(localStorage, "Documents");
            var path6 = Path.Combine(localStorage, "News");
            var path7 = Path.Combine(localStorage, "Suggestion");
            var path8 = Path.Combine(localStorage, "GroupMessage");
            var path9 = Path.Combine(localStorage, "RadioOrWeeklyNews/FileRadio");
            var path10 = Path.Combine(localStorage, "RadioOrWeeklyNews/Image");
            var path11 = Path.Combine(localStorage, "RadioOrWeeklyNews/FileWeeklyNews");
            var path12 = Path.Combine(localStorage, "AQIImages");
            List<string> pathes = new List<string>() { path1, path2, path3, path4, path5, path6, path7, path8, path9, path10, path11, path12 };

            foreach (var path in pathes)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(localStorage),
                RequestPath = new PathString("/Documents"),
                //EnableDirectoryBrowsing = false
            });
        }
        public static void UseCustomSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                //setupAction.SwaggerEndpoint(
                //    url: swaggerUrl,
                //    name: "ContractorBackend");
                ////setupAction.RoutePrefix = ""; //--> To be able to access it from this URL: https://localhost:5001/swagger/index.html

                setupAction.DefaultModelExpandDepth(1);
                setupAction.DefaultModelRendering(ModelRendering.Model);
                setupAction.DocExpansion(DocExpansion.None);
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
            });

            //var swaggerUrl = configuration["swaggerUrl"];
            //app.UseSwaggerUI(setupAction =>
            //{
            //    setupAction.SwaggerEndpoint(
            //        url: swaggerUrl,
            //        name: "ContractorBackend");
            //    //setupAction.RoutePrefix = ""; //--> To be able to access it from this URL: https://localhost:5001/swagger/index.html

            //    setupAction.DefaultModelExpandDepth(2);
            //    setupAction.DefaultModelRendering(ModelRendering.Model);
            //    setupAction.DocExpansion(DocExpansion.None);
            //    setupAction.EnableDeepLinking();
            //    setupAction.DisplayOperationId();
            //});
        }

        public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {

            const string allowedCorsPolicyName = "CorsPolicy";

            var corsOrigins = configuration.GetSection("CorsOrigins:AllowOrigins").Get<string[]>();
            if (corsOrigins==null || corsOrigins.Length == 0)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(allowedCorsPolicyName,
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyHeader());
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(allowedCorsPolicyName,
                    builder => builder
                    .WithOrigins(corsOrigins) //Note:  The URL must be specified without a trailing slash (/).
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
                });
            }
        }


        public static Task AddActionList(this IApplicationBuilder app)
        {
            return Task.Run(async () =>
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    try
                    {

                        var context = serviceScope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                        var _descriptor = serviceScope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();

                        var lst = new List<ActionPath>();
                        var ctrlActions = _descriptor.ActionDescriptors.Items
                         .ToList();

                        foreach (var action in ctrlActions)
                        {

                            var descriptor = action as ControllerActionDescriptor;
                            var isGlobalApi = descriptor.MethodInfo.GetCustomAttributes(typeof(IsGlobalAttribute), true).Length != 0;
                            if (descriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length == 0)
                            {
                                var displayName = action.EndpointMetadata.OfType<DisplayNameAttribute>().SingleOrDefault()?.DisplayName ?? descriptor.ActionName;
                                var obj = new ActionPath()
                                {
                                    Action = descriptor.ActionName,
                                    DisplayName = displayName,
                                    Ctrl = descriptor.ControllerName,
                                    Area = $"cli:{descriptor.RouteValues["area"]}",
                                    IsGlobal = isGlobalApi
                                };
                                lst.Add(obj);
                            }
                        }
                        var currentMethods = context.GeneralClaims.Where(_ => _.RoleType == RoleType.Employee);
                        IEnumerable<string> insertedOnes;

                        if (currentMethods != null)
                        {
                            insertedOnes = lst.Select(_ => _.FinalPath.ToLowerInvariant())
                                   .Except(currentMethods.Select(_ => _.ClaimValue.ToLowerInvariant()))
                                   .ToList();
                        }
                        else
                        {
                            insertedOnes = lst.Select(_ => _.FinalPath.ToLowerInvariant());
                        }

                        foreach (var item in insertedOnes)
                        {
                            var obj = new GeneralClaims()
                            {
                                ClaimValue = item,
                                ClaimName = lst.FirstOrDefault(_ => _.FinalPath.ToLowerInvariant() == item).DisplayName,
                                IsActive = true,
                                RoleType = RoleType.Employee,
                                Id = Guid.NewGuid(),
                                IsGlobal = lst.FirstOrDefault(_ => _.FinalPath.ToLowerInvariant() == item).IsGlobal,

                            };
                            context.GeneralClaims.Add(obj);
                            await context.SaveChangesAsync().ConfigureAwait(false);

                        }

                        //deleted from table
                        currentMethods = context.GeneralClaims.Where(_ => _.RoleType == RoleType.Employee);
                        var deletedOnes = currentMethods.Select(_ => _.ClaimValue.ToLowerInvariant()).ToList().Except(lst.Select(_ => _.FinalPath.ToLowerInvariant())).ToList();

                        foreach (var item in deletedOnes)
                        {
                            try
                            {
                                var obj = context.GeneralClaims.Include(x => x.PageRouteClaims).FirstOrDefault(x => x.ClaimValue == item);
                                context.GeneralClaims.Remove(obj);
                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }
                            catch (Exception e)
                            {
                                Log.Warning($"Action Could NOT delete. action used in AppRoleClaim or PageRouteClaim Tables. Exception Message is : {e.Message} ");
                            }
                        }
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Error in add actions in method {nameof(AddActionList)} with exception:{ex.Message}");
                        throw;
                    }
                }
            });
        }




    }
}
