using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.PageRoute.Commands.CreatePageRoute;
using ContractorBackend.Application.Core.PageRoute.Commands.UpdatePageRoute;
using ContractorBackend.Application.Core.PageRoute.Commands.UpdatePageRouteForOtp;
using ContractorBackend.Application.Core.PageRoute.Queries.GetAll;
using ContractorBackend.Application.Core.PageRoute.Queries.GetAllWithOutClaim;
using ContractorBackend.Application.Core.PageRoute.Queries.GetByIdPageRoutes;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Core.PageRoute.Commands.DeletePageRoute;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class PageRouteController : ApiControllerBase
    {
        /// <summary>
        /// UI CODE :107-01
        /// دریافت تمامی pageRouteها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت تمامی pageRouteها")]
        [ErrorCode("107-01")]
        public async Task<OkApiResult<SearchQueryResponse<PageRouteDto>>> GetAll([FromQuery] GetAllPageRoutesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<PageRouteDto>>(await Mediator.Send(query));
        }
        /// <summary>
        /// UI CODE :107-01
        /// گرفتن pageRoute بوسیله آیدی  
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن pageRoute بوسیله آیدی")]
        [ErrorCode("107-01")]
        public async Task<OkApiResult<PageRouteDto>> GetById([FromQuery] GetByIdPageRoutesQuery query)
        {
            return new OkApiResult<PageRouteDto>(await Mediator.Send(query));
        }


        /// <summary>
        /// UI CODE :107-06
        /// دریافت تمامی pageRouteها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست pageRouteها بدون claim")]
        [ErrorCode("107-06")]
        public async Task<OkApiResult<SearchQueryResponse<PageRouteDto>>> GetAllWithOutClaim([FromQuery] GetAllPageRoutesWithOutClaimQuery query)
        {
            return new OkApiResult<SearchQueryResponse<PageRouteDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :107-02
        /// ایجاد pageRoute جدید
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد pageRoute جدید")]
        [ErrorCode("107-02")]

        public async Task<OkApiResult<bool>> Create([FromForm] CreatePageRouteCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :107-04
        /// ویرایش pageroute
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش pageroute")]
        [ErrorCode("107-04")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdatePageRouteCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }



        /// <summary>
        /// UI CODE :107-03
        /// حذف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف")]
        [ErrorCode("107-03")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            await Mediator.Send(new DeletePageRouteCommand(id));

            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :107-05
        /// ویرایش pagerouteForOTP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش pagerouteForOTP")]
        [ErrorCode("107-05")]
        public async Task<OkApiResult<bool>> UpdateForOtp([FromForm] UpdatePageRouteForOtpCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }
    }
}
