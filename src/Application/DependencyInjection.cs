using System.Reflection;
using ContractorBackend.Application.Common.Behaviours;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using FluentValidation;
using Gridify;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ContractorBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InputSanitizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient<HttpClientFactory>();
            services.AddTransient<HttpClientMethods>();
            services.AddScoped<IsSuiteClientService>();
            services.AddScoped<ICustomLogRepository, CustomLogRepository>();
            //services.AddScoped(provider => new MapperConfiguration(cfg => cfg.AddProfile(new IndicatorProfile(provider.GetService<IApplicationDbContext>()))).CreateMapper());
            GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer();






            return services;
        }
    }
}
