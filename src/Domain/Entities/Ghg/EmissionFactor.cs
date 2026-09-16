using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// ثبت ضریب انتشار - جایگزین تمام جداول ضرایب شیت Settings ورک‌بوک MSC-GHG Atlas.
    /// ضرایب بر اساس دسته، کلید مرجع (نوع سوخت/وسیله نقلیه/نیروگاه/نوع پسماند/ماده/...) و گاز ذخیره می‌شوند.
    /// این ساختار برای افزودن ضرایب CBAM و LCA در آینده آماده است.
    /// </summary>
    public class EmissionFactor : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// دسته ضریب
        /// </summary>
        public EmissionFactorCategory Category { get; set; }

        /// <summary>
        /// کلید مرجع (نام سوخت / نوع وسیله نقلیه / نوع نیروگاه / نوع پسماند / نام ماده / ...)
        /// </summary>
        public string RefKey { get; set; } = null!;

        /// <summary>
        /// برچسب نمایشی فارسی
        /// </summary>
        public string? RefLabel { get; set; }

        /// <summary>
        /// زیرکلید اختیاری (مثلاً روش مدیریت پسماند: Sale/Landfill یا بخش بالادست/فرآیند)
        /// </summary>
        public string? SubKey { get; set; }

        /// <summary>
        /// گاز مربوطه (CO2, CH4, N2O, CO2e, ...)
        /// </summary>
        public string Gas { get; set; } = "CO2e";

        /// <summary>
        /// مقدار ضریب
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// واحد ضریب (tCO2/GJ, tCO2/km/ton, gr/kWh, tCO2e/ton, ...)
        /// </summary>
        public string Unit { get; set; } = null!;

        /// <summary>
        /// استاندارد / منبع
        /// </summary>
        public EmissionStandard Standard { get; set; } = EmissionStandard.Ipcc2006;

        /// <summary>
        /// متن مرجع / توضیح
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// سال اعتبار (نسخه‌بندی سالانه ضرایب)
        /// </summary>
        public int ValidFromYear { get; set; }
    }
}
