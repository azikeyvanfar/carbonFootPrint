using ContractorBackend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Identity
{
    public class CaptchaConfiguration : IEntityTypeConfiguration<Captcha>
    {
        public void Configure(EntityTypeBuilder<Captcha> builder)
        {
            builder.Property<string>("Key").HasMaxLength(500).IsRequired();
            builder.Property<string>("FinalCode").HasMaxLength(450);
            builder.ToTable("Captchas");
        }
    }
}