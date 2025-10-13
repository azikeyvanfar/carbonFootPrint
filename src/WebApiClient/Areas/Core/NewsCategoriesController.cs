using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetAllIsNotificationNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetNewsCategoryById;
using ContractorBackend.Application.Shared.NewsCategories.Queries.OGetNewsCategoryList;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core.Controllers
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]

    public class NewsCategoriesController : ApiControllerBase
    {

        /// <summary>
        /// UI CODE :410-20
        /// دریافت گروه خبر با شناسه  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت گروه خبر با شناسه")]
        [ErrorCode("410-20")]
        public async Task<OkApiResult<NewsCategoryDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<NewsCategoryDto>(
                await Mediator.Send(new GetNewsCategoryByIdQuery(id)));
        }

        /// <summary>
        /// UI CODE :410-40
        /// لیست گروه های خبر برای select 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست گروه های خبر برای select")]
        [ErrorCode("410-40")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> OGetList([FromQuery] OGetNewsCategoryListQuery query)
        {
            return new OkApiResult<SearchQueryResponse<SelectModel>>(
                await Mediator.Send(query));
        }



        /// <summary>
        /// UI CODE :410-41
        /// لیست گروه های خبر برای select 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست گروه های اطلاعیه برای select")]
        [ErrorCode("410-41")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> GetAllIsNotification([FromQuery] GetAllIsNotificationNewsCategoryQuery query)
        {
            return new OkApiResult<SearchQueryResponse<SelectModel>>(
                await Mediator.Send(query));
        }




    }
}
