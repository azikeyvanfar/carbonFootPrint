using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.MenuItems.Queries.GetMenuItemTreeByRoleId
{
    public class GetMenuItemTreeByRoleIdQuery : SearchQueryRequest, IRequest<IEnumerable<MenuItemDto>>
    {
        public Guid MenuId { get; set; }
        public GetMenuItemTreeByRoleIdQuery(Guid menuId)
        {
            MenuId = menuId;
        }


    }

    public class GetMenuItemTreeByRoleIdQueryHandler : IRequestHandler<GetMenuItemTreeByRoleIdQuery, IEnumerable<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IRepository<RolePageRouteAccess> _rolePageRouteAccessRepo;
        private readonly IRepository<PageRouteClaim> _pageRouteClaimRepo;
        private readonly IRepository<RoleClaim> _roleClaimRepository;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IApplicationRoleManager _roleManager;



        public GetMenuItemTreeByRoleIdQueryHandler(IRepository<MenuItem> repository, IMapper mapper,
            IRepository<RolePageRouteAccess> rolePageRouteAccessRepo, IRepository<PageRouteClaim> pageRouteClaimRepo,
            IApplicationDbContext applicationDbContext, IApplicationRoleManager roleManager)
        {
            _repository = repository;
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _rolePageRouteAccessRepo = rolePageRouteAccessRepo;
            _pageRouteClaimRepo = pageRouteClaimRepo;
        }

        public async Task<IEnumerable<MenuItemDto>> Handle(GetMenuItemTreeByRoleIdQuery request, CancellationToken cancellationToken)
        {

            var allmenuitem = _repository.GetAll();
            var currentUserRole = _roleManager.GetRolesForCurrentUser().Where(_ => _.RoleType == RoleType.Employee);
            var pageRouteClaims = _pageRouteClaimRepo.GetAllAsNoTracking().Include(x => x.GeneralClaim).Include(x => x.PageRoute).Select(x => new PageRouteClaimDto
            {
                Id = x.Id,
                ClaimName = x.GeneralClaim.ClaimName,
                ClaimValue = x.GeneralClaim.ClaimValue,
                PageRouteId = x.Id,
                Route = x.PageRoute.Route,
                RouteName = x.PageRoute.RouteName,
                GeneralClaimId = x.GeneralClaimsId
            });
            return _repository.GetAllAsNoTracking()
                   .Include(x => x.ParentMenuItem)
                   //.Include(x => x.Form)
                   .Include(x => x.PageRoute)
                   .ThenInclude(x => x.RolePageRouteAccesses)
               .Where(x => x.MenuId == request.MenuId && x.PageRoute.RolePageRouteAccesses.Any(c => c.RoleId == currentUserRole.FirstOrDefault().Id))
                   .Select(x => new MenuItemDto
                   {
                       Id = x.Id,
                       Title = x.Title,
                       Name = x.Name,
                       Description = x.Description,
                       MenuType = x.MenuType,
                       Route = x.PageRoute.Route,
                       PageRouteId = x.PageRouteId,
                       RoleAccessType = x.RoleAccessType,
                       RouteName = x.PageRoute.RouteName,
                       DocumentId = x.DocumentId,
                       OwnerId = x.OwnerId,
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
                       UserPersonnelCode = x.Owner.PersonnelCode.ToString(),
                       UserFullName = x.Owner.FirstName + " " + x.Owner.LastName,
                       ClaimChildren = pageRouteClaims.Where(c => c.PageRouteId == x.PageRouteId).ToList(),
                       Children = GetChildren(_mapper.Map<List<MenuItemDto>>(allmenuitem), x.Id, pageRouteClaims.ToList())
                   }).OrderBy(x => x.Priority).AsQueryable();


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
                        Route = x.Route,
                        PageRouteId = x.PageRouteId,
                        RoleAccessType = x.RoleAccessType,
                        DocumentId = x.DocumentId,
                        OwnerId = x.OwnerId,
                        BaseUrl = x.BaseUrl,
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
                        DocumentName = x.DocumentName,
                        DocumentMimeType = x.DocumentMimeType,
                        DocumentExtension = x.DocumentExtension,
                        UserPersonnelCode = x.UserPersonnelCode,
                        UserAvatar = x.UserAvatar,
                        UserFullName = x.UserFullName,
                        ClaimChildren = claimItem.Where(c => c.PageRouteId == x.PageRouteId).ToList(),
                        Children = GetChildren(items, x.Id, claimItem)
                    }).OrderBy(x => x.Priority).ToList();
        }


    }
}
