using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Shared
{
    /// <summary>
    /// موضوعات سوال جواب
    /// </summary>
    public class QASubject : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string SubjectName { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
    }
}
