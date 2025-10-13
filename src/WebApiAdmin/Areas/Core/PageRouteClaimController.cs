using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.PageRouteClaims.Commands.CreatePageRouteClaim;
using ContractorBackend.Application.PageRouteClaims.Commands.DeleteClaimMenuItemSByMenuItemId;
using ContractorBackend.Application.PageRouteClaims.Commands.DeletePageRouteClaim;
using ContractorBackend.Application.PageRouteClaims.Commands.UpdatePageRouteClaim;
using ContractorBackend.Application.PageRouteClaims.Queries.GetAllPageRouteClaims;
using ContractorBackend.Application.PageRouteClaims.Queries.GetByIdPageRouteClaimsQuery;
using ContractorBackend.Application.PageRouteClaims.Queries.GetPageRouteClaimByPageRouteId;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class PageRouteClaimController : ApiControllerBase
    {
        public PageRouteClaimController()
        {

        }

        /// <summary>
        /// UI CODE :130-01
        /// اضافه کردن claim های یک menu
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("اضافه کردن claim های یک menu")]
        [ErrorCode("130-01")]
        public async Task<OkApiResult<bool>> UpsertPageRouteClaim([FromBody] CreatePageRouteClaimCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :130-02
        /// دریافت لیست تمامی claimهای مجود در pageRouteها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست تمامی claimهای مجود در pageRouteها")]
        [ErrorCode("130-02")]
        public async Task<OkApiResult<SearchQueryResponse<PageRouteWithClaimDto>>> GetAll([FromQuery] GetAllPageRouteClaimsQuery query)
        {
            return new OkApiResult<SearchQueryResponse<PageRouteWithClaimDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :130-02
        /// دریافت لیست تمامی claimهای مجود در pageRouteها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست تمامی claimهای مجود در pageRouteها")]
        [ErrorCode("130-02")]
        public async Task<OkApiResult<PageRouteWithClaimDto>> GetById([FromQuery] GetByIdPageRouteClaimsQuery query)
        {
            return new OkApiResult<PageRouteWithClaimDto>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :130-03
        /// دریافت  منوها و claim های زیر مجموعه براساس MenuId
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن بوسیله شناسه")]
        [ErrorCode("130-03")]
        public async Task<OkApiResult<SearchQueryResponse<PageRouteClaimDto>>> GetByPageRouteId([FromQuery] GetPageRouteClaimByPageRouteIdQuery command)
        {
            return new OkApiResult<SearchQueryResponse<PageRouteClaimDto>>(await Mediator.Send(command));
        }

        /// <summary>
        /// UI CODE :130-04
        /// حذف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف")]
        [ErrorCode("130-04")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeletePageRouteClaimCommand(id));

            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :130-05
        /// اپدیت
        /// </summary>
        /// <param name ="command" ></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ویرایش")]
        [ErrorCode("130-05")]
        public async Task<ActionResult<OkApiResult<bool>>> Update([FromForm] UpdatePageRouteClaimCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :130-06
        /// گروهی بر اساس MenuItemIds حذف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف بوسیله pageRouteId")]
        [ErrorCode("130-06")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteByPageRouteId([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteByPageRouteIdCommand(id));

            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// دریافت لیست تمامی claimهای مجود در pageRouteها برای یوزر جاری
        /// UI CODE :130-07
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست تمامی claimهای موجود در pageRouteها برای یوزر جاری")]
        [ErrorCode("130-07")]
        public async Task<OkApiResult<IEnumerable<PageRouteWithClaimDto>>> GetAllCurrentUser([FromQuery] GetAllPageRouteClaimsCurrentUserQuery query)
        {
            query.RoleType = RoleType.Manager;
            return new OkApiResult<IEnumerable<PageRouteWithClaimDto>>(await Mediator.Send(query));
        }

    }
}
