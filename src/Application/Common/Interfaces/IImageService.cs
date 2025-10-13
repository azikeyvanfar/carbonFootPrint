using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IImageService
    {
        bool IsValidExtension(IFormFile inputFile);
    }
}
