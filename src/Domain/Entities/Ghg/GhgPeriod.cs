using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// دوره گزارش‌دهی گازهای گلخانه‌ای (سال 1403 / 2024 و ...)
    /// </summary>
    public class GhgPeriod : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// سال شمسی (1403)
        /// </summary>
        public int PersianYear { get; set; }

        /// <summary>
        /// سال میلادی (2024)
        /// </summary>
        public int GregorianYear { get; set; }

        /// <summary>
        /// عنوان دوره
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// وضعیت
        /// </summary>
        public GhgPeriodStatus Status { get; set; } = GhgPeriodStatus.Draft;

        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }

        public virtual ICollection<ActivityDataEntry> ActivityDataEntries { get; set; } = new List<ActivityDataEntry>();
        public virtual ICollection<EmissionResult> EmissionResults { get; set; } = new List<EmissionResult>();
    }
}
