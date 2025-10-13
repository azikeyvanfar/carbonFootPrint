using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Queries.GetAllItem
{
    /// <summary>
    /// لیست آیتم ها
    /// </summary>
    public class GetAllLookupItemQuery : SearchQueryRequest, IRequest<SearchQueryResponse<LookupItemDto>>
    {
        public string Code { get; set; }
        public Guid? ParentId { get; set; }
    }

    public class GetAllItemQueryHandler : IRequestHandler<GetAllLookupItemQuery, SearchQueryResponse<LookupItemDto>>
    {
        private readonly ILookupRepository _services;
        public GetAllItemQueryHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<SearchQueryResponse<LookupItemDto>> Handle(GetAllLookupItemQuery request, CancellationToken cancellationToken)
        {
            var query = _services.GetAllItemQuery;
            if (!string.IsNullOrEmpty(request.Code))
            {
                query = query.Where(x => x.Code == request.Code);
            }
            if (request.ParentId != null)
            {
                query = query.Where(x => x.ParentId == request.ParentId);
            }

            QueryablePaging<LookupItemDto> qp = await query.OrderBy(x => x.Priority).GridifyQueryableAsync(request, null, cancellationToken);
            Paging<LookupItemDto> pq = new(qp.Count, qp.Query);
            return new SearchQueryResponse<LookupItemDto>(request, pq);
        }
    }
}
