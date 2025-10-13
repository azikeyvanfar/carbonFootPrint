using ContractorBackend.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Identity
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasOne(userClaim => userClaim.User)
                   .WithMany(user => user.Claims)
                   .HasForeignKey(userClaim => userClaim.UserId);
            builder.Property(e => e.ClaimType).HasMaxLength(500);
            builder.Property(e => e.ClaimValue).HasMaxLength(500);


            builder.ToTable("AppUserClaims");
        }
    }
}