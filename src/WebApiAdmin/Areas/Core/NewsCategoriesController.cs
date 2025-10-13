using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.NewsCategories.Commands.CreateNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Commands.DeleteNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Commands.UpdateNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetAllIsNotificationNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetAllNewsCategories;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetNewsCategoryById;
using ContractorBackend.Application.Shared.NewsCategories.Queries.GetNewsCategoryList;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class NewsCategoriesController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :410-10
        /// لیست گروههای خبر
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName("لیست گروههای خبر")]
        [ErrorCode("410-10")]
        public async Task<OkApiResult<SearchQueryResponse<NewsCategoryDto>>> GetAll([FromQuery] GetAllNewsCategoriesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<NewsCategoryDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :410-20
        /// نمایش یک گروه خبر  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش یک گروه خبر با شناسه")]
        [ErrorCode("410-20")]
        public async Task<OkApiResult<NewsCategoryDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<NewsCategoryDto>(
                await Mediator.Send(new GetNewsCategoryByIdQuery(id)));
        }

        /// <summary>
        /// UI CODE :410-30
        ///  ایجاد یک گروه خبر 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد یک گروه خبر")]
        [ErrorCode("410-30")]
        public async Task<OkApiResult<bool>> Create([FromBody] CreateNewsCategoryCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :410-40
        /// لیست  برای drps 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست گروههای خبر برای سلکت")]
        [ErrorCode("410-40")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> GetList([FromQuery] GetNewsCategoryListQuery query)
        {
            return new OkApiResult<SearchQueryResponse<SelectModel>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :410-50
        /// ویرایش گروه خبر
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش گروه خبر")]
        [ErrorCode("410-50")]
        public async Task<OkApiResult<bool>> Update([FromBody] UpdateNewsCategoryCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :410-60
        /// حذف گروه خبر 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف گروه خبر")]
        [ErrorCode("410-60")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            await Mediator.Send(new DeleteNewsCategoryCommand(id));
            return new OkApiResult<bool>(true);
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
