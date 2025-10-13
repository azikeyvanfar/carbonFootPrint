using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class JwtUserToken : IEntity, IAuditableEntity
    {
        public Guid Id { get; set; }


        public string? AccessTokenHash { get; set; }


        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; } = null!;


        public string? RefreshTokenIdHashSource { get; set; }


        public bool IsActive { get; set; } = true;

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

        public long UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
