using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Cpm.Contractors.Queries.GetAllContractor;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Cpm
{
    [Area("Share")]
    [Route("api/[area]/[controller]/[action]")]
    public class ContractorsController : ApiControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJob;
        public ContractorsController(IBackgroundJobClient backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }


        /// <summary>
        /// لیست پیمانکار ها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست پیمانکار ها")]
        public async Task<OkApiResult<SearchQueryResponse<CpmperEmployeesVM>>> GetAll([FromQuery] GetAllContractorQuery query)
        {
            return new OkApiResult<SearchQueryResponse<CpmperEmployeesVM>>(await Mediator.Send(query));
        }


        ///// <summary>
        ///// UI CODE :35-02
        ///// سینک ناحیه ها
        ///// </summary> 
        ///// <returns></returns>
        //[HttpGet]
        //[DisplayName("سینک ناحیه ها")]
        //[ErrorCode("35-02")]
        //public OkApiResult<bool> SyncContractor()
        //{
        //    _backgroundJob.Schedule<IHangFireSyncService<Contractor>>((x) => x.SyncDataContractor(), TimeSpan.FromSeconds(2));

        //    return new OkApiResult<bool>(true);
        //}


    }
}
