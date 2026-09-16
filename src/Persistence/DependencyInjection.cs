using System.Text;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Login;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Persistence.Data;
using ContractorBackend.Persistence.Data.Shared;
using ContractorBackend.Persistence.DependencyInjectionExtensions;
using ContractorBackend.Persistence.Services;
using ContractorBackend.Persistence.Services.Identity;
using ContractorBackend.Persistence.Services.Login;
using ContractorBackend.Persistence.Services.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ReportServer.Services;

namespace ContractorBackend.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomIdentityServices(configuration);

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIsSuiteOtpService, IsSuiteOtpService>();
            services.AddScoped<IUserloginService, UserloginService>();
            services.AddTransient(typeof(IHangFireSyncService<>), typeof(HangFireSyncService<>));
            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<INewsCategoryService, NewsCategoryService>();
            services.AddScoped<ISeedService, SeedService>();

            // Ghg (Carbon Footprint) services
            services.AddScoped<Application.Ghg.Services.GhgCalculationService>();
            services.AddScoped<IGhgCalculationService>(sp => sp.GetRequiredService<Application.Ghg.Services.GhgCalculationService>());
            services.AddSingleton<IActivityDataProvider, ReferenceFileActivityDataProvider>();
            services.AddScoped<GhgReferenceSeeder>();

            


            // Needed for jwt auth.
            //???
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    var Key = Encoding.UTF8.GetBytes(configuration["BearerTokensSettings:Key"].Decrypt());
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["BearerTokensSettings:Issuer"],
                        ValidAudience = configuration["BearerTokensSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Key)
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                            }
                            return Task.CompletedTask;
                        },

                    };
                });

            //???
            services.AddAuthorization(options =>
            {
                //options.AddPolicy(ConstantRoles.Admin, policy => policy.RequireRole(ConstantRoles.Admin));
                //options.AddPolicy(ConstantRoles.Manager, policy => policy.RequireRole(ConstantRoles.Manager));
                //options.AddPolicy(ConstantRoles.Customer, policy => policy.RequireRole(ConstantRoles.Customer));
                // options.AddPolicy("CanPurge", policy => policy.RequireRole(ConstantRoles.Admin));
            });

            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IReportBuilderService, ReportBuilderService>();

            return services;
        }
    }
}