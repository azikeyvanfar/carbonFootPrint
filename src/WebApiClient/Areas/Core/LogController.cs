using System;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.WebApiClient.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class LogController : ApiControllerBase
    {
        private readonly ILogDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;
        public LogController(ILogDbContext dbContext, IHttpContextAccessor accessor)
        {
            _dbContext = dbContext;
            _accessor = accessor;
        }
        ///// <summary>
        ///// UI CODE :1111
        ///// لیست ارورهای سیستم برای ادیمن
        ///// </summary>
        ///// <returns></returns> 
        //[HttpGet]
        //[DisplayName(" لیست ارورهای سیستم برای ادیمن")]
        //[ErrorCode("1111")]
        //public async Task<OkApiResult<SearchQueryResponse<ErrorHistory>>> GetAllError([FromQuery] GetAllErrorHistoryQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<ErrorHistory>>(await Mediator.Send(query));
        //}
        ///// <summary>
        ///// UI CODE :1111
        ///// لیست ارورهای سیستم برای Developer
        ///// </summary>
        ///// <returns></returns> 
        //[HttpGet]
        //[DisplayName(" لیست ارورهای سیستم برای Developer")]
        //[ErrorCode("2222")]
        //public async Task<OkApiResult<SearchQueryResponse<AppLogEvent>>> GetAllLog([FromQuery] GetAllSeriLogQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<AppLogEvent>>(
        //        await Mediator.Send(query));
        //}




        /// <summary>
        /// افزودن لیست جدید
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        [AllowAnonymous]
        public async Task<bool> CreateLog([FromBody] LogDto cmd)
        {
            //  var userId = _accessor.HttpContext.GetUserId();
            var entity = new AppLogEvent
            {
                Message = $"CPMPerformanceTiming  source={cmd.Source} , Start={cmd.Start.ToOffset(TimeSpan.FromMinutes(210)).ToString()} , End={cmd.End.ToOffset(TimeSpan.FromMinutes(210)).ToString()} , Duration={cmd.Duration}",

                //UserId = userId,
                Level = "CPMPerformanceTiming",
                TimeStamp = DateTime.Now,
                SystemName = "CPMClient"

            };
            _dbContext.AppLogEvents.Add(entity);
            var res = _dbContext.SaveChanges();
            return res > 0;
        }

    }
    public class LogDto
    {
        public string Source { get; set; }
        public decimal Duration { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

    }

}
