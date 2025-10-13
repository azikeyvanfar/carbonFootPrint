using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;


namespace ContractorBackend.Domain.Entities.Core
{
    public class Menu : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        [MaxLength(500)]
        public string Title { get; set; } = null!;
        [MaxLength(500)]
        public string Name { get; set; }
        public Guid? DocumentId { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(500)]
        public string Options { get; set; }
        public bool IsActive { get; set; } = true;
        public long? OwnerId { get; set; }
        public virtual User Owner { get; set; } = null!;
        public virtual Document Document { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }

    }
}
