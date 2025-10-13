using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.UserRolePageRouteAccesses.Queries.GetRolePageRouteAccessById
{
    public class GetRolePageRouteAccessByIdQuery : IRequest<RoleAccessDto>
    {
        public long RoleId { get; set; }
        public GetRolePageRouteAccessByIdQuery(long roleId)
        {
            RoleId = roleId;
        }
    }
    public class GetRolePageRouteAccessByIdQueryHandler : IRequestHandler<GetRolePageRouteAccessByIdQuery, RoleAccessDto>
    {

        private readonly IRepository<RolePageRouteAccess> _repository;
        private readonly IRepository<PageRouteClaim> _pageRouteClaimRepo;
        private readonly IRepository<RoleClaim> _roleClaimRepo;
        private readonly RoleManager<Domain.Entities.Identity.Role> _roleManager;
        public GetRolePageRouteAccessByIdQueryHandler(IRepository<RolePageRouteAccess> repository,
            IRepository<PageRouteClaim> pageRouteClaimRepo, IRepository<RoleClaim> roleClaimRepo,
            RoleManager<Domain.Entities.Identity.Role> roleManager)
        {
            _repository = repository;
            _roleClaimRepo = roleClaimRepo;
            _pageRouteClaimRepo = pageRouteClaimRepo;
            _roleManager = roleManager;
        }

        public async Task<RoleAccessDto> Handle(GetRolePageRouteAccessByIdQuery request, CancellationToken cancellationToken)
        {
            var roleWhichHaveAccesses = await _repository
                .GetAllAsNoTracking()
                .Where(x => x.RoleId == request.RoleId)
                .Select(_ => _.RoleId)
                .Distinct()
                .Select(a => new RoleAccessDto() { RoleId = a }).FirstOrDefaultAsync(cancellationToken: cancellationToken);


            var roleEntity = _roleManager.Roles.FirstOrDefault(_ => _.Id == roleWhichHaveAccesses.RoleId);
            roleWhichHaveAccesses.RoleName = roleEntity.Name;
            roleWhichHaveAccesses.RoleType = roleEntity.RoleType;
            roleWhichHaveAccesses.RoleTypeId = ((int)roleEntity.RoleType).ToString();
            roleWhichHaveAccesses.RolePageRouteList = _repository
                .GetAllAsNoTracking()
                .Include(_ => _.PageRoute)
                .Where(_ => _.RoleId == roleWhichHaveAccesses.RoleId)
                .Select(x => new RolePageRoute
                {
                    PageRouteId = x.PageRouteId,
                    Route = (x.PageRoute != null) ? x.PageRoute.Route : string.Empty,
                    RouteName = (x.PageRoute != null) ? x.PageRoute.RouteName : string.Empty,
                    PageRouteClaimList = _roleClaimRepo
                            .GetAllAsNoTracking()
                            .Where(_ =>
                                    _pageRouteClaimRepo
                                    .GetAllAsNoTracking()
                                    .Where(_ => _.PageRouteId == x.PageRouteId)
                                    .Select(_ => _.Id)
                                    .Contains(_.PageRouteClaimId.Value) && _.RoleId == roleWhichHaveAccesses.RoleId
                                  )
                            .Include(_ => _.PageRouteClaim).ThenInclude(_ => _.GeneralClaim)
                            .Select(claim => new PageRouteClaimVm
                            {
                                ClaimValue = claim.ClaimValue,
                                GeneralClaimId = (claim.PageRouteClaim != null && claim.PageRouteClaim.GeneralClaim != null) ? claim.PageRouteClaim.GeneralClaim.Id : Guid.Empty
                            })
                            .ToList()
                })
                .ToList();

            return roleWhichHaveAccesses;
        }
    }
}