using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Shared
{
    public class NewsCategoryConfiguration : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> entity)
        {

            entity.HasMany(x => x.NewsCategoryOrgUnits)
           .WithOne(x => x.NewsCategory)
           .HasForeignKey(x => x.NewsCategoryId)
           .OnDelete(DeleteBehavior.NoAction);

        }
    }
}