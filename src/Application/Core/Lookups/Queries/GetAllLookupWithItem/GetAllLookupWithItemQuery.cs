using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Queries.GetAll
{
    /// <summary>
    /// جست و جو کل آیتم ها
    /// </summary>
    public class GetAllLookupWithItemQuery : SearchQueryRequest, IRequest<SearchQueryResponse<LookupWithItemDto>>
    {
    }

    public class GetAllQueryHandler : IRequestHandler<GetAllLookupWithItemQuery, SearchQueryResponse<LookupWithItemDto>>
    {
        private readonly ILookupRepository _services;

        public GetAllQueryHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<SearchQueryResponse<LookupWithItemDto>> Handle(GetAllLookupWithItemQuery request, CancellationToken cancellationToken)
        {
            var query = await _services.GetAllListWithItem();
            QueryablePaging<LookupWithItemDto> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<LookupWithItemDto> pq = new(qp.Count, qp.Query);
            return new SearchQueryResponse<LookupWithItemDto>(request, pq);
        }
    }
}
