using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Core.ApplicationSettingPage.Commands.CreateApplicationSettingsClient;
using ContractorBackend.Application.Core.ApplicationSettingPage.Queries.CheckUserHasAccessToSettings;
using ContractorBackend.Application.Core.ApplicationSettingPage.Queries.GetApplicationSettingsClient;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ContractorBackend.WebApiClient.Contractors.Share
{
    [Area("Share")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class SettingsController : ApiControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _descriptor;
        public SettingsController(IActionDescriptorCollectionProvider descriptor)
        {
            _descriptor = descriptor;
        }

        /// <summary> 
        /// ایجاد تنظیمات برنامه
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد تنظیمات برنامه")]
        [ErrorCode("154-01")]
        [AllowAnonymous]
        public async Task<OkApiResult<bool>> Create([FromBody] CreateApplicationSettingsClientCommand command)
        {
            var access = new CheckUserHasAccessToSettingsQuery();
            await Mediator.Send(access);

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary> 
        /// مشاهده تنظیمات برنامه
        /// this action is self called by project and access checks in CheckUserHasAccessToSettingsQuery
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("مشاهده تنظیمات برنامه")]
        [ErrorCode("154-02")]
        [AllowAnonymous]  // 
        public async Task<OkApiResult<ApplicationSettingDto>> GetApplicationSettings([FromQuery] GetApplicationSettingsClientQuery query)
        {
            var access = new CheckUserHasAccessToSettingsQuery();
            await Mediator.Send(access);

            return new OkApiResult<ApplicationSettingDto>(await Mediator.Send(query));
        }

        /// <summary> 
        /// لیست اکشن ها
        /// this action is self called by project and access checks in CheckUserHasAccessToSettingsQuery
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست اکشن ها")]
        [ErrorCode("154-03")]
        [AllowAnonymous]
        public async Task<OkApiResult<List<ActionPathDto>>> GetApplicationActions()
        {

            var access = new CheckUserHasAccessToSettingsQuery();
            await Mediator.Send(access);

            var lst = new List<ActionPathDto>();
            var ctrlActions = _descriptor.ActionDescriptors.Items.ToList();

            foreach (var action in ctrlActions)
            {
                var descriptor = action as ControllerActionDescriptor;

                var displayName = action.EndpointMetadata.OfType<DisplayNameAttribute>().SingleOrDefault()?.DisplayName ?? descriptor.ActionName;
                var obj = new ActionPathDto
                {
                    Ctrl = descriptor.ControllerName,
                    Action = descriptor.ActionName,
                    DisplayName = displayName,
                    finalPath = descriptor.ControllerName + "/" + descriptor.ActionName
                };
                lst.Add(obj);
            }

            return new OkApiResult<List<ActionPathDto>>(lst);
        }


    }
}
