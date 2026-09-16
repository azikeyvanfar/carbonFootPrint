using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.ActivityData;
using ContractorBackend.Application.Ghg.Calculation;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Ghg
{
    /// <summary>
    /// دوره‌های گزارش‌دهی و محاسبه موجودی انتشار
    /// </summary>
    [Area("Ghg")]
    [Route("api/[area]/[controller]/[action]")]
    [Authorize]
    public class PeriodsController : ApiControllerBase
    {
        /// <summary>
        /// لیست دوره‌های گزارش‌دهی
        /// </summary>
        [HttpGet]
        [DisplayName("لیست دوره‌های گزارش‌دهی")]
        [ErrorCode("143-10")]
        public async Task<OkApiResult<System.Collections.Generic.List<GhgPeriodDto>>> GetAll([FromQuery] GetPeriodsQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// ایجاد دوره گزارش‌دهی جدید
        /// </summary>
        [HttpPost]
        [DisplayName("ایجاد دوره گزارش‌دهی")]
        [ErrorCode("143-30")]
        public async Task<OkApiResult<GhgPeriodDto>> Create([FromBody] CreatePeriodCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// خلاصه نتایج محاسبه دوره (جداول و نمودارها)
        /// </summary>
        [HttpGet]
        [DisplayName("خلاصه نتایج دوره")]
        [ErrorCode("143-11")]
        public async Task<OkApiResult<GhgCalculationSummaryDto>> GetSummary([FromQuery] GetPeriodSummaryQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// محاسبه مجدد موجودی انتشار دوره
        /// </summary>
        [HttpPost]
        [DisplayName("محاسبه مجدد دوره")]
        [ErrorCode("143-31")]
        public async Task<OkApiResult<GhgCalculationSummaryDto>> Recalculate([FromBody] RecalculatePeriodCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ورود داده‌های اولیه از منبع داده (فایل‌های مرجع / آینده: سرویس وب)
        /// </summary>
        [HttpPost]
        [DisplayName("ورود داده‌های اولیه دوره")]
        [ErrorCode("143-32")]
        public async Task<OkApiResult<int>> ImportActivityData([FromBody] ImportActivityDataCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ورود ردپای کربن محصولات (بخش 2 - ISO 14067)
        /// </summary>
        [HttpPost]
        [DisplayName("ورود ردپای کربن محصولات")]
        [ErrorCode("143-33")]
        public async Task<OkApiResult<int>> ImportProductFootprints([FromBody] ImportProductFootprintsCommand command)
            => new(await Mediator.Send(command));
    }

    /// <summary>
    /// داده‌های فعالیت و فرم گام‌به‌گام ورود داده
    /// </summary>
    [Area("Ghg")]
    [Route("api/[area]/[controller]/[action]")]
    [Authorize]
    public class ActivityDataController : ApiControllerBase
    {
        /// <summary>
        /// لیست داده‌های فعالیت دوره
        /// </summary>
        [HttpGet]
        [DisplayName("لیست داده‌های فعالیت")]
        [ErrorCode("144-10")]
        public async Task<OkApiResult<SearchQueryResponse<ActivityDataEntryDto>>> GetAll([FromQuery] GetActivityDataQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// ساختار فرم گام‌به‌گام ورود داده
        /// </summary>
        [HttpGet]
        [DisplayName("فرم گام‌به‌گام ورود داده")]
        [ErrorCode("144-11")]
        public async Task<OkApiResult<ActivityWizardDto>> GetWizard([FromQuery] GetActivityWizardQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// افزودن ردیف داده فعالیت
        /// </summary>
        [HttpPost]
        [DisplayName("افزودن داده فعالیت")]
        [ErrorCode("144-30")]
        public async Task<OkApiResult<ActivityDataEntryDto>> Create([FromBody] CreateActivityDataCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ویرایش ردیف داده فعالیت
        /// </summary>
        [HttpPost]
        [DisplayName("ویرایش داده فعالیت")]
        [ErrorCode("144-31")]
        public async Task<OkApiResult<bool>> Update([FromBody] UpdateActivityDataCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// حذف ردیف داده فعالیت
        /// </summary>
        [HttpPost]
        [DisplayName("حذف داده فعالیت")]
        [ErrorCode("144-32")]
        public async Task<OkApiResult<bool>> Delete([FromBody] DeleteActivityDataCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// پیش‌نمایش محاسبه ردیف (بدون ذخیره)
        /// </summary>
        [HttpPost]
        [DisplayName("پیش‌نمایش محاسبه")]
        [ErrorCode("144-33")]
        public async Task<OkApiResult<ActivityPreviewDto>> Preview([FromBody] PreviewActivityCommand command)
            => new(await Mediator.Send(command));
    }
}
