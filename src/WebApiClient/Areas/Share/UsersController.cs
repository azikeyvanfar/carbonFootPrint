using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Core.Users.Query.GetAllActiveUser;
using ContractorBackend.Application.Core.Users.Query.GetAllActiveUserByScopeId;
using ContractorBackend.Application.Core.Users.Query.GetAllUsers;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Users.Share
{
    [Area("Share")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class UsersController : ApiControllerBase
    {


        /// <summary>
        /// UI CODE :29-01
        /// دریافت تمامی کاربران
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت تمامی کاربران")]
        [ErrorCode("29-01")]
        public async Task<OkApiResult<SearchQueryResponse<UserLovDto>>> GetAll([FromQuery] GetAllUserQuery query)
        {
            return new OkApiResult<SearchQueryResponse<UserLovDto>>(await Mediator.Send(query));
        }
        /// <summary>
        /// UI CODE :29-02
        /// دریافت کاربران فعال
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت کاربران فعال")]
        [ErrorCode("29-02")]
        public async Task<OkApiResult<SearchQueryResponse<UserLovDto>>> GetAllActive([FromQuery] GetAllActiveUserQuery query)
        {
            return new OkApiResult<SearchQueryResponse<UserLovDto>>(await Mediator.Send(query));
        }

        /// <summary>
        /// UI CODE :29-02
        /// دریافت کاربران فعال بر اساس حوزه
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت کاربران فعال بر اساس حوزه")]
        [ErrorCode("29-02")]
        public async Task<OkApiResult<SearchQueryResponse<UserLovDto>>> GetAllActiveByScopeId([FromQuery] GetAllActiveUserByScopeIdQuery query)
        {
            return new OkApiResult<SearchQueryResponse<UserLovDto>>(await Mediator.Send(query));
        }

    }
}
