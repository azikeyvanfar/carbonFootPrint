using ContractorBackend.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Identity
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property<string>("Name").HasMaxLength(450).IsRequired();
            builder.Property<string>("NormalizedName").HasMaxLength(450).IsRequired();
            builder.ToTable("AppRoles");
        }
    }
}