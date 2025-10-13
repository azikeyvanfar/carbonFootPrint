using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IThumbnailService
    {
        Task<string> CreateThumbnailAsync(IFormFile inputFile, int width, string fileDestinationDirectory, string fileName, CancellationToken cancellationToken);
    }
}