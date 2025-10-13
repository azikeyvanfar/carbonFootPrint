using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations
{
    public class QuestionAnswerConfigurations : IEntityTypeConfiguration<QuestionAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
        {
            builder.HasOne(x => x.QASubject).
                WithMany(x => x.QuestionAnswers).
                HasForeignKey(x => x.QaSubjectId).
                OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Questioner).
                WithMany(x => x.QuestionAnswerQuestioner).
                HasForeignKey(x => x.QuestionerId).
                OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Respond).
                WithMany(x => x.QuestionAnswerRespond).
                HasForeignKey(x => x.ResponderId).
                OnDelete(DeleteBehavior.Restrict);



            builder.Property<string>("Question").HasMaxLength(1000).IsRequired();
            builder.Property<string>("Answer").HasMaxLength(1000);
        }
    }
}