using System.Collections.Generic;
using System.Linq;
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
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.PageRoutes.Queries.GetAll
{
    public class GetAllPageRoutesQuery : SearchQueryRequest, IRequest<SearchQueryResponse<PageRouteDto>>
    {
    }

    public class GetAllPageRoutesQueryHandler : IRequestHandler<GetAllPageRoutesQuery, SearchQueryResponse<PageRouteDto>>
    {
        private readonly IRepository<PageRoute> _repository;
        private readonly IRepository<PageRouteClaim> _pgRouteClaimRepository;
        private readonly IMapper _mapper;

        public GetAllPageRoutesQueryHandler(IRepository<PageRoute> repository, IMapper mapper, IRepository<PageRouteClaim> pgRouteClaimRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _pgRouteClaimRepository = pgRouteClaimRepository;

        }

        public async Task<SearchQueryResponse<PageRouteDto>> Handle(GetAllPageRoutesQuery request, CancellationToken cancellationToken)
        {

            var query = _repository.GetAllAsNoTracking();


            QueryablePaging<PageRoute> qp1 = await
                query.GridifyQueryableAsync<PageRoute>(request, null, cancellationToken);
            var qp = _mapper.Map<List<PageRouteDto>>(qp1.Query);
            foreach (var item in qp)
            {
                var claims = _pgRouteClaimRepository.GetAllAsNoTracking().Where(_ => _.PageRouteId == item.Id).Include(_ => _.GeneralClaim);
                foreach (var claimm in claims)
                {
                    item.Claims.Add(new SelectModel() { Value = claimm.GeneralClaim.Id.ToString(), Text = claimm.GeneralClaim.ClaimValue });
                }
            }

            Paging<PageRouteDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<PageRouteDto>(request, result);
        }
    }
}
