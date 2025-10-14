using System;
using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Logger
{
    public interface IAppLogItemsService
    {
        System.Threading.Tasks.Task DeleteAllAsync(string logLevel = "");
        System.Threading.Tasks.Task DeleteAsync(Guid logItemId);
        System.Threading.Tasks.Task DeleteOlderThanAsync(DateTimeOffset cutoffDateUtc, string logLevel = "");
        Task<int> GetCountAsync(string logLevel = "");
        //Task<PagedAppLogItemsViewModel> GetPagedAppLogItemsAsync(int pageNumber, int pageSize, SortOrder sortOrder, string logLevel = "");
    }
}