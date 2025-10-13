using System;

namespace ContractorBackend.Application.Dtos
{
    public class MenuDto
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Name { get; set; }
        public Guid? DocumentId { get; set; }
        public string? Description { get; set; }
        public string? Options { get; set; }
        public Guid? LanguageId { get; set; }
        public bool IsActive { get; set; } = true;
        public long OwnerId { get; set; }

        public string? LanguageName { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentMimeType { get; set; }
        public string? DocumentExtension { get; set; }
        public string? UserPersonnelCode { get; set; }
        public string? UserAvatar { get; set; }
        public string? UserFullName { get; set; }

        // DO NOT REMOVE THIS COMMENT:NG02
    }
}
