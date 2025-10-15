using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.Application.Common.Task;

namespace ContractorBackend.WebApiClient.Areas.Job
{
    [Route("api/cli/core/[controller]/[action]")]
    public class JobController: ApiControllerBase
    {
        private readonly SyncUserJob _userService = null;

        public JobController(SyncUserJob userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// اجرای بروز رسانی کاربران
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RunJob()
        {
            var result = await _userService.SyncUsersFromDateToNow();
            return Ok(new { MofifiedUsersCount = result.ModifiedUsersCount , AddedUsersCount = result.AddedUsersCount});
        }

    }
}
