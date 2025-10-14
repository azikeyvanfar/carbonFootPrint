using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using Gridify;
using MediatR;

namespace ContractorBackend.Application.Cpm.Contractors.Queries.GetFamilesAllQuery
{
    public class GetFamilesAllQuery :
         SearchQueryRequest, IRequest<SearchQueryResponse<ContractFamiliesVM>>
    {
    }

    public class GetFamilesAllQueryHandler : IRequestHandler<GetFamilesAllQuery,
                  SearchQueryResponse<ContractFamiliesVM>>
    {
        private readonly IMapper _mapper;
        private readonly IsSuiteClientService _isSuitHttp;
        public GetFamilesAllQueryHandler(IMapper mapper, IsSuiteClientService isSuitHttp)
        {
            _mapper = mapper;
            _isSuitHttp = isSuitHttp;
        }
        public async Task<SearchQueryResponse<ContractFamiliesVM>> Handle(GetFamilesAllQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new List<QueryParamModel>()
            { };

            var isResult = await _isSuitHttp.GetContractorFamiliesViwAsync(queryParams);

            var query = isResult.Items.AsQueryable();
            Paging<ContractFamiliesVM> result = new(isResult.Count, query);
            return new SearchQueryResponse<ContractFamiliesVM>(request, result);
        }
    }
}
