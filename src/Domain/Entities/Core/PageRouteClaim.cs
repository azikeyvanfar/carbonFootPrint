using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Domain.Entities.Core
{
    public class PageRouteClaim : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public GeneralClaims GeneralClaim { get; set; }
        public Guid GeneralClaimsId { get; set; }
        public PageRoute PageRoute { get; set; }
        public Guid PageRouteId { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }
}
