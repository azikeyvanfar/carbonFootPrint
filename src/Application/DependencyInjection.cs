using System;
using System.Reflection;
using ContractorBackend.Application.Common.Behaviours;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Task;
using ContractorBackend.Application.Services;
using FluentValidation;
using Gridify;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ContractorBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(cfg =>
            {
                // تمام Profileهای موجود در اسمبلی جاری رو اضافه کن
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InputSanitizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient<HttpClientFactory>();
            services.AddTransient<HttpClientMethods>();
            services.AddScoped<IsSuiteClientService>();
            
            
            services.AddScoped<SyncUserJob>();

            services.AddScoped<ICustomLogRepository, CustomLogRepository>();
            //services.AddScoped(provider => new MapperConfiguration(cfg => cfg.AddProfile(new IndicatorProfile(provider.GetService<IApplicationDbContext>()))).CreateMapper());
            GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer();






            return services;
        }
    }
}
