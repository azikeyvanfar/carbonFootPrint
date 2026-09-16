using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// فرمول محاسبه انتشار - قلب سیستم. تمام فرمول‌های ورک‌بوک MSC-GHG و دستورالعمل EMSPR
    /// به صورت عبارت‌های ریاضی قابل ویرایش در بخش تنظیمات ذخیره می‌شوند.
    /// متغیرها به صورت JSON ذخیره می‌شوند و مبنای تولید پویا و خودکار فرم ورود داده هستند.
    /// ساختار برای افزودن فرمول‌های CBAM و LCA در آینده آماده است.
    /// </summary>
    public class CalculationFormula : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// کد یکتای فرمول (GHG-COMBUSTION-GAS, GHG-COMBUSTION-CO2E, ...)
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; } = null!;

        /// <summary>
        /// دسته ضریب مرتبط
        /// </summary>
        public EmissionFactorCategory? Category { get; set; }

        /// <summary>
        /// عبارت ریاضی (مثل: Consumption * LHV * EF)
        /// </summary>
        public string Expression { get; set; } = null!;

        /// <summary>
        /// تعریف متغیرها به صورت JSON:
        /// [{"name":"Consumption","label":"مصرف سالانه","type":"Input","unit":"kNm3"},
        ///  {"name":"LHV","label":"ارزش حرارتی سوخت","type":"FuelProperty"},
        ///  {"name":"EF","label":"ضریب انتشار","type":"EmissionFactor","refKey":"Natural Gas","gas":"CO2"}]
        /// </summary>
        public string VariablesJson { get; set; } = "[]";

        /// <summary>
        /// واحد خروجی (tCO2/y, tCO2e/y, ...)
        /// </summary>
        public string OutputUnit { get; set; } = null!;

        /// <summary>
        /// استاندارد / مبنای قانونی فرمول
        /// </summary>
        public EmissionStandard Standard { get; set; } = EmissionStandard.Iso14064;

        /// <summary>
        /// مرجع (بخش دستورالعمل EMSPR یا شیت ورک‌بوک)
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// فعال بودن فرمول
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// نسخه فرمول
        /// </summary>
        public int Version { get; set; } = 1;
    }
}
