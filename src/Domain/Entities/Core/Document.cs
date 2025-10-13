using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    public class Document : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Document()
        {
            Children = new List<Document>();
            Menus = new List<Menu>();
            MenuItems = new List<MenuItem>();
        }


        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public long OwnerId { get; set; }

        public User Owner { get; set; }
        /// <summary>
        /// برای فایل ها باید پر شود
        /// </summary>
        public Guid? RootId { get; set; }

        /// <summary>
        /// this field only used for folder alias name
        /// </summary>

        public string AliasName { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// for folders in first level where parentid is null this is real names of folder
        /// </summary> 
        public string Name { get; set; }
        public string RealName { get; set; } = null!;
        public string GeneratedName { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; } = null!;

        public DocumentType Type { get; set; }

        public string Description { get; set; }


        public bool IsPublic { get; set; }

        public string Alt { get; set; }

        public Document Parent { get; set; }
        public virtual ICollection<Document> Children { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; }


        public Guid? NewsId { get; set; }

    }
}
