using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItem;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItemByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItemsTree;
using ContractorBackend.Application.MenuItems.Queries.GetCurrentUserMenuItemTreeByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetMenuItemById;
using ContractorBackend.Application.MenuItems.Queries.GetMenuItemTreeByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetMenuTypes;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class MenuItemsController : ApiControllerBase
    {

        /// <summary>
        /// UI CODE :140-01
        /// نمایش منوها 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش منوآیتم ها")]
        [ErrorCode("140-01")]
        [IsGlobal]
        public async Task<OkApiResult<IEnumerable<MenuItemDto>>> GetAll([FromQuery] GetAllMenuItemsTreeQuery query)
        {
            return new OkApiResult<IEnumerable<MenuItemDto>>(
                  await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :140-01
        /// نمایش منوی بر اساس منو 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش منوی بر اساس منوی والد")]
        [ErrorCode("140-01")]
        public async Task<OkApiResult<IEnumerable<MenuItemDto>>> GetByMenuId(Guid menuId)
        {
            return new OkApiResult<IEnumerable<MenuItemDto>>(await Mediator.Send(new GetMenuItemTreeByMenuIdQuery(menuId)));
        }

        /// <summary>
        /// UI CODE :140-02
        /// نمایش منوی محتوا 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش یک منو آیتم بوسیله شناسه")]
        [ErrorCode("140-02")]
        public async Task<OkApiResult<MenuItemDto>> GetById([FromQuery] Guid Id)
        {
            return new OkApiResult<MenuItemDto>(await Mediator.Send(new GetMenuItemByIdQuery(Id)));

        }


        /// <summary>
        /// UI CODE :140-07
        /// نمایش انواع منو  
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("نمایش انواع منو")]
        [ErrorCode("140-07")]
        [IsGlobal]
        public async Task<OkApiResult<List<SelectModel>>> GetMenuTypes()
        {
            //query.PageSize = 10;
            return new OkApiResult<List<SelectModel>>(
                await Mediator.Send(new GetMenuTypesQuery()));
        }

        /// <summary>
        /// UI CODE :140-08
        /// نمایش لیست منوآیتم ها  
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName("نمایش لیست منوآیتم ها")]
        [ErrorCode("140-08")]
        public async Task<OkApiResult<SearchQueryResponse<MenuItemDto>>> GetAllMenuItem([FromQuery] GetAllMenuItemQuery query)
        {
            return new OkApiResult<SearchQueryResponse<MenuItemDto>>(
                await Mediator.Send(query));
        }
        /// <summary>
        /// UI CODE :140-09
        /// نمایش لیست منوآیتم ها بر اساس menuId  
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName("نمایش لیست منوآیتم ها بر اساس menuId")]
        [ErrorCode("140-09")]
        public async Task<OkApiResult<SearchQueryResponse<MenuItemDto>>> GetAllMenuItemByMenuId([FromQuery] GetAllMenuItemByMenuIdQuery request)
        {
            return new OkApiResult<SearchQueryResponse<MenuItemDto>>(await Mediator.Send(request));
        }

        /// <summary>
        /// UI CODE :140-10 
        /// نمایش منوهای کاربر بر اساس منوی والد
        /// </summary>
        /// <param name="menuId"></param> 
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش منوهای کاربر بر اساس منوی والد")]
        [ErrorCode("140-10")]
        [IsGlobal]
        public async Task<OkApiResult<IEnumerable<MenuItemDto>>> GetCurrentUserByMenuId(Guid menuId)
        {
            var request = new GetCurrentUserMenuItemTreeByMenuIdQuery();
            request.MenuId = menuId;
            request.RequestedRoleType = RoleType.Employee;

            return new OkApiResult<IEnumerable<MenuItemDto>>(await Mediator.Send(request));
        }

    }
}
