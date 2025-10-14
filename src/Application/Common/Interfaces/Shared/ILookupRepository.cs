using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Core;

namespace ContractorBackend.Application.Common.Interfaces.Shared
{
    public interface ILookupRepository
    {
        Task<string> GetListName(string Code);
        IQueryable<LookupItemDto> GetAllItemQuery { get; }
        IQueryable<LookupDto> GetAllListQuery { get; }
        Task<List<LookupDto>> GetAllList();
        Task<IQueryable<LookupWithItemDto>> GetAllListWithItem();
        Task<LookupDto> GetListById(Guid Id);
        Task<LookupDto> CreateList(AddLookupDto addLookupDto);
        Task<LookupDto> UpdateList(UpdateLookupDto updateLookupDto);
        Task<LookupItemDto> GetItemById(Guid Id);
        Task<LookupItemDto> CreateItem(AddLookupItemDto addLookupItemDto);
        Task<LookupItemDto> UpdateItem(UpdateLookupItemDto updateLookupItemDto);
        Task<List<LookupItemDto>> GetItemsByType(string Code);
        System.Threading.Tasks.Task DeleteList(Guid Id);
        System.Threading.Tasks.Task DeleteItem(Guid Id);
    }

}
