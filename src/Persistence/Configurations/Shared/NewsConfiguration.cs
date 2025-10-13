using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Shared
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {

            builder.Property<string>("Title").HasMaxLength(450).IsRequired();
            builder.Property<string>("Subtitle").HasMaxLength(450);
        }
    }
}
