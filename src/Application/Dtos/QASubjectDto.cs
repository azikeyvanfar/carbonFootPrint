using System;
using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos
{
    public class QASubjectDto : ShadowPropertyDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string SubjectName { get; set; }
    }
}
