using ContractorBackend.WebApiAdmin.Controllers;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiAdmin.Areas.Share
{
    [Area("Share")]
    [Route("api/[area]/[controller]/[action]")]
    public class BusinessUnitsController : ApiControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJob;
        public BusinessUnitsController(IBackgroundJobClient backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }

        ///// <summary>
        ///// UI CODE :15-01
        ///// دریافت تمامی مکان ها
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[DisplayName("دریافت تمامی مکان ها")]
        //[ErrorCode("15-01")]
        //public async Task<OkApiResult<SearchQueryResponse<BusinessUnitDto>>> GetAll([FromQuery] GetAllBusinessUnitQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<BusinessUnitDto>>(await Mediator.Send(query));
        //}





    }
}
