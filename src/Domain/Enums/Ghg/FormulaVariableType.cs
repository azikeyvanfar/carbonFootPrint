namespace ContractorBackend.Domain.Enums.Ghg
{
    /// <summary>
    /// نوع متغیر در تعریف فرمول محاسبه - مبنای تولید پویا و خودکار فرم‌های ورود داده
    /// </summary>
    public enum FormulaVariableType
    {
        /// <summary>ورودی کاربر (مثل مقدار مصرف، تعداد تجهیزات، ...)</summary>
        Input = 1,
        /// <summary>ویژگی سوخت (مثل LHV)</summary>
        FuelProperty = 2,
        /// <summary>ضریب انتشار از تنظیمات (EmissionFactor)</summary>
        EmissionFactor = 3,
        /// <summary>پارامتر عمومی از تنظیمات (مثل ساعت کاری سالانه)</summary>
        Parameter = 4,
        /// <summary>پتانسیل گرمایش جهانی گاز (GWP)</summary>
        GlobalWarmingPotential = 5,
        /// <summary>خروجی فرمول دیگر (زیرفرمول)</summary>
        FormulaOutput = 6
    }
}
