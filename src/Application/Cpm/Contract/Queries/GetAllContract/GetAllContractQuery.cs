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

namespace ContractorBackend.Application.Cpm.Contract.Queries.GetAllContract
{
    public class GetAllContractQuery :
         SearchQueryRequest, IRequest<SearchQueryResponse<PerContractVM>>
    {
    }

    public class GetAllContractQueryHandler : IRequestHandler<GetAllContractQuery,
                  SearchQueryResponse<PerContractVM>>
    {
        private readonly IsSuiteClientService _isSuitHttp;
        public GetAllContractQueryHandler(IsSuiteClientService isSuitHttp)
        {
            _isSuitHttp = isSuitHttp;
        }
        public async Task<SearchQueryResponse<PerContractVM>> Handle(GetAllContractQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new List<QueryParamModel>(){ };

            var isResult = await _isSuitHttp.GetPerContractInfoViwAsync(queryParams);

            var query = isResult.Items.AsQueryable();
            Paging<PerContractVM> result = new(isResult.Count, query);
            return new SearchQueryResponse<PerContractVM>(request, result);
        }
    }
}
