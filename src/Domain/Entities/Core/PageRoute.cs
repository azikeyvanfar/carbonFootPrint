using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    public class PageRoute : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; } = true;

        public string RouteName { get; set; }

        public string Route { get; set; }

        public string Icon { get; set; }

        public RoleType RoleType { get; set; }
        public bool IsOtp { get; set; }
        public bool? HasIsSuiteOtp { get; set; }

        public virtual ICollection<PageRouteClaim> PageRouteClaims { get; set; }

        public virtual ICollection<RolePageRouteAccess> RolePageRouteAccesses { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }

    }
}
