using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Shared
{
    public class QuestionAnswerConfigurations : IEntityTypeConfiguration<QuestionAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
        {
            builder.HasOne(x => x.QASubject).
                WithMany(x => x.QuestionAnswers).
                HasForeignKey(x => x.QaSubjectId).
                OnDelete(DeleteBehavior.Restrict);

            builder.Property<string>("Question").HasMaxLength(1000).IsRequired();
            builder.Property<string>("Answer").HasMaxLength(1000);
        }
    }
}