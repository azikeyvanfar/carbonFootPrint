using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Shared
{
    public class NewsCategory : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// اطلاعیه
        /// </summary>
        public bool IsNotifications { get; set; }

        public virtual ICollection<News> News { get; set; }

        public virtual ICollection<NewsCategoryOrgUnit> NewsCategoryOrgUnits { get; set; }
    }
}
