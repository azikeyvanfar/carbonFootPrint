using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.Newss.Commands.CreateNews;
using ContractorBackend.Application.Shared.Newss.Commands.DeleteNews;
using ContractorBackend.Application.Shared.Newss.Commands.DeleteNewsPhoto;
using ContractorBackend.Application.Shared.Newss.Commands.UpdateNews;
using ContractorBackend.Application.Shared.Newss.Queries.GetAllNews;
using ContractorBackend.Application.Shared.Newss.Queries.GetNewsById;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core.Controllers
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class NewsController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :420-10
        /// نمایش اخبار  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست اخبار")]
        [ErrorCode("420-10")]
        public async Task<OkApiResult<SearchQueryResponse<NewsDto>>> GetAll([FromQuery] GetAllNewsQuery query)
        {
            query.IsManager = true;
            return new OkApiResult<SearchQueryResponse<NewsDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :420-20
        /// نمایش  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش یک خبر باشناسه")]
        [ErrorCode("420-20")]
        public async Task<OkApiResult<NewsDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<NewsDto>(
                await Mediator.Send(new GetNewsByIdQuery(id)));
        }


        /// <summary>
        /// UI CODE :420-30
        /// ایجاد 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد خبر")]
        [ErrorCode("420-30")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateNewsCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }


        /// <summary>
        /// UI CODE :420-40
        /// ویرایش 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش خبر")]
        [ErrorCode("420-40")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateNewsCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :420-50
        /// حذف 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف خبر")]
        [ErrorCode("420-50")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteNewsCommand(id));
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :420-53
        /// حذف پیوست مربوط به اخبار 
        /// </summary>
        /// <param name="id">is documentId or attachmentId</param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف پیوست مربوط به خبر")]
        [ErrorCode("420-53")]
        public async Task<ActionResult<OkApiResult<Guid>>> DeletePhoto([FromQuery] Guid id, [FromQuery] Guid parentId)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            await Mediator.Send(new DeleteNewsPhotoCommand(id, parentId));
            return new OkApiResult<Guid>(id);
        }
    }
}
