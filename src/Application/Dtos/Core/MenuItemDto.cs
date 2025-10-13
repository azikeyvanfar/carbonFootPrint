using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Name { get; set; }
        public Guid? DocumentId { get; set; }
        public string Description { get; set; }
        public MenuType MenuType { get; set; }
        public RoleType RoleAccessType { get; set; }
        public string RoleAccessTypeId { get; set; }
        public string RoleAccessTypeName { get; set; }
        public string MenuTypeId { get; set; }
        public string MenuTypeName { get; set; }
        public Guid? PageRouteId { get; set; }
        public string Route { get; set; }
        public string RouteName { get; set; }
        public string FormTitle { get; set; }
        public string BaseUrl { get; set; }
        public Guid? ExternalUrlId { get; set; }

        public string ExternalUrlName { get; set; }
        public string Url { get; set; }
        public Guid? PageId { get; set; }
        public Guid MenuId { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentTitle { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOpen { get; set; }
        public long? OwnerId { get; set; }
        public string MenuName { get; set; }
        public int? Priority { get; set; }
        public string Icon { get; set; }
        public string MenuTitle { get; set; }
        public string PageEnTitle { get; set; }
        public string PageTitle { get; set; }
        public string PageRoute { get; set; }
        public bool? PageIsSystem { get; set; }
        public string LanguageName { get; set; }
        public string DocumentName { get; set; }
        public string DocumentMimeType { get; set; }
        public string DocumentExtension { get; set; }
        public string UserPersonnelCode { get; set; }
        public string UserAvatar { get; set; }
        public string UserFullName { get; set; }
        public List<MenuItemDto> Children { get; set; }
        public List<PageRouteClaimDto> ClaimChildren { get; set; }

        public bool HasClaim { get; set; }
    }
}
