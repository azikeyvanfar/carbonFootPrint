using ContractorBackend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations
{
    public class ApplicationSettingConfiguration : IEntityTypeConfiguration<ApplicationSetting>
    {
        public void Configure(EntityTypeBuilder<ApplicationSetting> builder)
        {
            builder.HasMany(x => x.RateLimitCustomRules)
                   .WithOne(x => x.ApplicationSetting)
                   .HasForeignKey(x => x.ApplicationSettingId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}