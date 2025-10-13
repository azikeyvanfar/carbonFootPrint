using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Data.Shared
{
    public class LookupRepository : ILookupRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public IQueryable<LookupDto> GetAllListQuery => _dbContext.Lookups.Include(x => x.Parent).Where(x => x.Type == LookupType.Lookup).AsNoTracking().AsQueryable().ProjectTo<LookupDto>(_mapper.ConfigurationProvider);

        public IQueryable<LookupItemDto> GetAllItemQuery => _dbContext.Lookups.Include(x => x.Parent).Where(x => x.Type == LookupType.Item).OrderBy(x => x.Priority).Select(x => new LookupItemDto
        {
            Id = x.Id,
            IsActive = x.IsActive,
            EnName = x.EnName,
            FaName = x.FaName,
            Code = x.Code,
            EnumCode = x.EnumCode,
            ListName = _dbContext.Lookups.FirstOrDefault(p => p.Type == LookupType.Lookup && p.EnName.ToLower() == x.Code.ToLower()).FaName,
            ParentId = x.ParentId == null ? null : x.ParentId,
            ParentName = x.ParentId == null ? null : x.Parent.FaName,
            ParentListName = x.ParentId == null ? null : _dbContext.Lookups.FirstOrDefault(p => p.Type == LookupType.Lookup && p.EnName.ToLower() == x.Parent.Code.ToLower()).FaName,
            Priority = x.Priority == null ? 1 : x.Priority.Value

        }).AsNoTracking().AsQueryable();

        public LookupRepository(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region List Actions

        /// <summary>
        /// کلیه لیست ها به انضمام آیتم های زیر مجموعه آنها
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IQueryable<LookupWithItemDto>> GetAllListWithItem()
        {
            var parentList = await _dbContext.Lookups.Where(x => x.Type == LookupType.Lookup).Include(x => x.Children).ToListAsync();
            List<LookupWithItemDto> data = new();
            foreach (var parent in parentList)
            {
                LookupWithItemDto listWithItemDto = new()
                {
                    Id = parent.Id,
                    Name = parent.FaName,
                    ParentId = parent.ParentId == null ? null : parent.ParentId.Value,
                    ParentName = parent.ParentId == null ? null : parent.Parent.FaName,
                    ChildList = parent.Children == null ? null : _mapper.Map<List<LookupDto>>(parent.Children),
                };

                var items = await _dbContext.Lookups.Where(x => x.Type == LookupType.Item && x.Code == parent.EnName).ToListAsync();
                if (items.Any())
                    listWithItemDto.Items = _mapper.Map<List<LookupItemDto>>(items);

                data.Add(listWithItemDto);
            }

            return data.AsQueryable();
        }

        public async Task<List<LookupDto>> GetAllList()
        {
            var list = await _dbContext.Lookups.Where(x => x.Type == LookupType.Lookup).ProjectTo<LookupDto>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            return list;
        }

        private async Task<Lookup> GetList(Guid Id)
        {
            var list = await _dbContext.Lookups.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == Id);
            return list;
        }

        /// <summary>
        /// پیدا کردن رکورد لیست با استفاده از شناسه
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<LookupDto> GetListById(Guid Id)
        {
            var list = await GetList(Id);
            if (list == null) throw new Exception($"Id : {Id} invalid.record not found");
            return _mapper.Map<LookupDto>(list);
        }

        /// <summary>
        /// ساخت لیست جدید
        /// </summary>
        /// <param name="addLookupDto"></param>
        /// <returns></returns>
        public async Task<LookupDto> CreateList(AddLookupDto addLookupDto)
        {
            var list = _mapper.Map<Lookup>(addLookupDto);
            await _dbContext.Lookups.AddAsync(list);
            await _dbContext.SaveChangesAsync();
            return await GetListById(list.Id);
        }


        /// <summary>
        /// بروز رسانی لیست
        /// </summary>
        /// <param name="addLookupDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<LookupDto> UpdateList(UpdateLookupDto updateLookupDto)
        {
            var list = await GetList(updateLookupDto.Id);
            if (list == null) throw new Exception($"Id :{updateLookupDto.Id} invalid. record not found");
            _mapper.Map(updateLookupDto, list);
            _dbContext.Lookups.Update(list);
            await _dbContext.SaveChangesAsync();
            return await GetListById(list.Id);
        }

        /// <summary>
        /// حذف لیست 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="CustomException"></exception>
        public async Task DeleteList(Guid Id)
        {
            var record = await GetList(Id);
            if (record is null) throw new Exception($"id :{Id} invalid.item not found!");

            if (await CheckListHasChildernList(record.Id)) throw new CustomException("لیست فعلی دارای لیست زیر مجموعه می باشد");

            await DeleteLookupItem(record);
            _dbContext.Lookups.Remove(record);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// حذف کردن کل آیتم های لیست
        /// </summary>
        /// <param name=TypeEnum.Lookup></param>
        /// <returns></returns>
        private async Task DeleteLookupItem(Lookup list)
        {
            var items = await _dbContext.Lookups.Where(x => x.Code.ToLower() == list.EnName.ToLower()).ToListAsync();
            foreach (var item in items)
            {
                await DeleteItem(item.Id);
            }
        }

        /// <summary>
        /// آیا لیست دارای لیست زیر مجموعه می باشد
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private async Task<bool> CheckListHasChildernList(Guid Id)
        {
            return await _dbContext.Lookups.AnyAsync(x => x.Type == LookupType.Lookup && x.ParentId == Id);
        }


        #endregion

        #region List Item Actions

        private async Task<Lookup> GetItem(Guid Id)
        {
            var item = await _dbContext.Lookups.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == Id);
            return item;
        }

        /// <summary>
        /// جست و جو آیتم براساس شناسه
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<LookupItemDto> GetItemById(Guid Id)
        {
            var item = await GetItem(Id);
            LookupItemDto itemDto = new();
            _mapper.Map(item, itemDto);
            itemDto.ListName = await GetListName(item.Code);
            if (item is null) throw new Exception($"id : {Id} invalid.item not found!");
            if (item.ParentId.HasValue)
            {
                itemDto.ParentListName = await GetListName(item.Parent.Code);
            }
            return itemDto;
        }

        public async Task<string> GetListName(string Code)
        {
            var list = await _dbContext.Lookups.FirstOrDefaultAsync(x => x.EnName.ToLower() == Code.ToLower());
            return list.FaName;
        }

        /// <summary>
        /// جست و جو نمایش ایتم های یک لیست با اولویت نمایشی
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<List<LookupItemDto>> GetItemsByType(string Code)
        {
            var query = _dbContext.Lookups.Include(x => x.Parent).Where(x => x.Code.ToLower() == Code.Trim().ToLower())
                .OrderBy(x => x.Priority)
                .AsNoTracking()
                .ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider);

            var items = await query.ToListAsync();

            return items;
        }

        /// <summary>
        /// ایجاد ایتم برای لیست
        /// </summary>
        /// <param name="addLookupItemDto"></param>
        /// <returns></returns>
        public async Task<LookupItemDto> CreateItem(AddLookupItemDto addLookupItemDto)
        {
            var category = _dbContext.Lookups.FirstOrDefault(x => x.Id == addLookupItemDto.CategoryId);
            ArgumentNullException.ThrowIfNull(category);
            var item = _mapper.Map<Lookup>(addLookupItemDto);
            item.Code = category.EnName;
            await _dbContext.Lookups.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return await GetItemById(item.Id);
        }

        /// <summary>
        /// بروز رسانی آیتم لیست
        /// </summary>
        /// <param name="updateLookupItemDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<LookupItemDto> UpdateItem(UpdateLookupItemDto updateLookupItemDto)
        {
            var item = await GetItem(updateLookupItemDto.Id);
            if (item is null) throw new Exception($"id :{updateLookupItemDto.Id} invalid.item not found!");

            var category = _dbContext.Lookups.FirstOrDefault(x => x.Id == updateLookupItemDto.CategoryId);
            ArgumentNullException.ThrowIfNull(category);
            _mapper.Map(updateLookupItemDto, item);
            item.Code = category.EnName;

            _dbContext.Lookups.Update(item);
            await _dbContext.SaveChangesAsync();

            return await GetItemById(item.Id);
        }

        /// <summary>
        /// حذف آیتم از لیست
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeleteItem(Guid Id)
        {
            var item = await GetItem(Id);
            if (await CheckItemHasChildrenItem(item.Id)) throw new CustomException("امکان حذف آیتم به دلیل داشتن آیتم های زیر مجموعه وجود ندارد");
            _dbContext.Lookups.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// چک کردن ایتم ایا زیر مجموعه دارد یا خیر
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public async Task<bool> CheckItemHasChildrenItem(Guid ItemId)
        {
            return await _dbContext.Lookups.AnyAsync(x => x.ParentId == ItemId);
        }

        #endregion

    }
}
