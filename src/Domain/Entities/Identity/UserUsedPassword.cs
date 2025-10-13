using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class UserUsedPassword : IEntity, IAuditableEntity
    {
        public Guid Id { get; set; }

        public string HashedPassword { get; set; } = null!;

        public long UserId { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual User User { get; set; } = null!;
    }
}