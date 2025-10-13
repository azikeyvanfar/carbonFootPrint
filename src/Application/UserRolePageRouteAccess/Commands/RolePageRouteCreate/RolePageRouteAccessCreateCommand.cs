using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace ContractorBackend.Application.UserRolePageRouteAccesses.Commands.RolePageRouteUpsert
{
    public class RolePageRouteAccessCreateCommand : IRequest
    {
        public long RoleId { get; set; }

        public List<PageRouteWithClaimsDto> PageRouteClaimDtos { get; set; }
    }
    public class PageRouteWithClaimsDto
    {
        public Guid PageRouteId { get; set; }
        public List<Guid> GeneralClaimIds { get; set; } = new();
    }
    public class RolePageRouteAccessCreateCommandHandler : IRequestHandler<RolePageRouteAccessCreateCommand>
    {
        private readonly IRepository<RolePageRouteAccess> _repository;
        private readonly IRepository<PageRouteClaim> _pageRouteClaimRepository;
        private readonly IRepository<RoleClaim> _roleClaimRepository;


        public RolePageRouteAccessCreateCommandHandler(
            IRepository<RolePageRouteAccess> repository,
            IRepository<PageRouteClaim> pageRouteClaimRepository,
            IRepository<RoleClaim> roleClaimRepository)
        {
            _repository = repository;
            _pageRouteClaimRepository = pageRouteClaimRepository;
            _roleClaimRepository = roleClaimRepository;
        }
        public async Task<Unit> Handle(RolePageRouteAccessCreateCommand request, CancellationToken cancellationToken)
        {
            request.PageRouteClaimDtos = request.PageRouteClaimDtos.Where(_ => _.GeneralClaimIds != null).ToList();

            #region Delete Removed PageRoutes For Current Role  
            var deleteRolePageRouteAccessList = _repository.GetAll()
                .Where(x => x.RoleId == request.RoleId)
                .ToList()
                .Where(x => request.PageRouteClaimDtos.All(l => l.PageRouteId != x.PageRouteId))
                .ToList();
            if (deleteRolePageRouteAccessList.Any())
            {
                //Delete list RolePageRouteAccess
                _repository.DeleteRange(deleteRolePageRouteAccessList);
            }


            #region New Query for delete with iterate on each page
            foreach (var page in request.PageRouteClaimDtos.ToList())
            {
                var deleteRoleClaims = _roleClaimRepository.GetAll()
                   .Include(x => x.PageRouteClaim)
                   .Where(x => x.RoleId == request.RoleId)
                   .AsEnumerable()
                   .Where(x => x.PageRouteClaim?.PageRouteId != page.PageRouteId || page.GeneralClaimIds.All(z => z != x.PageRouteClaim?.GeneralClaimsId))
                   .ToList();

                if (deleteRoleClaims.Any())
                {
                    //delete list roleclaim
                    _roleClaimRepository.DeleteRange(deleteRoleClaims);
                }
            }
            // if role has no page assigned to it delete all existing roleClaims 
            if (request.PageRouteClaimDtos == null || request.PageRouteClaimDtos.Count == 0)
            {
                var deleteRoleClaims = _roleClaimRepository.GetAll()
                  .Where(x => x.RoleId == request.RoleId)
                  .AsEnumerable()
                  .ToList();

                if (deleteRoleClaims.Any())
                {
                    //delete list roleclaim
                    _roleClaimRepository.DeleteRange(deleteRoleClaims);
                }
            }
            #endregion


            #endregion

            var roleItemAccess = new List<RolePageRouteAccess>();
            foreach (var item in request.PageRouteClaimDtos)
            {
                var currentAccess = _repository.GetAllAsNoTracking().FirstOrDefault(x => x.PageRouteId == item.PageRouteId && x.RoleId == request.RoleId);
                if (currentAccess is null)
                {
                    // Insert RolePageRouteAccess if null
                    _repository.Insert(new RolePageRouteAccess()
                    {
                        PageRouteId = item.PageRouteId,
                        RoleId = request.RoleId
                    });
                }

                item.GeneralClaimIds = item.GeneralClaimIds.Distinct().ToList();
                var roleClaim = new List<RoleClaim>();
                foreach (var claimId in item.GeneralClaimIds)
                {
                    var pageRouteClaim = _pageRouteClaimRepository.GetAllAsNoTracking().Include(_ => _.PageRoute).Include(_ => _.GeneralClaim)
                        .FirstOrDefault(_ => _.PageRouteId == item.PageRouteId && _.GeneralClaimsId == claimId);

                    if (pageRouteClaim != null)//it must NOT be null
                    {
                        var exists = _roleClaimRepository.GetAll()
                            .FirstOrDefault(x =>
                                x.ClaimValue == pageRouteClaim.GeneralClaim.ClaimValue &&
                                x.RoleId == request.RoleId &&
                                x.PageRouteClaimId == pageRouteClaim.Id
                            );
                        if (exists is null)
                        {
                            roleClaim.Add(new RoleClaim()
                            {
                                ClaimValue = pageRouteClaim.GeneralClaim.ClaimValue,
                                RoleId = request.RoleId,
                                ClaimType = "DynamicPermission",
                                PageRouteClaimId = pageRouteClaim.Id
                            });

                        }
                    }
                }
                //insert list roleClaim
                _roleClaimRepository.InsertRange(roleClaim);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}

