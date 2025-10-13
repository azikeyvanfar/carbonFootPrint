using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface INewsCategoryService
    {
        Task<bool> CheckExistNameAsync(Guid? id, string name, CancellationToken cancellationToken);
    }
}
