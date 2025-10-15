using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Cpm.Contract.Queries.GetAllContract;
using ContractorBackend.Application.Cpm.Contractors.Queries.GetAllContractor;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Cpm
{
    [Route("api/cli/core/[controller]/[action]")]
    public class ContractsController : ApiControllerBase
    {

        /// <summary>
        /// لیست قرارداد ها
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("لیست قرارداد ها")]
        public async Task<OkApiResult<SearchQueryResponse<PerContractVM>>> GetAll([FromQuery] GetAllContractQuery query)
        {
            return new OkApiResult<SearchQueryResponse<PerContractVM>>(await Mediator.Send(query));
        }


    }
}
