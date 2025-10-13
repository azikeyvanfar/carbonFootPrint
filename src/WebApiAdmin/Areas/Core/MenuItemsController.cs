using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.MenuItems.Commands.CreateMenuItem;
using ContractorBackend.Application.MenuItems.Commands.DeleteMenuItem;
using ContractorBackend.Application.MenuItems.Commands.UpdateMenuItem;
using ContractorBackend.Application.MenuItems.Commands.UpdateMenuItemPriority;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItem;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItemByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetAllMenuItemsTree;
using ContractorBackend.Application.MenuItems.Queries.GetCurrentUserMenuItemTreeByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetMenuItemById;
using ContractorBackend.Application.MenuItems.Queries.GetMenuItemTreeByMenuId;
using ContractorBackend.Application.MenuItems.Queries.GetMenuTypes;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
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
            return new OkApiResult<IEnumerable<MenuItemDto>>(await Mediator.Send(query));
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
        /// UI CODE :140-03
        /// ایجاد منوی محتوا
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجادمنوآیتم")]
        [ErrorCode("140-03")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateMenuItemCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :140-04
        /// ویرایش منوی محتوا
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش منوآیتم")]
        [ErrorCode("140-04")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateMenuItemCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }
        /// <summary>
        /// UI CODE :140-05
        /// ویرایش ترتیب منو 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش ترتیب منو")]
        [ErrorCode("140-05")]
        public async Task<OkApiResult<bool>> UpdatePriority([FromForm] UpdateMenuItemPriorityCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }
        /// <summary>
        /// UI CODE :140-06
        /// حذف منوی محتوا
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف منوآیتم")]
        [ErrorCode("140-06")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid Id)
        {
            if (Id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteMenuItemCommand(Id));
            return new OkApiResult<bool>(true);
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
            request.RequestedRoleType = RoleType.Manager;

            return new OkApiResult<IEnumerable<MenuItemDto>>(await Mediator.Send(request));
        }

    }
}
