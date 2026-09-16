using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Formulas;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Ghg
{
    /// <summary>
    /// مدیریت فرمول‌های محاسبه انتشار - بخش تنظیمات
    /// </summary>
    [Area("Ghg")]
    [Route("api/[area]/[controller]/[action]")]
    [Authorize]
    public class FormulasController : ApiControllerBase
    {
        /// <summary>
        /// لیست فرمول‌ها
        /// </summary>
        [HttpGet]
        [DisplayName("لیست فرمول‌های محاسبه")]
        [ErrorCode("140-10")]
        public async Task<OkApiResult<SearchQueryResponse<CalculationFormulaDto>>> GetAll([FromQuery] GetAllFormulasQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// دریافت فرمول بر اساس شناسه
        /// </summary>
        [HttpGet]
        [DisplayName("دریافت فرمول")]
        [ErrorCode("140-11")]
        public async Task<OkApiResult<CalculationFormulaDto>> GetById([FromQuery] GetFormulaByIdQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// افزودن فرمول محاسبه جدید
        /// </summary>
        [HttpPost]
        [DisplayName("افزودن فرمول محاسبه")]
        [ErrorCode("140-30")]
        public async Task<OkApiResult<CalculationFormulaDto>> Create([FromBody] CreateFormulaCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ویرایش فرمول محاسبه
        /// </summary>
        [HttpPost]
        [DisplayName("ویرایش فرمول محاسبه")]
        [ErrorCode("140-31")]
        public async Task<OkApiResult<CalculationFormulaDto>> Update([FromBody] UpdateFormulaCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// حذف فرمول محاسبه
        /// </summary>
        [HttpPost]
        [DisplayName("حذف فرمول محاسبه")]
        [ErrorCode("140-32")]
        public async Task<OkApiResult<bool>> Delete([FromBody] DeleteFormulaCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// آزمایش فرمول با مقادیر نمونه
        /// </summary>
        [HttpPost]
        [DisplayName("آزمایش فرمول")]
        [ErrorCode("140-33")]
        public async Task<OkApiResult<TestFormulaResultDto>> Test([FromBody] TestFormulaCommand command)
            => new(await Mediator.Send(command));
    }
}
