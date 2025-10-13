using ContractorBackend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations
{
    public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
    {
        public void Configure(EntityTypeBuilder<Lookup> builder)
        {
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                ;

            builder.HasOne(x => x.Category)
                .WithMany(x => x.CategoryChildren)
                .HasForeignKey(x => x.CategoryId)
                ;

            builder.HasMany(x => x.Roles)
                .WithOne(x => x.RoleScope)
                .HasForeignKey(x => x.RoleScopeId)
                .OnDelete(DeleteBehavior.Restrict)
                ;

            builder.HasIndex(x => new { x.EnName, x.Code, x.Type });

            builder.Property<string>(x => x.EnName).HasMaxLength(4000);
            builder.Property<string>(x => x.FaName).HasMaxLength(4000);
            builder.Property<string>(x => x.Code).HasMaxLength(100);
            builder.Property<string>(x => x.Description).HasMaxLength(4000);
        }
    }
}
