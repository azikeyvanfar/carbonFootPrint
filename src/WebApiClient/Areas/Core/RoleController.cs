using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Documents.Queries.GetAllRootDocument;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.GeneralClaims.Queries.GetAllGeneralClaims;
using ContractorBackend.Application.Role.Queries.GetAllRoles;
using ContractorBackend.Application.Role.Queries.GetAllRoleTypes;
using ContractorBackend.Application.Role.Queries.GetListRoles;
using ContractorBackend.Application.Role.Queries.GetRoleById;
using ContractorBackend.Application.UserRolePageRouteAccesses.Queries.GetAllRolePageRouteAccess;
using ContractorBackend.Application.UserRolePageRouteAccesses.Queries.GetRolePageRouteAccessById;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core
{

    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
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
            return new OkApiResult<RoleAccessDto>(await Mediator.Send(new GetRolePageRouteAccessByIdQuery(roleId)));
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
