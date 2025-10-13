using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Core.ApplicationSettingPage.Queries.GetApplicationActionsClient;
using ContractorBackend.Application.Core.ApplicationSettingPage.Commands.CreateApplicationSettings;
using ContractorBackend.Application.Core.ApplicationSettingPage.Queries.GetApplicationSettings;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
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
        public async Task<OkApiResult<bool>> Create([FromBody] CreateApplicationSettingsCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary> 
        /// مشاهده تنظیمات برنامه
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("مشاهده تنظیمات برنامه")]
        [ErrorCode("154-02")]
        public async Task<OkApiResult<ApplicationSettingDto>> GetApplicationSettings([FromQuery] GetApplicationSettingsQuery query)
        {
            return new OkApiResult<ApplicationSettingDto>(await Mediator.Send(query));
        }


        [HttpGet]
        [DisplayName("لیست اکشن ها")]
        [ErrorCode("154-03")]
        public async Task<OkApiResult<List<ActionPathDto>>> GetApplicationActions(bool isAdmin = false)
        {
            if (isAdmin == true)
            {

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
            else
            {
                var query = new GetApplicationActionsClientQuery();
                return new OkApiResult<List<ActionPathDto>>(await Mediator.Send(query));

            }
        }

    }
}
