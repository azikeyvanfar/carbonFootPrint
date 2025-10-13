using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.MenuItems.Queries.GetCurrentUserMenuItemTreeByMenuId
{
    public class GetCurrentUserMenuItemTreeByMenuIdQuery : IRequest<IEnumerable<MenuItemDto>>
    {
        public Guid MenuId { get; set; }

        /// <summary>
        /// دسترسی به کلیم ها بر اساس نوع 
        /// Employee یا Manager
        /// بودن رول های کاربر
        /// </summary>
        public RoleType RequestedRoleType { get; set; }
    }

    public class GetCurrentUserMenuItemTreeByMenuIdQueryHandler : IRequestHandler<GetCurrentUserMenuItemTreeByMenuIdQuery, IEnumerable<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        //private readonly IRepository<PageRouteClaim> _repositoryPageRouteClaim;
        private readonly IRepository<RoleClaim> _repositoryRoleClaim;
        private readonly IRepository<RolePageRouteAccess> _repositoryPageRouteAcc;
        private readonly IMapper _mapper;
        private readonly IApplicationUserManager _userManager;
        private readonly RoleManager<Domain.Entities.Identity.Role> _roleManager;

        public GetCurrentUserMenuItemTreeByMenuIdQueryHandler(
            IRepository<MenuItem> repository,
            //IRepository<PageRouteClaim> repositoryPageRouteClaim,
            IRepository<RolePageRouteAccess> repositoryPageRouteAcc,
            IMapper mapper,
            IApplicationUserManager userManager,
            RoleManager<Domain.Entities.Identity.Role> roleManager,
            IRepository<RoleClaim> repositoryRoleClaim)
        {
            _repository = repository;
            _mapper = mapper;
            //_repositoryPageRouteClaim = repositoryPageRouteClaim;
            _userManager = userManager;
            _roleManager = roleManager;
            _repositoryPageRouteAcc = repositoryPageRouteAcc;
            _repositoryRoleClaim = repositoryRoleClaim;
        }

        public async Task<IEnumerable<MenuItemDto>> Handle(GetCurrentUserMenuItemTreeByMenuIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetCurrentUserAsync();
            var userRoles = await _userManager.GetRolesAsync(user, request.RequestedRoleType);
            var roleNames = userRoles != null && userRoles.Any() ?
                _roleManager.Roles
                    .Where(x => userRoles.Any(l => l == x.Id.ToString()) && x.RoleType == request.RequestedRoleType)
                    .Select(x => x.NormalizedName)
                    .ToList()
                : null;

            var pageRouteIds = _repositoryPageRouteAcc.GetAllAsNoTracking().Where(a => userRoles.Contains(a.RoleId.ToString()) && a.IsActive).Select(a => a.PageRouteId.ToString());

            var userClaims = _repositoryRoleClaim.GetAllAsNoTracking()
            .Include(x => x.PageRouteClaim).ThenInclude(x => x.PageRoute)
            .Include(x => x.PageRouteClaim).ThenInclude(x => x.GeneralClaim)
            .Include(x => x.Role)
            .Where(x => x.PageRouteClaim.IsActive)
            .Where(x => x.PageRouteClaim.GeneralClaim.RoleType == request.RequestedRoleType)
            .Where(x => x.PageRouteClaim.GeneralClaim.IsGlobal != true)
            .Where(x => x.PageRouteClaim.RoleClaims.Any(l => roleNames.Any(c => c == l.Role.NormalizedName)))

            .Where(x => pageRouteIds.Contains(x.PageRouteClaim.PageRouteId.ToString()))
            .Select(x => new PageRouteClaimDto
            {
                Id = x.PageRouteClaim.Id,
                ClaimName = x.PageRouteClaim.GeneralClaim.ClaimName,
                ClaimValue = x.PageRouteClaim.GeneralClaim.ClaimValue,
                PageRouteId = x.PageRouteClaim.PageRouteId,
                ClaimRoleType = x.PageRouteClaim.GeneralClaim.RoleType,
                GeneralClaimId = x.PageRouteClaim.GeneralClaimsId,
            });
            if (userClaims is null)
            {
                return null;
            }
            var menuItems = _repository.GetAllAsNoTracking()
                .Include(x => x.Children)
                .Include(x => x.PageRoute.PageRouteClaims).ThenInclude(x => x.GeneralClaim)
                .Where(x => x.RoleAccessType == request.RequestedRoleType)
                .Where(x => x.IsActive)
                .Where(x =>
                        (
                            (
                                x.PageRoute.PageRouteClaims.Any(c => userClaims.Any(b => b.ClaimValue == c.GeneralClaim.ClaimValue))
                            ) &&
                            (
                                pageRouteIds.Contains(x.PageRouteId.ToString())
                            )
                        )
                      );

            var result = menuItems
                       .Include(x => x.ParentMenuItem)
                       .Include(x => x.PageRoute)
                       .Where(x => x.MenuId == request.MenuId && x.ParentId == null)
                       .Select(x => new MenuItemDto
                       {
                           Id = x.Id,
                           Title = x.Title,
                           Name = x.Name,
                           Description = x.Description,
                           MenuType = x.MenuType,
                           PageRouteId = x.PageRouteId,
                           Route = x.PageRoute.Route,
                           RouteName = x.PageRoute.RouteName,
                           DocumentId = x.DocumentId,
                           BaseUrl = x.BaseUrl,
                           PageId = x.PageId,
                           MenuId = x.MenuId,
                           ParentId = x.ParentId,
                           IsActive = x.IsActive,
                           IsOpen = x.IsOpen,
                           Priority = x.Priority,
                           MenuName = x.Menu.Name,
                           MenuTitle = x.Menu.Title,
                           Icon = x.Icon,
                           HasClaim = userClaims.Where(c => c.PageRouteId == x.PageRouteId).ToList().Count > 0,
                           Children = GetChildren(menuItems.ProjectTo<MenuItemDto>(_mapper.ConfigurationProvider).ToList(), x.Id, userClaims.ToList())
                       })
                       .OrderBy(x => x.Priority)
                       .ToList();
            return result;
        }


        private static List<MenuItemDto> GetChildren(List<MenuItemDto> items, Guid parentId, List<PageRouteClaimDto> claimItem)
        {
            return items
                    .Where(x => x.ParentId == parentId)
                    .Select(x => new MenuItemDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Name = x.Name,
                        Description = x.Description,
                        MenuType = x.MenuType,
                        PageRouteId = x.PageRouteId,
                        Route = x.Route,
                        RouteName = x.RouteName,
                        DocumentId = x.DocumentId,
                        BaseUrl = x.BaseUrl,
                        ExternalUrlId = x.ExternalUrlId,
                        PageId = x.PageId,
                        MenuId = x.MenuId,
                        ParentId = x.ParentId,
                        IsActive = x.IsActive,
                        IsOpen = x.IsOpen,
                        Priority = x.Priority,
                        MenuName = x.MenuName,
                        MenuTitle = x.MenuTitle,
                        PageEnTitle = x.PageEnTitle,
                        PageTitle = x.PageTitle,
                        Icon = x.Icon,
                        HasClaim = claimItem.Where(c => c.PageRouteId == x.PageRouteId).ToList().Count > 0,
                        Children = GetChildren(items, x.Id, claimItem)
                    }).OrderBy(x => x.Priority).ToList();
        }
    }
}
