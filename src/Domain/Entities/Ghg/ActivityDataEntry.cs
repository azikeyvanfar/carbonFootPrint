using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// ردیف داده فعالیت (ورودی کاربر از فرم گام‌به‌گام) - معادل ردیف‌های شیت‌های
    /// Combustion, Vent, Fugitive, Electricity, Waste, Wastewater و ... در ورک‌بوک MSC-GHG.
    /// در آینده این داده‌ها از سرویس وب دریافت خواهند شد (ActivityDataSource.WebService).
    /// </summary>
    public class ActivityDataEntry : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
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
        /// مرکز هزینه
        /// </summary>
        public Guid? CostCenterId { get; set; }
        public virtual CostCenter? CostCenter { get; set; }

        /// <summary>
        /// سوخت (برای دسته احتراق)
        /// </summary>
        public Guid? FuelId { get; set; }
        public virtual Fuel? Fuel { get; set; }

        /// <summary>
        /// کلید مرجع ضریب (نام سوخت/نیروگاه/وسیله/ماده/...) برای اتصال خودکار به ضرایب تنظیمات
        /// </summary>
        public string? FactorRefKey { get; set; }

        /// <summary>
        /// کلید فرعی (روش مدیریت پسماند، کلاس پرواز و ...)
        /// </summary>
        public string? FactorSubKey { get; set; }

        /// <summary>
        /// شرح منبع انتشار (مثل: CO2 process vent in EAF (1-8))
        /// </summary>
        public string? EmissionSource { get; set; }

        /// <summary>
        /// کمیت اصلی (مصرف سوخت، برق مصرفی، تعداد تجهیزات، ...)
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// کمیت دوم (فاصله حمل، COD، تولید محصول و ...)
        /// </summary>
        public double? Quantity2 { get; set; }

        /// <summary>
        /// کمیت سوم (تعداد تجهیزات فرار: شیر، فلنج، PSV و ...)
        /// </summary>
        public double? Quantity3 { get; set; }

        /// <summary>
        /// واحد کمیت اصلی
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// درصد راندمان کنترل (برای فرار)
        /// </summary>
        public double? ControlEfficiency { get; set; }

        /// <summary>
        /// منبع داده
        /// </summary>
        public ActivityDataSource DataSource { get; set; } = ActivityDataSource.PurchaseOrder;

        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }
    }
}
