using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.QASubjects.Commands.CreateQASubject;
using ContractorBackend.Application.QASubjects.Commands.DeleteQASubject;
using ContractorBackend.Application.QASubjects.Commands.UpdateQASubject;
using ContractorBackend.Application.QASubjects.Queries.GetAllQASubject;
using ContractorBackend.Application.QASubjects.Queries.GetByIdQASubject;
using ContractorBackend.Application.QASubjects.Queries.OGetAllQASubject;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Share
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    public class QASubjectsController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :740-10
        /// نمایش 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [DisplayName("لیست موضوعات سوال جواب")]
        [ErrorCode("740-10")]
        public async Task<OkApiResult<SearchQueryResponse<QASubjectDto>>> GetAll([FromQuery] GetAllQASubjectQuery query)
        {
            //query.PageSize = 10;
            return new OkApiResult<SearchQueryResponse<QASubjectDto>>(
                await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :740-20
        /// نمایش  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("نمایش یک موضوع سوال جواب بوسیله آیدی")]
        [ErrorCode("740-20")]
        public async Task<OkApiResult<QASubjectDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<QASubjectDto>(
                await Mediator.Send(new GetByIdQASubjectQuery(id)));
        }

        /// <summary>
        /// UI CODE :740-30
        /// ایجاد 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد")]
        [ErrorCode("740-30")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateQASubjectCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :740-40
        /// ویرایش 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش")]
        [ErrorCode("740-40")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateQASubjectCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :740-50
        /// لیست  برای drps 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست موضوعات سوال جواب برای سلکت")]
        [ErrorCode("740-50")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> OGetAll([FromQuery] OGetAllQASubjectQuery query)
        {
            query.IsAdmin = true;
            var model = await Mediator.Send(query);
            return new OkApiResult<SearchQueryResponse<SelectModel>>(
                model);
        }

        /// <summary>
        /// UI CODE :740-60
        /// حذف 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف")]
        [ErrorCode("740-60")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteQASubjectCommand(id));
            return new OkApiResult<bool>(true);
        }
    }
}
