using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Menus.Commands.CreateMenu;
using ContractorBackend.Application.Menus.Commands.DeleteMenu;
using ContractorBackend.Application.Menus.Commands.UpdateMenu;
using ContractorBackend.Application.Menus.Queries.GetAllMenus;
using ContractorBackend.Application.Menus.Queries.GetMenuById;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class MenusController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :130-10
        /// نمایش منوی محتواها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش منوها")]
        [ErrorCode("130-10")]
        [IsGlobal]
        public async Task<OkApiResult<SearchQueryResponse<MenuDto>>> GetAll([FromQuery] GetAllMenusQuery query)
        {
            return new OkApiResult<SearchQueryResponse<MenuDto>>(await Mediator.Send(new GetAllMenusQuery()));
        }

        /// <summary>
        /// UI CODE :130-20
        /// نمایش منوی محتوا 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش یک منو با شناسه")]
        [ErrorCode("130-20")]
        public async Task<OkApiResult<MenuDto>> GetById([FromQuery] Guid Id)
        {
            return new OkApiResult<MenuDto>(
                await Mediator.Send(new GetMenuByIdQuery(Id)));
        }

        /// <summary>
        /// UI CODE :130-30
        /// ایجاد منوی محتوا
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجادمنو")]
        [ErrorCode("130-30")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateMenuCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :130-40
        /// ویرایش منوی محتوا
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش منو")]
        [ErrorCode("130-40")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateMenuCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :130-50
        /// حذف منوی محتوا
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف منو")]
        [ErrorCode("130-50")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return BadRequest();
            }

            await Mediator.Send(new DeleteMenuCommand(Id));
            return new OkApiResult<bool>(true);
        }


    }
}
