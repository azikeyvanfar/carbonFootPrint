using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.ErrorHistorys.Queries.GetAll;
using ContractorBackend.Application.Core.SeriLogs.Queries.GetAll;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Log;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class LogController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :1111
        /// لیست ارورهای سیستم برای ادیمن
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName(" لیست ارورهای سیستم برای ادیمن")]
        [ErrorCode("1111")]
        public async Task<OkApiResult<SearchQueryResponse<ErrorHistory>>> GetAllError([FromQuery] GetAllErrorHistoryQuery query)
        {
            return new OkApiResult<SearchQueryResponse<ErrorHistory>>(await Mediator.Send(query));
        }
        /// <summary>
        /// UI CODE :1111
        /// لیست ارورهای سیستم برای Developer
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName(" لیست ارورهای سیستم برای Developer")]
        [ErrorCode("2222")]
        public async Task<OkApiResult<SearchQueryResponse<AppLogEvent>>> GetAllLog([FromQuery] GetAllSeriLogQuery query)
        {
            return new OkApiResult<SearchQueryResponse<AppLogEvent>>(
                await Mediator.Send(query));
        }
    }
}
