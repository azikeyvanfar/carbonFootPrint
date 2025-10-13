using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.PageRouteClaims.Queries.GetAllPageRouteClaims
{
    public class GetAllPageRouteClaimsCurrentUserQuery : SearchQueryRequest, IRequest<IEnumerable<PageRouteWithClaimDto>>
    {
        public RoleType RoleType { get; set; }
    }
    public class GetAllPageRouteClaimsCurrentUserQueryHandler : IRequestHandler<GetAllPageRouteClaimsCurrentUserQuery, IEnumerable<PageRouteWithClaimDto>>
    {
        private readonly IRepository<PageRouteClaim> _repository;
        private readonly IApplicationUserManager _userManager;
        private readonly RoleManager<Domain.Entities.Identity.Role> _roleManager;

        private readonly IApplicationDbContext _context;
        private readonly DbSet<RoleClaim> _roleClaims;

        public GetAllPageRouteClaimsCurrentUserQueryHandler(IRepository<PageRouteClaim> repository,
            IApplicationUserManager userManager,
            RoleManager<Domain.Entities.Identity.Role> roleManager,
            IApplicationDbContext context
            )
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _roleClaims = _context.Set<RoleClaim>();

        }
        public async Task<IEnumerable<PageRouteWithClaimDto>> Handle(GetAllPageRouteClaimsCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetCurrentUserAsync();
            var userRoles = await _userManager.GetRolesAsync(user, request.RoleType);
            var roleNames = userRoles != null && userRoles.Any() ?
                _roleManager.Roles
                    .Where(x => userRoles.Any(l => l == x.Id.ToString()) && x.RoleType == request.RoleType)
                    .Select(x => x.NormalizedName)
                : null;

            var query = _repository.GetAllAsNoTracking()
            .Include(c => c.PageRoute)
            .Include(c => c.RoleClaims).ThenInclude(c => c.Role)
            .Include(_ => _.GeneralClaim)
            .Where(_ => _.GeneralClaim.IsGlobal != true && _.PageRoute.RoleType == request.RoleType);


            var lst = query.AsEnumerable()
                .GroupBy(_ => _.PageRouteId)
                .Select(pageRouteClaims => new PageRouteWithClaimDto
                {
                    PageRouteId = pageRouteClaims.Key,
                    Id = pageRouteClaims.Key,
                    Route = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.Route ?? "",
                    RouteName = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RouteName ?? "",
                    RoleType = pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RoleType,
                    RoleTypeId = ((int)pageRouteClaims.FirstOrDefault(_ => _.PageRouteId == pageRouteClaims.Key).PageRoute.RoleType).ToString() ?? "",

                    Claims = query
                        .Where(_ => _.PageRouteId == pageRouteClaims.Key)
                        .Where(c => c.RoleClaims.Any(l => roleNames.Any(v => v == l.Role.NormalizedName)))
                        .Where(c => _roleClaims.Any(v => v.ClaimValue == c.GeneralClaim.ClaimValue))
                        .Select(claim => new ClaimDto
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



            return lst;

        }
    }
}
