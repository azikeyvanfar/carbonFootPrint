using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.UserRolePageRouteAccesses.Queries.GetAllRolePageRouteAccess
{
    public class GetAllRolePageRouteAccessQuery : SearchQueryRequest, IRequest<SearchQueryResponse<RoleAccessDto>>
    {
    }
    public class GetAllRolePageRouteAccessQueryHandler : IRequestHandler<GetAllRolePageRouteAccessQuery, SearchQueryResponse<RoleAccessDto>>
    {
        private readonly IRepository<RolePageRouteAccess> _repository;
        private readonly IRepository<PageRouteClaim> _pageRouteClaimRepo;
        private readonly IRepository<RoleClaim> _roleClaimRepo;
        private readonly RoleManager<Domain.Entities.Identity.Role> _roleManager;
        public GetAllRolePageRouteAccessQueryHandler(IRepository<RolePageRouteAccess> repository,
            IRepository<PageRouteClaim> pageRouteClaimRepo, IRepository<RoleClaim> roleClaimRepo,
            RoleManager<Domain.Entities.Identity.Role> roleManager)
        {
            _repository = repository;
            _roleClaimRepo = roleClaimRepo;
            _pageRouteClaimRepo = pageRouteClaimRepo;
            _roleManager = roleManager;
        }

        public async Task<SearchQueryResponse<RoleAccessDto>> Handle(GetAllRolePageRouteAccessQuery request, CancellationToken cancellationToken)
        {
            var rolesWhichHaveAccesses = _repository
                .GetAllAsNoTracking()
                .Select(_ => _.RoleId)
                .Distinct()
                .Select(a => new RoleAccessDto() { RoleId = a });

            QueryablePaging<RoleAccessDto> qp = await rolesWhichHaveAccesses.GridifyQueryableAsync<RoleAccessDto>(request, null, cancellationToken);
            Paging<RoleAccessDto> result = new(qp.Count, qp.Query.ToList());

            foreach (var item in result.Data)
            {
                var roleEntity = _roleManager.Roles.FirstOrDefault(_ => _.Id == item.RoleId);
                item.RoleName = roleEntity.Name;
                item.RoleType = roleEntity.RoleType;
                item.RoleTypeId = ((int)roleEntity.RoleType).ToString();
                item.RolePageRouteList = _repository
                    .GetAllAsNoTracking()
                    .Include(_ => _.PageRoute)
                    .Where(_ => _.RoleId == item.RoleId)
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
                                        .Contains(_.PageRouteClaimId.Value) && _.RoleId == item.RoleId
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
            }

            return new SearchQueryResponse<RoleAccessDto>(request, result);
        }
    }
}