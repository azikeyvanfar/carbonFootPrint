using System.ComponentModel;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Contractors.Share
{
    [Area("Share")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class ContractorsController : ApiControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJob;
        public ContractorsController(IBackgroundJobClient backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }


        ///// <summary>
        ///// UI CODE :35-05
        ///// دریافت تمامی پیمانکار ها
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[DisplayName("دریافت تمامی پیمانکار ها")]
        //[ErrorCode("35-05")]
        //public async Task<OkApiResult<SearchQueryResponse<ContractorDto>>> GetAll([FromQuery] GetAllContractorQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<ContractorDto>>(await Mediator.Send(query));
        //}


        ///// <summary>
        ///// UI CODE :35-05
        ///// دریافت تمامی پیمانکار های شرکت کاربر جاری
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[DisplayName("دریافت تمامی پیمانکار های شرکت کاربر جاری")]
        //[ErrorCode("35-05")]
        //public async Task<OkApiResult<SearchQueryResponse<ContractorDto>>> GetAllByCurrentUser([FromQuery] GetAllByCurrentUserContractorQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<ContractorDto>>(await Mediator.Send(query));
        //}


        /// <summary>
        /// UI CODE :35-02
        /// سینک پیمانکار ها
        /// </summary> 
        /// <returns></returns>
        [HttpGet]
        [DisplayName("سینک پیمانکار ها")]
        [ErrorCode("35-02")]
        public OkApiResult<bool> SyncContractor()
        {
            // _backgroundJob.Schedule<IHangFireSyncService<Contractor>>((x) => x.SyncDataContractor(), TimeSpan.FromSeconds(2));

            return new OkApiResult<bool>(true);
        }




    }
}
