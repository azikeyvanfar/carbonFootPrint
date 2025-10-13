using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.Lookups.Commands.Create;
using ContractorBackend.Application.Core.Lookups.Commands.CreateItem;
using ContractorBackend.Application.Core.Lookups.Commands.Delete;
using ContractorBackend.Application.Core.Lookups.Commands.DeleteItem;
using ContractorBackend.Application.Core.Lookups.Commands.Update;
using ContractorBackend.Application.Core.Lookups.Commands.UpdateItem;
using ContractorBackend.Application.Core.Lookups.Queries.GetAll;
using ContractorBackend.Application.Core.Lookups.Queries.GetAllItem;
using ContractorBackend.Application.Core.Lookups.Queries.GetItemById;
using ContractorBackend.Application.Core.Lookups.Queries.GetListById;
using ContractorBackend.Application.Core.LoVTypeValues.Queries.GetAllList;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Route("api/Core/[controller]/[action]")]
    [Area("Core")]
    public class LookupsController : ApiControllerBase
    {

        #region GET ACTIONS

        /// <summary>
        /// کلیه آیتم های تعریف شده به همراه نام لیست آنها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<SearchQueryResponse<LookupDto>>> GetAllLookup([FromQuery] GetAllLookupQuery query)
        {
            return new OkApiResult<SearchQueryResponse<LookupDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// کلیه آیتم های تعریف شده به همراه نام لیست آنها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<SearchQueryResponse<LookupItemDto>>> GetAllLookupItem([FromQuery] GetAllLookupItemQuery query)
        {
            return new OkApiResult<SearchQueryResponse<LookupItemDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// کلیه لیست ها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<SearchQueryResponse<LookupWithItemDto>>> GetAllLookupWithItems([FromQuery] GetAllLookupWithItemQuery query)
        {
            return new OkApiResult<SearchQueryResponse<LookupWithItemDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// جست وجو لیست براساس شناسه
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<LookupDto>> GetLookupById([FromQuery] GetLookupByIdQuery query)
        {
            return new OkApiResult<LookupDto>(await Mediator.Send(query));
        }

        /// <summary>
        /// جست وجو آیتم لیست براساس شناسه
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<LookupItemDto>> GetLookupItemById([FromQuery] GetLookupItemByIdQuery query)
        {
            return new OkApiResult<LookupItemDto>(await Mediator.Send(query));
        }

        #endregion

        #region POST ACTIONS 

        /// <summary>
        /// افزودن لیست جدید
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<LookupDto>> CreateLookup([FromBody] CreateLookupCommand cmd)
        {
            return new OkApiResult<LookupDto>(await Mediator.Send(cmd));
        }

        /// <summary>
        /// بروز رسانی لیست
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<LookupDto>> UpdateLookup([FromBody] UpdateLookupCommand cmd)
        {
            return new OkApiResult<LookupDto>(await Mediator.Send(cmd));
        }

        /// <summary>
        /// حذف لیست
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<Unit>> DeleteLookup([FromBody] DeleteLookupCommand cmd)
        {
            return new OkApiResult<Unit>(await Mediator.Send(cmd));
        }

        /// <summary>
        /// افزودن ایتم جدید به لیست های کشویی
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<LookupItemDto>> CreateLookupItem([FromBody] CreateLookupItemCommand cmd)
        {
            return new OkApiResult<LookupItemDto>(await Mediator.Send(cmd));
        }

        /// <summary>
        /// بروز رسانی آیتم
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<LookupItemDto>> UpdateLookupItem([FromBody] UpdateLookupItemCommand cmd)
        {
            return new OkApiResult<LookupItemDto>(await Mediator.Send(cmd));
        }

        /// <summary>
        /// حذف ایتم لیست 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OkApiResult<Unit>> DeleteLookupItem([FromBody] DeleteLookupItemCommand cmd)
        {
            return new OkApiResult<Unit>(await Mediator.Send(cmd));
        }



        #endregion

    }
}
