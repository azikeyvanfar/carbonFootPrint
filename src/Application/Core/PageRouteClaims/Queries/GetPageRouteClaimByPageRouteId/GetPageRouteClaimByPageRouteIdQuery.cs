using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.PageRouteClaims.Queries.GetPageRouteClaimByPageRouteId
{
    public class GetPageRouteClaimByPageRouteIdQuery : SearchQueryRequest, IRequest<SearchQueryResponse<PageRouteClaimDto>>
    {
        public Guid PageRouteId { get; set; }
    }
    public class GetPageRouteClaimByIdQueryHandler : IRequestHandler<GetPageRouteClaimByPageRouteIdQuery, SearchQueryResponse<PageRouteClaimDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetPageRouteClaimByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<SearchQueryResponse<PageRouteClaimDto>> Handle(GetPageRouteClaimByPageRouteIdQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PageRouteClaims.Where(c => c.PageRouteId == request.PageRouteId);

            QueryablePaging<PageRouteClaim> qp1 = await query.GridifyQueryableAsync(request, null, cancellationToken);
            var qp = _mapper.Map<List<PageRouteClaimDto>>(qp1.Query);

            Paging<PageRouteClaimDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<PageRouteClaimDto>(request, result);


        }
    }
}
