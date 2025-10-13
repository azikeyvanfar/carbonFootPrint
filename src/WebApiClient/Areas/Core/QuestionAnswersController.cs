using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Shared.QuestionAnswers.Commands.CreateQA;
using ContractorBackend.Application.Shared.QuestionAnswers.Commands.DeleteQA;
using ContractorBackend.Application.Shared.QuestionAnswers.Commands.UpdateQA;
using ContractorBackend.Application.Shared.QuestionAnswers.Queries.GetByIdQA;
using ContractorBackend.Application.Shared.QuestionAnswers.Queries.OGetAllQA;
using ContractorBackend.Common.Extensions;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class QuestionAnswersController : ApiControllerBase
    {

        /// <summary>
        /// UI CODE :750-20
        /// نمایش پرسش ها 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست سوال جوابها")]
        [ErrorCode("750-20")]
        public async Task<OkApiResult<SearchQueryResponse<QuestionAnswersDto>>> OGetAll([FromQuery] OGetAllQuestionAnswersQuery query)
        {
            query.IsAdmin = false;
            return new OkApiResult<SearchQueryResponse<QuestionAnswersDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :750-30
        /// نمایش  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت سوال جواب با شناسه")]
        [ErrorCode("750-30")]

        public async Task<OkApiResult<QuestionAnswersDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<QuestionAnswersDto>(
                await Mediator.Send(new GetByIdQuestionAnswersQuery(id)));
        }


        /// <summary>
        /// UI CODE :750-50
        /// ایجاد 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد")]
        [ErrorCode("750-50")]
        public async Task<OkApiResult<bool>> OCreate([FromForm] CreateQuestionAnswersCommand command)
        {
            command.QuestionerId = HttpContext.GetUserId();
            command.IsAdminType = false;
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }


        /// <summary>
        /// UI CODE :750-80
        /// ویرایش 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش")]
        [ErrorCode("750-80")]
        public async Task<OkApiResult<bool>> OUpdate([FromForm] UpdateQuestionAnswersCommand command)
        {
            command.QuestionerId = HttpContext.GetUserId();
            command.IsFromEmployee = true;
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :750-90
        /// حذف 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف")]
        [ErrorCode("750-90")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteQuestionAnswersCommand(id, true));
            return new OkApiResult<bool>(true);
        }

    }
}
