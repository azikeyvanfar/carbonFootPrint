using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    public class MenuItem : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        [MaxLength(500)]
        public string Name { get; set; }
        public Guid? DocumentId { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public MenuType MenuType { get; set; }
        public RoleType RoleAccessType { get; set; } = RoleType.Employee;
        public PageRoute PageRoute { get; set; }
        public Guid? PageRouteId { get; set; }
        public string BaseUrl { get; set; }

        public Guid? PageId { get; set; }
        public Guid MenuId { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOpen { get; set; }
        public long? OwnerId { get; set; }
        public int? Priority { get; set; }
        public string Icon { get; set; }

        public Document Document { get; set; }
        public Menu Menu { get; set; }
        public MenuItem ParentMenuItem { get; set; }
        public User Owner { get; set; } = null!;


        #region Navigation
        public ICollection<MenuItem> Children { get; set; }
        #endregion
    }

}
