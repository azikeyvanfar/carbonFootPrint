using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ContractorBackend.Application.Core.MenuItems.Queries.GetAllMenuItemsTree
{
    public class GetAllMenuItemsTreeQuery : SearchQueryRequest, IRequest<IEnumerable<MenuItemDto>>
    {


    }

    public class GetAllMenuItemsTreeQueryHandler : IRequestHandler<GetAllMenuItemsTreeQuery, IEnumerable<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _applicationDbContext;

        public GetAllMenuItemsTreeQueryHandler(IRepository<MenuItem> repository, IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IEnumerable<MenuItemDto>> Handle(GetAllMenuItemsTreeQuery request, CancellationToken cancellationToken)
        {
            var allmenuitem = _repository.GetAll();
            var allClaim = _applicationDbContext.PageRouteClaims.Include(x => x.GeneralClaim).Select(x => new PageRouteClaimDto
            {
                Id = x.Id,
                ClaimName = x.GeneralClaim.ClaimName,
                ClaimValue = x.GeneralClaim.ClaimValue,
                GeneralClaimId = x.GeneralClaimsId,
                PageRouteId = x.PageRouteId,
                Route = x.PageRoute.Route,
                RouteName = x.PageRoute.RouteName
            });
            return allmenuitem
                        .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDateTime"))
                        .Include(x => x.ParentMenuItem)
                        //.Include(x => x.Form)
                        //.Include(x => x.Owner)
                        .Include(x => x.PageRoute)
                        .Where(x => x.ParentId == null)
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
                                //UserPersonnelCode = x.Owner.PersonnelCode.ToString(),
                                //UserAvatar = x.Owner.UserDetails.ProfileImage != null ? Convert.ToBase64String(x.Owner.UserDetails.ProfileImage) : "",
                                //UserFullName = x.Owner.FirstName + " " + x.Owner.LastName,
                                //ClaimChildren = allClaim.Where(c => c.PageRouteId == x.PageRouteId).ToList(),
                                Children = GetChildren(allmenuitem.ProjectTo<MenuItemDto>(_mapper.ConfigurationProvider).ToList(), x.Id, allClaim.ToList())
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
                        PageRouteId = x.PageRouteId,
                        Route = x.Route,
                        RouteName = x.RouteName,
                        DocumentId = x.DocumentId,
                        OwnerId = x.OwnerId,
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
                        DocumentName = x.DocumentName,
                        DocumentMimeType = x.DocumentMimeType,
                        DocumentExtension = x.DocumentExtension,
                        //UserPersonnelCode = x.UserPersonnelCode,
                        //UserAvatar = x.UserAvatar,
                        //UserFullName = x.UserFullName,
                        //ClaimChildren = claimItem.Where(c => c.PageRouteId == x.PageRouteId).ToList(),
                        Children = GetChildren(items, x.Id, claimItem)
                    }).OrderBy(x => x.Priority).ToList();
        }

    }




}
