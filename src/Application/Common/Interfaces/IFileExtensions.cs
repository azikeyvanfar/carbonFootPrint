using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Core;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IFileExtensions
    {
        Task<bool> ValidTypeFileAsync(ICollection<IFormFile> files, DocumentFolder entity);
        Task<bool> ValidTypeFileAsync(IFormFile file, DocumentFolder entity);
        Task<bool> ValidMaxCountAsync(ICollection<IFormFile> files, DocumentFolder entity);

    }
}