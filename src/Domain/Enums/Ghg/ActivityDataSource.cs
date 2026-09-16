namespace ContractorBackend.Domain.Enums.Ghg
{
    /// <summary>
    /// منبع داده فعالیت - مطابق ستون Data Source در ورک‌بوک MSC-GHG
    /// در آینده منبع WebService به این لیست اضافه می‌شود (دریافت از سرویس وب)
    /// </summary>
    public enum ActivityDataSource
    {
        /// <summary>سفارش خرید (Purchase Order)</summary>
        PurchaseOrder = 1,
        /// <summary>قبض (Bill)</summary>
        Bill = 2,
        /// <summary>اندازه‌گیری (Measurement)</summary>
        Measurement = 3,
        /// <summary>نقشه‌های P&ID</summary>
        PAndId = 4,
        /// <summary>برآورد (Estimation)</summary>
        Estimation = 5,
        /// <summary>محاسبه (Calculation)</summary>
        Calculation = 6,
        /// <summary>گزارش‌های حمل و نقل (Transportation Reports)</summary>
        TransportationReport = 7,
        /// <summary>مصاحبه با پرسنل (Personnel interview)</summary>
        PersonnelInterview = 8,
        /// <summary>سرویس وب (Web Service) - برای دریافت خودکار در آینده</summary>
        WebService = 9
    }
}
