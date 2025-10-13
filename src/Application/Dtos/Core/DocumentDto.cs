using System;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class DocumentDto
    {
        // DO NOT REMOVE THIS COMMENT:NG01
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? RootId { get; set; }
        public string AliasName { get; set; }
        public long OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string RealName { get; set; } = null!;
        // public String GeneratedName { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public DocumentType Type { get; set; } = DocumentType.Folder;
        public string Description { get; set; }
        public string Alt { get; set; }
        public string UserPersonnelCode { get; set; }
        public string UserAvatar { get; set; }
        public string UserFullName { get; set; }

        // DO NOT REMOVE THIS COMMENT:NG02
    }

    public class DocumentFileDto
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public long? OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string RealName { get; set; } = null!;
        public string GeneratedName { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string Description { get; set; }
        public string Alt { get; set; }
    }
    public class DocumentRoleDto
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public long RoleId { get; set; }

        // DO NOT REMOVE THIS COMMENT:NG02

    }
    public class DocumentRoleUserDto
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid Id { get; set; }
        public Guid DocumentRoleId { get; set; }
        public long UserId { get; set; }

        // DO NOT REMOVE THIS COMMENT:NG02
    }
}
