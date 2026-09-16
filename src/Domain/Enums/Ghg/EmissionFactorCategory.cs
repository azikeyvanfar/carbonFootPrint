namespace ContractorBackend.Domain.Enums.Ghg
{
    /// <summary>
    /// دسته‌بندی عامل انتشار - مبنا برای گروه‌بندی ضرایب انتشار در تنظیمات
    /// Equivalent of the factor tables in the Settings sheet of MSC-GHG Atlas workbook.
    /// </summary>
    public enum EmissionFactorCategory
    {
        /// <summary>سوخت - احتراق ثابت (Fuel combustion - Settings!B46)</summary>
        FuelCombustion = 1,
        /// <summary>تولید برق (Electricity generation - Settings!B110)</summary>
        ElectricityGeneration = 2,
        /// <summary>حمل و نقل مواد (Material transportation - Settings!B80)</summary>
        MaterialTransportation = 3,
        /// <summary>حمل و نقل داخل سایت (On-site transportation - Settings!B89)</summary>
        OnSiteTransportation = 4,
        /// <summary>رفت و آمد پرسنل (Employee commuting - Settings!B61)</summary>
        EmployeeCommuting = 5,
        /// <summary>سفرهای کاری (Business travels - Settings!B69)</summary>
        BusinessTravel = 6,
        /// <summary>مدیریت پسماند (Waste management - Settings!B98)</summary>
        WasteManagement = 7,
        /// <summary>مواد خریداری شده (Purchased materials - Settings!B287)</summary>
        PurchasedMaterial = 8,
        /// <summary>مرتبط با انرژی - بالادست (Energy related upstream - Settings!B124)</summary>
        EnergyRelated = 9,
        /// <summary>تجهیزات فرار (Fugitive equipment - Settings!B138)</summary>
        FugitiveEquipment = 10,
        /// <summary>سیلاب‌ها (Utilities - Settings!B148)</summary>
        Utility = 11,
        /// <summary>فاضلاب (Wastewater)</summary>
        Wastewater = 12
    }
}
