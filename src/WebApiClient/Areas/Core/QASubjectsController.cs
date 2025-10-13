using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.QASubjects.Queries.GetByIdQASubject;
using ContractorBackend.Application.QASubjects.Queries.OGetAllQASubject;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class QASubjectsController : ApiControllerBase
    {


        /// <summary>
        /// UI CODE :740-20
        /// دریافت موضوع سوال جواب با شناسه  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت موضوع سوال جواب با شناسه")]
        [ErrorCode("740-20")]
        public async Task<OkApiResult<QASubjectDto>> GetById([FromQuery] Guid id)
        {
            return new OkApiResult<QASubjectDto>(
                await Mediator.Send(new GetByIdQASubjectQuery(id)));
        }


        /// <summary>
        /// UI CODE :740-50
        /// لیست  برای drps 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست موضوعات سوال جواب برای select")]
        [ErrorCode("740-50")]

        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> OGetAll([FromQuery] OGetAllQASubjectQuery query)
        {
            query.IsAdmin = false;
            var model = await Mediator.Send(query);
            return new OkApiResult<SearchQueryResponse<SelectModel>>(
                model);
        }


    }
}
