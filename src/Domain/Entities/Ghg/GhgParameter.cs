using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// پارامترهای عمومی محاسبات - مطابق جدول Parameters در Settings!B12
    /// (ساعت کاری سالانه، درصد وزنی متان در گاز طبیعی، نرخ دلار، ...)
    /// </summary>
    public class GhgParameter : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// کلید یکتا برای استفاده در فرمول‌ها (WorkingHours, MethaneWtPct, ...)
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// نام انگلیسی پارامتر
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی پارامتر
        /// </summary>
        public string? FaName { get; set; }

        /// <summary>
        /// مقدار
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// واحد
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// منبع
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// سال اعتبار (نسخه‌بندی سالانه)
        /// </summary>
        public int Year { get; set; }
    }
}
