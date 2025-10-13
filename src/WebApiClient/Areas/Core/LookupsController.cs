using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.Lookups.Queries.GetAllItem;
using ContractorBackend.Application.Core.Lookups.Queries.GetAllScopeByCurrentUser;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/Core/[controller]/[action]")]
    public class LookupsController : ApiControllerBase
    {
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
        /// لیست حوزه های سیستم بر اساس نقش کاربر
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [IsGlobal]
        public async Task<OkApiResult<SearchQueryResponse<LookupItemDto>>> GetAllScopeByCurrentUser([FromQuery] GetAllScopeByCurrentUserQuery query)
        {
            return new OkApiResult<SearchQueryResponse<LookupItemDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// لیست شاخص ها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OkApiResult<SearchQueryResponse<LookupItemDto>>> GetAllIndicator([FromQuery] GetAllLookupItemQuery query)
        {
            query.Code = "indicator";
            query.ParentId = null;

            return new OkApiResult<SearchQueryResponse<LookupItemDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// لیست شاخص های سیستم فعلی و سیستم قدیم
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<OkApiResult<SearchQueryResponse<LookupItemDto>>> GetAllScoreIndicator([FromQuery] GetAllLookupItemQuery query)
        {
            query.Code = "indicator";
            query.ParentId = null;
            var res1 = await Mediator.Send(query);
            var lst = res1.Items.ToList();

            query.Code = "oldCpmIndicator";
            query.ParentId = null;
            var res2 = await Mediator.Send(query);

            res2.Items = res2.Items.Concat(lst);
            res2.TotalCount = res1.TotalCount + res2.TotalCount;

            return new OkApiResult<SearchQueryResponse<LookupItemDto>>(res2);

        }


    }
}
