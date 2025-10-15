using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using Gridify;
using MediatR;

namespace ContractorBackend.Application.Cpm.Contractors.Queries.GetAllContractor
{
    public class GetAllContractorQuery :
         SearchQueryRequest, IRequest<SearchQueryResponse<CpmperEmployeesVM>>
    {
    }

    public class GetAllContractorQueryHandler : IRequestHandler<GetAllContractorQuery,
                  SearchQueryResponse<CpmperEmployeesVM>>
    {
        private readonly IsSuiteClientService _isSuitHttp;
        public GetAllContractorQueryHandler(IsSuiteClientService isSuitHttp)
        {
            _isSuitHttp = isSuitHttp;
        }
        public async Task<SearchQueryResponse<CpmperEmployeesVM>> Handle(GetAllContractorQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new List<QueryParamModel>()
            { };

            var isResult = await _isSuitHttp.GetCpmperEmployeesViwAsync(queryParams);

            var query = isResult.Items.AsQueryable();
            Paging<CpmperEmployeesVM> result = new(isResult.Count, query);
            return new SearchQueryResponse<CpmperEmployeesVM>(request, result);
        }
    }
}
