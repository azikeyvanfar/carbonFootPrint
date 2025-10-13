using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.Documents.Queries.GetAllRootDocument;
using ContractorBackend.Application.Core.GeneralClaims.Queries.GetAllGeneralClaims;
using ContractorBackend.Application.Core.Role.Commands.CreateRole;
using ContractorBackend.Application.Core.Role.Commands.DeleteRole;
using ContractorBackend.Application.Core.Role.Commands.UpdateRole;
using ContractorBackend.Application.Core.Role.Queries.GetAllRoles;
using ContractorBackend.Application.Core.Role.Queries.GetAllRoleTypes;
using ContractorBackend.Application.Core.Role.Queries.GetListRoles;
using ContractorBackend.Application.Core.Role.Queries.GetRoleById;
using ContractorBackend.Application.Core.UserRolePageRouteAccess.Commands.DeleteRoleAccessByRoleId;
using ContractorBackend.Application.Core.UserRolePageRouteAccess.Commands.RolePageRouteCreate;
using ContractorBackend.Application.Core.UserRolePageRouteAccess.Queries.GetAllRolePageRouteAccess;
using ContractorBackend.Application.Core.UserRolePageRouteAccess.Queries.GetRolePageRouteAccessById;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{

    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class RoleController : ApiControllerBase
    {

        /// <summary>
        /// UI CODE :120-01
        /// واکشی تمام نقش ها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("واکشی تمام نقش ها")]
        [ErrorCode("120-01")]
        public async Task<OkApiResult<SearchQueryResponse<RoleDto>>> GetAll([FromQuery] GetAllRolesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<RoleDto>>(await Mediator.Send(query));

        }

        /// <summary>
        /// UI CODE :120-02
        ///واکشی یک نقش یوسیله id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("واکشی یک نقش یوسیله id")]
        [ErrorCode("120-02")]
        public async Task<OkApiResult<RoleDto>> GetById([FromQuery] long id)
        {
            return new OkApiResult<RoleDto>(await Mediator.Send(new GetRoleByIdQuery(id)));
        }

        /// <summary>
        /// UI CODE :120-03
        /// لیست تمام اکشن های سمت سرور
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست تمام اکشن های سمت سرور")]
        [ErrorCode("120-03")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> GetActionList([FromQuery] GetAllGeneralClaimsQuery query)
        {
            var model = await Mediator.Send(query);
            return new OkApiResult<SearchQueryResponse<SelectModel>>(model);
        }



        /// <summary>
        /// UI CODE :120-04
        /// تولید یک نقش جدید
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("تولید یک نقش جدید")]
        [ErrorCode("120-04")]
        public async Task<OkApiResult<bool>> Create([FromForm] CreateRoleCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :120-05
        /// ویرایش یک نقش
        /// این متد استفاده نشود چون همه ی دسترسی ها آن پاک می شود
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Obsolete("این متد استفاده نشود چون همه ی دسترسی ها آن پاک می شود")]
        [HttpPost]
        [DisplayName("ویرایش یک نقش")]
        [ErrorCode("120-05")]
        public async Task<OkApiResult<bool>> Update([FromForm] UpdateRoleCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :120-06
        /// حذف یک نقش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف یک نقش")]
        [ErrorCode("120-06")]
        public async Task<ActionResult<OkApiResult<bool>>> Delete([FromQuery] long id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            await Mediator.Send(new DeleteRoleCommand(id));
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :120-07
        /// ایجاد دسترسی صفحات کلاینت برای یک نقش
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد دسترسی صفحات کلاینت برای یک نقش")]
        [ErrorCode("120-07")]
        public async Task<OkApiResult<bool>> RolePageRouteAccessCreate([FromBody] RolePageRouteAccessCreateCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :120-09
        /// حذف دستزسی ها بوسیله شناسه نقش
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("حذف دسترسی ها بوسیله شناسه نقش")]
        [ErrorCode("120-09")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteRoleAccessByRoleId([FromQuery] long id)
        {
            if (id == 0)
                return BadRequest();

            await Mediator.Send(new DeleteRoleAccessByRoleIdCommand(id));
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UI CODE :120-08
        /// دسترسی های کلاینت برای یک نفش بوسیله شناسه نقش
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دسترسی های کلاینت برای یک نفش بوسیله شناسه نقش")]
        [ErrorCode("120-08")]
        public async Task<OkApiResult<RoleAccessDto>> GetRoleAccessByRoleId([FromQuery] long roleId)
        {
            var res = new GetRolePageRouteAccessByIdQuery(roleId);
            return new OkApiResult<RoleAccessDto>(await Mediator.Send(res));
        }


        /// <summary>
        /// UI CODE :120-10
        /// فولدر های روت 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت فولدرهای روت")]
        [ErrorCode("120-10")]
        public async Task<OkApiResult<IEnumerable<DocumentDto>>> GetRootDocumentList()
        {
            var lst = await Mediator.Send(new GetAllRootDocumentQuery());
            return new OkApiResult<IEnumerable<DocumentDto>>(lst);
        }

        /// <summary>
        /// UI CODE :120-11
        ///دریافت تمامی دسترسی نقش
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت تمامی دسترسی نقش")]
        [ErrorCode("120-11")]
        public async Task<OkApiResult<SearchQueryResponse<RoleAccessDto>>> GetAllRoleAccess([FromQuery] GetAllRolePageRouteAccessQuery query)
        {
            return new OkApiResult<SearchQueryResponse<RoleAccessDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :120-12
        /// لیست انواع نقش ها    
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName("لیست انواع نقش ها")]
        [ErrorCode("120-12")]
        public async Task<OkApiResult<List<SelectModel>>> GetAllRoleTypes()
        {
            //query.PageSize = 10;
            return new OkApiResult<List<SelectModel>>(
                await Mediator.Send(new GetAllRoleTypesQuery()));
        }


        /// <summary>
        /// UI CODE :120-13
        /// واکشی لیست نقش ها برای سلکت
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("واکشی لیست نقش ها برای سلکت")]
        [ErrorCode("120-13")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> GetList([FromQuery] GetListRolesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<SelectModel>>(await Mediator.Send(query));

        }
        [HttpGet]
        [DisplayName("واکشی لیست نقش ها برای سلکت با شرط نوع نقش")]
        [ErrorCode("120-14")]
        public async Task<OkApiResult<SearchQueryResponse<SelectModel>>> GetListByRoleType([FromQuery] GetListRolesQuery query)
        {
            return new OkApiResult<SearchQueryResponse<SelectModel>>(await Mediator.Send(query));
        }
    }
}
