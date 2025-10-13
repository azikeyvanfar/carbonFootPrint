using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Core;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class RoleClaim : IdentityRoleClaim<long>, IBaseEntity, IAuditableEntity, ISoftDeleteEntity
    {
        public Guid? PageRouteClaimId { get; set; }
        public PageRouteClaim PageRouteClaim { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}