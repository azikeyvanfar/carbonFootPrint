using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Domain.Entities.Core
{
    /// <summary>
    ///this roleId access this PageRouteId
    /// </summary>
    public class RolePageRouteAccess : BaseEntity, IEntity,
        ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public Guid PageRouteId { get; set; }
        public PageRoute PageRoute { get; set; }

    }
}
