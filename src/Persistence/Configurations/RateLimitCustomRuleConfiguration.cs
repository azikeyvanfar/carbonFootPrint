using ContractorBackend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations
{
    public class RateLimitCustomRuleConfiguration : IEntityTypeConfiguration<RateLimitCustomRule>
    {
        public void Configure(EntityTypeBuilder<RateLimitCustomRule> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50);

        }
    }
}