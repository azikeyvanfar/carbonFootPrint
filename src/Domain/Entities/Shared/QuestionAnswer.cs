using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Domain.Entities.Shared
{
    public class QuestionAnswer : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {

        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreateDate { get; set; }

        //public AllowedQAOrgUnit OrgUnit { get; set; }

        //public Guid OrgUnitId { get; set; }

        public QASubject QASubject { get; set; }

        public Guid QaSubjectId { get; set; }

        public bool IsPopular { get; set; }

        public long QuestionerId { get; set; }

        public User Questioner { get; set; }

        public long? ResponderId { get; set; }

        /// <summary>
        /// user who responds
        /// </summary>
        public User Respond { get; set; }

        public DateTime? ResponseDate { get; set; }

        public bool IsPrivate { get; set; }

        /// <summary>
        /// اگر ادمین باشد خودش هم جواب میدهد اگر نباشد باید کسی جز خود سئوال کننده جواب دهد
        /// </summary>
        public bool IsAdminType { get; set; }


    }
}
