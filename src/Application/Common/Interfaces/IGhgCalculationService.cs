using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Ghg;

namespace ContractorBackend.Application.Common.Interfaces
{
    /// <summary>
    /// سرویس محاسبه انتشار گازهای گلخانه‌ای - اجرای فرمول‌های تعریف شده در تنظیمات
    /// بر روی داده‌های فعالیت دوره و تولید نتایج موجودی (Inventory)
    /// </summary>
    public interface IGhgCalculationService
    {
        /// <summary>
        /// محاسبه مجدد کل موجودی انتشار برای یک دوره
        /// </summary>
        Task<GhgCalculationSummaryDto> RecalculatePeriodAsync(Guid periodId);

        /// <summary>
        /// پیش‌نمایش محاسبه یک ردیف داده فعالیت بدون ذخیره نتیجه (برای فرم گام‌به‌گام)
        /// </summary>
        Task<ActivityPreviewDto> PreviewActivityAsync(ActivityPreviewRequestDto request);

        /// <summary>
        /// ساخت خلاصه نتایج محاسبه دوره (بر اساس نتیجه‌های ذخیره شده)
        /// </summary>
        Task<GhgCalculationSummaryDto> BuildSummaryAsync(Guid periodId);
    }
}
