using ContractorBackend.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class UserToken : IdentityUserToken<long>, IBaseEntity, IAuditableEntity
    {
        public virtual User User { get; set; } = null!;
    }
}