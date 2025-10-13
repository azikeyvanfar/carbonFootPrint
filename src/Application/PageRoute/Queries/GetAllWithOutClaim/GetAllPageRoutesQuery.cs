using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.PageRoutes.Queries.GetAllWithOutClaim
{
    public class GetAllPageRoutesWithOutClaimQuery : SearchQueryRequest, IRequest<SearchQueryResponse<PageRouteDto>>
    {
    }

    public class GetAllPageRoutesWithOutClaimQueryHandler : IRequestHandler<GetAllPageRoutesWithOutClaimQuery, SearchQueryResponse<PageRouteDto>>
    {
        private readonly IRepository<PageRoute> _repository;
        private readonly IMapper _mapper;

        public GetAllPageRoutesWithOutClaimQueryHandler(IRepository<PageRoute> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<PageRouteDto>> Handle(GetAllPageRoutesWithOutClaimQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking();
            QueryablePaging<PageRoute> qp1 = await query.GridifyQueryableAsync(request, null, cancellationToken);
            var qp = _mapper.Map<List<PageRouteDto>>(qp1.Query);
            Paging<PageRouteDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<PageRouteDto>(request, result);
        }
    }
}