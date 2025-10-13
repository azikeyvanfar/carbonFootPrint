using System;

namespace ContractorBackend.Application.Dtos.Core
{
    public class QASubjectDto : ShadowPropertyDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string SubjectName { get; set; }
    }
}
