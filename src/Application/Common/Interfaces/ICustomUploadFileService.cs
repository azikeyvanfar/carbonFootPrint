using System;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Core;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface ICustomUploadFileService
    {
        void DeleteFile(string fileNameWithUploadFolder);
        void DeleteFile(string uploadFolder, string fileName);
        void DeleteFile(string fileName, string webRootPath, string uploadFolder);
        void DeleteFiles(IQueryable<Document> docs, string webRootPath, string uploadFolder);
        Task<(bool IsSaved, string DirectoryPath, string FilePath, string FileName)> UploadFileAsync(string fileDestinationDirectory, IFormFile file, string defaultName = "");

        [Obsolete("use DocumentService Methods to save Document File Instead")]
        Task<(bool IsSaved, string FilePath, string FileName)> UploadFileInCustomStorageAsync(IFormFile file, string localstoragePath, string subFolder);
        Task<(bool IsSaved, string DirectoryPath, string FilePath)> UploadThumbnailAsync(IFormFile file, string fileDestinationDirectory, string fileName);
        Task<(bool IsSaved, string DirectoryPath, string FilePath)> UploadThumbnailInCustomStorageAsync(IFormFile file, string localstoragePath,
            string subFolder, string fileName);
        Task<(bool IsSaved, string DirectoryPath, string FilePath, string FileName)> UploadFileAsync(IFormFile file, string fileDestinationDirectory);
        Task<string> UploadFileAsync(IFormFile inputFile, string webRootPath, string uploadFolder);
    }
}
