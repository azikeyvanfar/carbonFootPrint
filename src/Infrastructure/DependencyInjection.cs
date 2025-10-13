using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ContractorBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddSingleton<ICustomUploadFileService, CustomUploadFileService>();
            services.AddScoped<IDocumentService, DocumentService>();

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IThumbnailService, ThumbnailService>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileExtensions, FileExtensions>();
            services.AddScoped<ICaptchaService, CaptchaService>();
            return services;
        }
    }
}