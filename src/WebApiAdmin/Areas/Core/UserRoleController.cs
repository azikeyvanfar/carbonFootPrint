using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.UserRole.Commands.CreateUserRole;
using ContractorBackend.Application.Core.UserRole.Commands.DeleteByUserId;
using ContractorBackend.Application.Core.UserRole.Commands.DeleteUserRole;
using ContractorBackend.Application.Core.UserRole.Queries.GetAllUserRoles;
using ContractorBackend.Application.Core.UserRole.Queries.GetAllUserRolesCurrentUser;
using ContractorBackend.Application.Core.UserRole.Queries.GetUserRoleByExternalUrlId;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class UserRoleController : ApiControllerBase
    {
        public UserRoleController()
        {

        }

        /// <summary>
        /// UI CODE :133-01
        /// اضافه کردن claim های یک menu
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("اضافه کردن role های یک کاربر")]
        [ErrorCode("133-01")]
        public async Task<OkApiResult<bool>> UpsertUserRole([FromBody] CreateUserRoleCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :133-02
        /// دریافت لیست تمامی claimهای مجود در Userها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست تمامی role مجود در Userها")]
        [ErrorCode("133-02")]
        public async Task<OkApiResult<SearchQueryResponse<UserWithRoleDto>>> GetAll([FromQuery] GetAllUserRolesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<UserWithRoleDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :133-03
        /// دریافت role کاربر جاری
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن بوسیله شناسه")]
        [ErrorCode("133-03")]
        public async Task<OkApiResult<SearchQueryResponse<UserRoleDto>>> GetByUserId([FromQuery] GetUserRoleByUserIdQuery command)
        {
            return new OkApiResult<SearchQueryResponse<UserRoleDto>>(await Mediator.Send(command));
        }

        /// <summary>
        /// UI CODE :133-04
        /// حذف
        /// </summary>
        /// <param name="RId"></param>
        /// <param name="UId"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف")]
        [ErrorCode("133-04")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] long RId, long UId)
        {
            if (RId == 0 || UId == 0)
                return BadRequest();

            await Mediator.Send(new DeleteUserRoleCommand(RId, UId));

            return new OkApiResult<bool>(true);
        }

        ///// <summary>
        ///// UI CODE :133-05
        ///// اپدیت
        ///// </summary>
        ///// <param name="command"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[DisplayName("ویرایش")]
        //[ErrorCode("133-05")]
        //public async Task<ActionResult<OkApiResult<bool>>> Update([FromBody] UpdateUserRoleCommand command)
        //{
        //    await Mediator.Send(command);
        //    return new OkApiResult<bool>(true);
        //}

        /// <summary>
        /// UI CODE :133-06
        /// گروهی بر اساس MenuItemIds حذف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف بوسیله UserId")]
        [ErrorCode("133-06")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteByUserId([FromQuery] long id)
        {
            if (id == 0)
                return BadRequest();

            await Mediator.Send(new DeleteByUserIdCommand(id));

            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// دریافت لیست تمامی claimهای مجود در Userها برای یوزر جاری
        /// UI CODE :133-07
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت لیست تمامی Roleهای مجود در Userها برای یوزر جاری")]
        [ErrorCode("133-07")]
        public async Task<OkApiResult<SearchQueryResponse<UserWithRoleDto>>> GetAllCurrentUser([FromQuery] GetAllUserRolesCurrentUserQuery query)
        {
            query.RoleType = RoleType.Manager;
            return new OkApiResult<SearchQueryResponse<UserWithRoleDto>>(await Mediator.Send(query));
        }

    }
}
