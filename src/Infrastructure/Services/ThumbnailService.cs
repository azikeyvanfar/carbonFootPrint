using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ContractorBackend.Infrastructure.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly IWebHostEnvironment _environment;

        public ThumbnailService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> CreateThumbnailAsync(IFormFile inputFile, int width, string fileDestinationDirectory, string fileName, CancellationToken cancellationToken)
        {
            var image = await Image.LoadAsync(inputFile.OpenReadStream());

            var aspectRatio = image.Height / image.Width;

            image.Mutate(x => x.Resize(width, width * aspectRatio));

            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, fileDestinationDirectory);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }

            var newFileName = inputFile.GenerateUniqFileName();

            var extension = Path.GetExtension(fileName);
            //fileName = fileName.Replace(extension, "");
            fileName = $"{newFileName}{extension}";
            var filePath = Path.Combine(uploadsRootFolder, fileName);
            await image.SaveAsync(filePath, cancellationToken);

            return fileName;
        }
    }
}
