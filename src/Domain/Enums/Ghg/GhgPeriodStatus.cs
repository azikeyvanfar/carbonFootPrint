namespace ContractorBackend.Domain.Enums.Ghg
{
    /// <summary>
    /// وضعیت دوره گزارش‌دهی گازهای گلخانه‌ای
    /// </summary>
    public enum GhgPeriodStatus
    {
        /// <summary>پیش‌نویس - در حال ورود داده</summary>
        Draft = 1,
        /// <summary>محاسبه شده - نتایج موجود است</summary>
        Calculated = 2,
        /// <summary>منتشر شده - نهایی و تایید شده</summary>
        Published = 3
    }
}
