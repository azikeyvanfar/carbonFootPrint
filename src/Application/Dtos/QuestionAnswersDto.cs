using System;

namespace ContractorBackend.Application.Dtos
{
    public class QuestionAssessmentsDto : SortableDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string Question { get; set; }
        public string Assessment { get; set; }

        public DateTime CreateDate { get; set; }
        //public Guid OrgUnitId { get; set; }

        //public string OrgUnitName { get; set; }
        public Guid QASubjectId { get; set; }

        public string QASubjectName { get; set; }

        public long QuestionerId { get; set; }

        public string QuestionarDisplayName { get; set; }

        public long ResponderId { get; set; }

        public string ResponderDisplayName { get; set; }

        public DateTime? ResponseDate { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsPopular { get; set; }

        /// <summary>
        /// اگر ادمین باشد خودش هم جواب میدهد اگر نباشد باید کسی جز خود سئوال کننده جواب دهد
        /// </summary>
        public bool IsAdminType { get; set; }

        public long? LastModifiedByUserId { get; set; }
        public string LastModifiedByFullName { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
