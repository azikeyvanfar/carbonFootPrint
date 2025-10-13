using System.Collections.Generic;
using System.IO;
using ContractorBackend.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public bool IsValidExtension(IFormFile inputFile)
        {
            string extention = Path.GetExtension(inputFile.FileName);
            string mimeType = MimeKit.MimeTypes.GetMimeType(inputFile.FileName);

            var mimeTypes = new List<string>()
            {
                "image/bmp","image/gif","image/jpeg","image/png"
            };

            var extensions = new List<string>()
            {
                ".bmp",".gif",".jpe", ".jpeg", ".jpg",".png"
            };

            if (!extensions.Contains(extention) && !mimeTypes.Contains(mimeType))
                return false;
            return true;
        }
    }
}
