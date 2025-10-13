using System;
using ContractorBackend.Application.Dtos.Core;

namespace ContractorBackend.Application.Dtos.Share
{
    public class QASubjectDto : ShadowPropertyDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string SubjectName { get; set; }
    }
}
