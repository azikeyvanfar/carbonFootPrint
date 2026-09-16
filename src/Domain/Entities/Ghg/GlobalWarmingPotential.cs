using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// پتانسیل گرمایش جهانی گازها (GWP) - قابل ویرایش در تنظیمات.
    /// منبع فعلی: گزارش ششم IPCC (AR6) - Settings!B23:G24
    /// </summary>
    public class GlobalWarmingPotential : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// گاز
        /// </summary>
        public Guid? GasId { get; set; }
        public virtual GhgGas? Gas { get; set; }

        /// <summary>
        /// نام گاز (کلید متنی برای فرمول‌ها - مثل GwpCH4)
        /// </summary>
        public string GasKey { get; set; } = null!;

        /// <summary>
        /// مقدار GWP (CO2=1, CH4=27, N2O=273, SF6=24300, ...)
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// گزارش ارزیابی IPCC (AR5, AR6, ...)
        /// </summary>
        public string AssessmentReport { get; set; } = "AR6";

        /// <summary>
        /// افق زمانی (100 سال)
        /// </summary>
        public int TimeHorizon { get; set; } = 100;

        /// <summary>
        /// سال اعتبار
        /// </summary>
        public int ValidFromYear { get; set; }
    }
}
