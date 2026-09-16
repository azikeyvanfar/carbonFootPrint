using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// نتیجه محاسبه انتشار - معادل شیت Inventory ورک‌بوک MSC-GHG.
    /// برای هر (دوره، دسته، ناحیه) کد انتشار به شکل ناحیه-علامت-شماره ساخته می‌شود (مثل 1-C-1).
    /// </summary>
    public class EmissionResult : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// دوره گزارش‌دهی
        /// </summary>
        public Guid PeriodId { get; set; }
        public virtual GhgPeriod Period { get; set; } = null!;

        /// <summary>
        /// دسته انتشار
        /// </summary>
        public Guid CategoryId { get; set; }
        public virtual EmissionCategory Category { get; set; } = null!;

        /// <summary>
        /// ناحیه
        /// </summary>
        public Guid? AreaId { get; set; }
        public virtual GhgArea? Area { get; set; }

        /// <summary>
        /// کد انتشار (ناحیه-علامت-شماره)
        /// </summary>
        public string EmissionCode { get; set; } = null!;

        /// <summary>
        /// انتشار CO2 (تن در سال)
        /// </summary>
        public double Co2 { get; set; }

        /// <summary>
        /// انتشار CH4 (تن در سال)
        /// </summary>
        public double Ch4 { get; set; }

        /// <summary>
        /// انتشار N2O (تن در سال)
        /// </summary>
        public double N2o { get; set; }

        /// <summary>
        /// انتشار SF6 (تن در سال)
        /// </summary>
        public double Sf6 { get; set; }

        /// <summary>
        /// انتشار معادل CO2 (tCO2e/y)
        /// </summary>
        public double Co2e { get; set; }

        /// <summary>
        /// سهم از کل (درصد)
        /// </summary>
        public double SharePct { get; set; }
    }
}
