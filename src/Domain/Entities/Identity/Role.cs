using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class Role : IdentityRole<long>, IEntity<long>, IAuditableEntity
    {
        [Obsolete("use Constractor with Type and Scope")]
        public Role(string name)
            : base(name)
        {

        }

        public Role(string name, RoleType roleType, Guid roleScopeId)
            : base(name)
        {
            RoleType = roleType;
            RoleScopeId = roleScopeId;
        }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        /// <summary>
        /// manager/employee
        /// </summary>
        public RoleType RoleType { get; set; }
        /// <summary>
        /// حوزه سیستم
        /// </summary>
        public Guid? RoleScopeId { get; set; }
        /// <summary>
        /// حوزه سیستم
        /// </summary>
        public Lookup RoleScope { get; set; }
        public bool IsAdministrator { get; set; }
        public virtual ICollection<UserRole> Users { get; set; }
        public virtual ICollection<RoleClaim> Claims { get; set; }
        public virtual ICollection<RolePageRouteAccess> RoleAccessItems { get; set; }
    }
}