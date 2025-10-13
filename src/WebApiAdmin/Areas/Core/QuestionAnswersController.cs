using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.QuestionAnswers.Commands.ChangeQAStatus;
using ContractorBackend.Application.QuestionAnswers.Commands.CreateQA;
using ContractorBackend.Application.QuestionAnswers.Commands.DeleteQA;
using ContractorBackend.Application.QuestionAnswers.Commands.UpdateQA;
using ContractorBackend.Application.QuestionAnswers.Queries.GetAllQA;
using ContractorBackend.Application.QuestionAnswers.Queries.GetByIdQA;
using ContractorBackend.Application.QuestionAnswers.Queries.OGetAllQA;
using ContractorBackend.Common.Extensions;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Cor.Controllers
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class QuestionAnswersController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :750-10
        /// نمایش پرسش ها 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست سوال جوابها")]
        [ErrorCode("750-10")]
        public async Task<OkApiResult<SearchQueryResponse<QuestionAnswersDto>>> GetAll([FromQuery] GetAllQuestionAnswersQuery query)
        {
            return new OkApiResult<SearchQueryResponse<QuestionAnswersDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :750-20
        /// نمایش پرسش ها 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست سوال جواب برای select")]
        [ErrorCode("750-20")]
        public async Task<OkApiResult<SearchQueryResponse<QuestionAnswersDto>>> OGetAll([FromQuery] OGetAllQuestionAnswersQuery query)
        {
            query.IsAdmin = true;
            return new OkApiResult<SearchQueryResponse<QuestionAnswersDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :750-30
        /// نمایش  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن یک سوال جواب بوسیله آیدی")]
        [ErrorCode("750-30")]

        public async Task<OkApiResult<QuestionAnswersDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<QuestionAnswersDto>(
                await Mediator.Send(new GetByIdQuestionAnswersQuery(id)));
        }

        /// <summary>
        /// UI CODE :750-40
        /// ایجاد 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد")]
        [ErrorCode("750-40")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateQuestionAnswersCommand command)
        {
            command.IsAdminType = true;
            command.QuestionerId = HttpContext.GetUserId();

            await Mediator.Send(command);

            return new OkApiResult<bool>(true);
        }
        /// <summary>
        /// UI CODE :750-50
        /// تغییر وضعیت متداول بودن سوال 
        /// </summary>
        /// <param name="questionAnswerId"></param>
        /// <param name="isPopular"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("تغییر وضعیت متداول بودن سوال")]
        [ErrorCode("750-50")]
        public async Task<OkApiResult<bool>> ChangePopularStatus([FromQuery] Guid questionAnswerId, [FromQuery] bool isPopular)
        {

            return new OkApiResult<bool>(await Mediator.Send(new ChangeQaStatusCommand(questionAnswerId, isPopular)));
        }

        /// <summary>
        /// UI CODE :750-70
        /// ویرایش 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش")]
        [ErrorCode("750-70")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateQuestionAnswersCommand command)
        {
            command.IsFromEmployee = false;
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
