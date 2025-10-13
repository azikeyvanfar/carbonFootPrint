using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.PageRouteClaims.Queries.GetAllPageRouteClaims
{
    public class GetAllPageRouteClaimsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<PageRouteWithClaimDto>>
    {
    }
    public class GetAllPageRouteClaimsQueryHandler : IRequestHandler<GetAllPageRouteClaimsQuery, SearchQueryResponse<PageRouteWithClaimDto>>
    {
        private readonly IRepository<PageRouteClaim> _repository;
        private readonly IMapper _mapper;
        public GetAllPageRouteClaimsQueryHandler(IRepository<PageRouteClaim> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SearchQueryResponse<PageRouteWithClaimDto>> Handle(GetAllPageRouteClaimsQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                .AsNoTracking()
                .Include(c => c.PageRoute)
                .Include(_ => _.GeneralClaim)
                .Where(_ => _.GeneralClaim.IsGlobal != true);

            var lst = query.AsEnumerable()
                .GroupBy(_ => _.PageRouteId)
                .Select(pageRouteClaims => new PageRouteWithClaimDto
                {
                    PageRouteId = pageRouteClaims.Key,
                    Id = pageRouteClaims.Key,
                    Route = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.Route,
                    RouteName = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RouteName,
                    RoleType = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RoleType,
                    RoleTypeId = ((int)pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RoleType).ToString(),

                    Claims = query.Where(_ => _.PageRouteId == pageRouteClaims.Key).Select(claim => new ClaimDto
                    {
                        ClaimName = claim.GeneralClaim.ClaimName,
                        ClaimValue = claim.GeneralClaim.ClaimValue,
                        GeneralClaimId = claim.GeneralClaimsId,
                        ClaimRoleType = claim.GeneralClaim.RoleType,
                        ClaimRoleTypeId = ((int)claim.GeneralClaim.RoleType).ToString(),
                        Id = claim.GeneralClaimsId,
                        PageRouteId = pageRouteClaims.Key
                    })
                });

            QueryablePaging<PageRouteWithClaimDto> qp = lst.AsQueryable().GridifyQueryable(request, null);
            Paging<PageRouteWithClaimDto> result = new(qp.Count, qp.Query.ToList());
            return new SearchQueryResponse<PageRouteWithClaimDto>(request, result);

        }
    }
}
