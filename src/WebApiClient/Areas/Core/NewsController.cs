using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.Newss.Queries.GetAllNews;
using ContractorBackend.Application.Shared.Newss.Queries.GetAllNotificationNews;
using ContractorBackend.Application.Shared.Newss.Queries.GetNewsById;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core.Controllers
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class NewsController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :420-10
        /// لیست اخبار  
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست اخبار")]
        [ErrorCode("420-10")]
        public async Task<OkApiResult<SearchQueryResponse<NewsDto>>> GetAll([FromQuery] GetAllNewsQuery query)
        {
            query.IsManager = false;
            var res = await Mediator.Send(query);
            return new OkApiResult<SearchQueryResponse<NewsDto>>(res);
        }

        /// <summary>
        /// UI CODE :420-11
        /// لیست اطلاعیه ها  
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست اخبار")]
        [ErrorCode("420-11")]
        public async Task<OkApiResult<SearchQueryResponse<NewsDto>>> GetAllIsNotification([FromQuery] GetAllNotificationNewsQuery query)
        {
            query.IsManager = false;
            return new OkApiResult<SearchQueryResponse<NewsDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :420-20
        /// دریافت خبر با شناسه  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت خبر با شناسه")]
        public async Task<OkApiResult<NewsDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<NewsDto>(
                await Mediator.Send(new GetNewsByIdQuery(id)));
        }

    }
}
