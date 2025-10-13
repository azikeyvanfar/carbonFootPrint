using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core.Controllers
{
    [Route("api/cli/[controller]")]

    public class HealthCheckController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public HealthCheckController(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }

        /// <summary>
        /// نمایش فایل و پوشه بر اساس شناسه  
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [AllowAnonymous]
        public async Task<string> GetById()
        {
            return "Hello";
        }

    }
}