using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.LoVTypeValues.Queries.GetAllList
{
    /// <summary>
    /// لیست ها
    /// </summary>
    public class GetAllLookupQuery : SearchQueryRequest, IRequest<SearchQueryResponse<LookupDto>>
    {
    }

    public class GetAllListQueryHandler : IRequestHandler<GetAllLookupQuery, SearchQueryResponse<LookupDto>>
    {
        private readonly ILookupRepository _service;

        public GetAllListQueryHandler(ILookupRepository service)
        {
            _service = service;
        }

        public async Task<SearchQueryResponse<LookupDto>> Handle(GetAllLookupQuery request, CancellationToken cancellationToken)
        {
            var query = _service.GetAllListQuery;
            QueryablePaging<LookupDto> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<LookupDto> pq = new(qp.Count, qp.Query);
            return new SearchQueryResponse<LookupDto>(request, pq);
        }
    }
}
