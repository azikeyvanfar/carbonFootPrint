using ContractorBackend.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Identity
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.HasOne(roleClaim => roleClaim.Role)
                   .WithMany(role => role.Claims)
                   .HasForeignKey(roleClaim => roleClaim.RoleId);
            builder.Property(e => e.ClaimValue).HasMaxLength(5000);
            builder.Property(e => e.ClaimType).HasMaxLength(1000);

            builder.ToTable("AppRoleClaims");
        }
    }
}