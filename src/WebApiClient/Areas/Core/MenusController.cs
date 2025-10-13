using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.Menus.Queries.GetAllMenus;
using ContractorBackend.Application.Core.Menus.Queries.GetPageById;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
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


    }
}
