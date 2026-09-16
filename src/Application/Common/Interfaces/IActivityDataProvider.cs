using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Application.Common.Interfaces
{
    /// <summary>
    /// آیتم داده فعالیت - نمایش مستقل از پایگاه داده برای انتقال بین منبع داده و سیستم.
    /// </summary>
    public sealed class ActivityDataItem
    {
        /// <summary>علامت دسته انتشار (C, V, F, E, M, W, Z, Y, ...)</summary>
        public string CategorySign { get; set; } = null!;

        /// <summary>کد ناحیه (1-9)</summary>
        public int? AreaCode { get; set; }

        /// <summary>کد مرکز هزینه</summary>
        public int? CostCenterCode { get; set; }

        /// <summary>نام سوخت (Natural Gas, Gas oil, ...)</summary>
        public string? FuelName { get; set; }

        /// <summary>کلید مرجع ضریب</summary>
        public string? FactorRefKey { get; set; }

        /// <summary>کلید فرعی ضریب (روش مدیریت پسماند و ...)</summary>
        public string? FactorSubKey { get; set; }

        /// <summary>شرح منبع انتشار</summary>
        public string? EmissionSource { get; set; }

        /// <summary>کمیت اصلی</summary>
        public double Quantity { get; set; }

        /// <summary>کمیت دوم (فاصله، COD، تولید و ...)</summary>
        public double? Quantity2 { get; set; }

        /// <summary>کمیت سوم</summary>
        public double? Quantity3 { get; set; }

        /// <summary>واحد</summary>
        public string? Unit { get; set; }

        /// <summary>درصد راندمان کنترل</summary>
        public double? ControlEfficiency { get; set; }

        /// <summary>منبع داده</summary>
        public ActivityDataSource DataSource { get; set; }

        /// <summary>توضیحات</summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// آیتم ردپای کربن محصول
    /// </summary>
    public sealed class ProductFootprintItem
    {
        public string Name { get; set; } = null!;
        public string? FaName { get; set; }
        public string? AreaName { get; set; }
        public double CarbonFootprint { get; set; }
        public double? UpstreamSharePct { get; set; }
        public double? AnnualProduction { get; set; }
        public string? Boundary { get; set; }
        public string? Standard { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// تأمین‌کننده داده فعالیت - لایه انتزاع بین منبع داده و سیستم محاسبه.
    /// پیاده‌سازی فعلی داده‌ها را از فایل‌های ارجاع (ورک‌بوک MSC-GHG و بسته فایل‌های datas) بذر می‌کند.
    /// در آینده پیاده‌سازی سرویس وب (WebServiceActivityDataProvider) جایگزین می‌شود و
    /// همین قرارداد را از طریق وب‌سرویس برآورده خواهد کرد.
    /// </summary>
    public interface IActivityDataProvider
    {
        /// <summary>
        /// نام منبع داده (ExcelSeed یا WebService)
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// توضیح منبع
        /// </summary>
        string Description { get; }

        /// <summary>
        /// دریافت داده‌های فعالیت برای یک دوره (سال شمسی)
        /// </summary>
        Task<List<ActivityDataItem>> GetActivityDataAsync(int persianYear);

        /// <summary>
        /// دریافت ردپای کربن محصولات (بخش 2 - ISO 14067) برای یک دوره
        /// </summary>
        Task<List<ProductFootprintItem>> GetProductFootprintsAsync(int persianYear);
    }
}
