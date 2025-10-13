using System;
using AutoMapper;
using ContractorBackend.Application.Common.Services;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.MenuItems.Commands.CreateMenuItem;
using ContractorBackend.Application.MenuItems.Commands.UpdateMenuItem;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;


namespace ContractorBackend.Application.Common.Mappings.Core
{
    public class MenuItemProfile : Profile
    {

        public MenuItemProfile()
        {
            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Icon, m => m.MapFrom(s => s.Icon))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                .ForMember(s => s.MenuType, m => m.MapFrom(s => s.MenuType))
                .ForMember(s => s.MenuTypeId, m => m.MapFrom(s => ((int)s.MenuType).ToString()))
                .ForMember(s => s.MenuTypeName, m => m.MapFrom(s => EnumHelper<MenuType>.GetDisplayValue(s.MenuType)))
                .ForMember(s => s.RoleAccessType, m => m.MapFrom(s => s.RoleAccessType))
                .ForMember(s => s.RoleAccessTypeId, m => m.MapFrom(s => ((int)s.RoleAccessType).ToString()))
                .ForMember(s => s.RoleAccessTypeName, m => m.MapFrom(s => EnumHelper<RoleType>.GetDisplayValue(s.RoleAccessType)))
                .ForMember(s => s.PageRouteId, m => m.MapFrom(s => s.PageRouteId))
                .ForMember(s => s.RouteName, m => m.MapFrom(s => s.PageRoute != null ? s.PageRoute.RouteName : ""))
                .ForMember(s => s.Route, m => m.MapFrom(s => s.PageRoute != null ? s.PageRoute.Route : ""))
                .ForMember(s => s.BaseUrl, m => m.MapFrom(s => s.BaseUrl))
                .ForMember(s => s.PageId, m => m.MapFrom(s => s.PageId))
                .ForMember(s => s.MenuId, m => m.MapFrom(s => s.MenuId))
                .ForMember(s => s.Priority, m => m.MapFrom(s => s.Priority))
                .ForMember(s => s.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(s => s.ParentTitle, m => m.MapFrom(s => s.ParentId != null ? s.ParentMenuItem.Title ?? "" : string.Empty))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForMember(s => s.IsOpen, m => m.MapFrom(s => s.IsOpen))
                .ForMember(s => s.OwnerId, m => m.MapFrom(s => s.OwnerId))
                .ForMember(s => s.MenuName, m => m.MapFrom(s => s.Menu != null ? s.Menu.Name : string.Empty))
                .ForMember(s => s.MenuTitle, m => m.MapFrom(s => s.Menu != null ? s.Menu.Title : string.Empty))
                .ForMember(d => d.DocumentName, m => m.MapFrom(s => s.Document != null ? s.Document.GeneratedName : string.Empty))
                .ForMember(d => d.DocumentMimeType, m => m.MapFrom(s => s.Document != null ? s.Document.MimeType : string.Empty))
                .ForMember(d => d.DocumentExtension, m => m.MapFrom(s => s.Document != null ? s.Document.Extension : string.Empty))
                .ForMember(d => d.UserPersonnelCode, m => m.MapFrom(s => s.Owner != null ? s.Owner.PersonnelCode.ToString() : string.Empty))
                //.ForMember(d => d.UserAvatar, m => m.MapFrom(s => s.Owner != null ? s.Owner.UserDetails.ProfileImage.ToString() : null))
                .ForMember(d => d.UserFullName, m => m.MapFrom(s => s.Owner != null ? s.Owner.FirstName + " " + s.Owner.LastName : string.Empty))
                .ForMember(d => d.Children, m => m.MapFrom(s => s.Children))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateMenuItemCommand, MenuItem>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Icon, m => m.MapFrom(s => s.Icon))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                .ForMember(s => s.MenuType, m => m.MapFrom(s => s.MenuType))
                .ForMember(s => s.RoleAccessType, m => m.MapFrom(s => s.RoleAccessType))
                .ForMember(s => s.PageRouteId, m => m.MapFrom(s => s.PageRouteId))
                .ForMember(s => s.BaseUrl, m => m.MapFrom(s => s.BaseUrl))
                .ForMember(s => s.PageId, m => m.MapFrom(s => s.PageId))
                .ForMember(s => s.MenuId, m => m.MapFrom(s => s.MenuId))
                .ForMember(s => s.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateMenuItemCommand, MenuItem>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Icon, m => m.MapFrom(s => s.Icon))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                .ForMember(s => s.MenuType, m => m.MapFrom(s => s.MenuType))
                .ForMember(s => s.RoleAccessType, m => m.MapFrom(s => s.RoleAccessType))
                .ForMember(s => s.PageRouteId, m => m.MapFrom(s => s.PageRouteId))
                .ForMember(s => s.BaseUrl, m => m.MapFrom(s => s.BaseUrl))
                .ForMember(s => s.PageId, m => m.MapFrom(s => s.PageId))
                .ForMember(s => s.Priority, m => m.MapFrom(s => s.Priority))
                .ForMember(s => s.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
