using ContractorBackend.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class UserRole : IdentityUserRole<long>, IBaseEntity, IAuditableEntity
    {
        public virtual User User { get; set; } = null!;

        public virtual Role Role { get; set; } = null!;
    }
}