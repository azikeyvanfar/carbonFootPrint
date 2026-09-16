using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// ردپای کربن محصولات (بخش 2 - ISO 14067) - معادل نتایج شیت Units CF ورک‌بوک
    /// مثل: تختال 2.33 tCO2e/t
    /// </summary>
    public class ProductFootprint : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// دوره گزارش‌دهی
        /// </summary>
        public Guid PeriodId { get; set; }
        public virtual GhgPeriod Period { get; set; } = null!;

        /// <summary>
        /// نام محصول (Steel Slab, Hot Rolled Coil, ...)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی محصول (تختال، کویل گرم، ...)
        /// </summary>
        public string? FaName { get; set; }

        /// <summary>
        /// ناحیه تولیدکننده
        /// </summary>
        public string? AreaName { get; set; }

        /// <summary>
        /// ردپای کربن محصول (tCO2e/t)
        /// </summary>
        public double CarbonFootprint { get; set; }

        /// <summary>
        /// سهم انتشار بالادست (درصد)
        /// </summary>
        public double? UpstreamSharePct { get; set; }

        /// <summary>
        /// مقدار تولید سالانه (تن)
        /// </summary>
        public double? AnnualProduction { get; set; }

        /// <summary>
        /// مرز محاسبه (Cradle-to-Gate)
        /// </summary>
        public string? Boundary { get; set; }

        /// <summary>
        /// استاندارد محاسبه
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }
    }
}
