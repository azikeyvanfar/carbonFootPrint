using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Factors;
using ContractorBackend.Application.Ghg.Settings;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Ghg
{
    /// <summary>
    /// مدیریت ضرایب انتشار - بخش تنظیمات
    /// </summary>
    [Area("Ghg")]
    [Route("api/[area]/[controller]/[action]")]
    [Authorize]
    public class FactorsController : ApiControllerBase
    {
        /// <summary>
        /// لیست ضرایب انتشار
        /// </summary>
        [HttpGet]
        [DisplayName("لیست ضرایب انتشار")]
        [ErrorCode("141-10")]
        public async Task<OkApiResult<SearchQueryResponse<EmissionFactorDto>>> GetAll([FromQuery] GetAllEmissionFactorsQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// افزودن ضریب انتشار
        /// </summary>
        [HttpPost]
        [DisplayName("افزودن ضریب انتشار")]
        [ErrorCode("141-30")]
        public async Task<OkApiResult<EmissionFactorDto>> Create([FromBody] CreateEmissionFactorCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ویرایش ضریب انتشار
        /// </summary>
        [HttpPost]
        [DisplayName("ویرایش ضریب انتشار")]
        [ErrorCode("141-31")]
        public async Task<OkApiResult<EmissionFactorDto>> Update([FromBody] UpdateEmissionFactorCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// حذف ضریب انتشار
        /// </summary>
        [HttpPost]
        [DisplayName("حذف ضریب انتشار")]
        [ErrorCode("141-32")]
        public async Task<OkApiResult<bool>> Delete([FromBody] DeleteEmissionFactorCommand command)
            => new(await Mediator.Send(command));
    }

    /// <summary>
    /// داده‌های مرجع و پارامترهای تنظیمات
    /// </summary>
    [Area("Ghg")]
    [Route("api/[area]/[controller]/[action]")]
    [Authorize]
    public class SettingsController : ApiControllerBase
    {
        /// <summary>
        /// لیست نواحی
        /// </summary>
        [HttpGet]
        [DisplayName("لیست نواحی")]
        [ErrorCode("142-10")]
        public async Task<OkApiResult<System.Collections.Generic.List<GhgAreaDto>>> GetAreas([FromQuery] GetAllAreasQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// لیست مراکز هزینه
        /// </summary>
        [HttpGet]
        [DisplayName("لیست مراکز هزینه")]
        [ErrorCode("142-11")]
        public async Task<OkApiResult<SearchQueryResponse<CostCenterDto>>> GetCostCenters([FromQuery] GetAllCostCentersQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// لیست دسته‌های انتشار
        /// </summary>
        [HttpGet]
        [DisplayName("لیست دسته‌های انتشار")]
        [ErrorCode("142-12")]
        public async Task<OkApiResult<System.Collections.Generic.List<EmissionCategoryDto>>> GetCategories([FromQuery] GetAllEmissionCategoriesQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// لیست سوخت‌ها و LHV
        /// </summary>
        [HttpGet]
        [DisplayName("لیست سوخت‌ها")]
        [ErrorCode("142-13")]
        public async Task<OkApiResult<System.Collections.Generic.List<FuelDto>>> GetFuels([FromQuery] GetAllFuelsQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// لیست ضرایب GWP
        /// </summary>
        [HttpGet]
        [DisplayName("لیست ضرایب GWP")]
        [ErrorCode("142-14")]
        public async Task<OkApiResult<System.Collections.Generic.List<GlobalWarmingPotentialDto>>> GetGwps([FromQuery] GetAllGwpsQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// لیست پارامترهای محاسبات
        /// </summary>
        [HttpGet]
        [DisplayName("لیست پارامترهای محاسبات")]
        [ErrorCode("142-15")]
        public async Task<OkApiResult<SearchQueryResponse<GhgParameterDto>>> GetParameters([FromQuery] GetAllParametersQuery query)
            => new(await Mediator.Send(query));

        /// <summary>
        /// افزودن پارامتر
        /// </summary>
        [HttpPost]
        [DisplayName("افزودن پارامتر")]
        [ErrorCode("142-30")]
        public async Task<OkApiResult<GhgParameterDto>> CreateParameter([FromBody] CreateParameterCommand command)
            => new(await Mediator.Send(command));

        /// <summary>
        /// ویرایش پارامتر
        /// </summary>
        [HttpPost]
        [DisplayName("ویرایش پارامتر")]
        [ErrorCode("142-31")]
        public async Task<OkApiResult<GhgParameterDto>> UpdateParameter([FromBody] UpdateParameterCommand command)
            => new(await Mediator.Send(command));
    }
}
