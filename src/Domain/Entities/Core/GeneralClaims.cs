using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    /// <summary>
    /// کلیم های سیستم - سیستمی پر میشود
    /// </summary>
    public class GeneralClaims : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; } = true;

        public RoleType RoleType { get; set; }

        [MaxLength(500)]
        public string ClaimValue { get; set; }
        [MaxLength(500)]
        public string ClaimName { get; set; }

        public bool IsGlobal { get; set; }

        public virtual ICollection<PageRouteClaim> PageRouteClaims { get; set; }

    }
}
