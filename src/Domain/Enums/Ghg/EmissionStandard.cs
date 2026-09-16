namespace ContractorBackend.Domain.Enums.Ghg
{
    /// <summary>
    /// استاندارد / منبع روش‌شناسی ضریب انتشار یا فرمول محاسبه.
    /// طراحی شده به گونه‌ای که در آینده روش‌های CBAM و LCA نیز قابل افزودن باشند.
    /// </summary>
    public enum EmissionStandard
    {
        /// <summary>محاسبه شده در این مطالعه (Calculated in this study)</summary>
        Calculated = 0,
        /// <summary>IPCC 2006 Guidelines for National GHG Inventories</summary>
        Ipcc2006 = 1,
        /// <summary>IPCC Sixth Assessment Report (AR6)</summary>
        IpccAR6 = 2,
        /// <summary>DEFRA-UK Government GHG Conversion Factors 2024</summary>
        Defra2024 = 3,
        /// <summary>GHG Protocol - Mobile Combustion V2.7 / Iron and Steel V2.0</summary>
        GhgProtocol = 4,
        /// <summary>ECOINVENT V3.11 / SimaPro 2025</summary>
        Ecoinvent = 5,
        /// <summary>World Steel Association - CO2 Data Collection 2024_1</summary>
        WorldSteel = 6,
        /// <summary>IEA Life Cycle Upstream Emission Factors 2023</summary>
        Iea2023 = 7,
        /// <summary>TCEQ - Air Permit Technical Guidance (Equipment Leak Fugitives)</summary>
        Tceq = 8,
        /// <summary>API Compendium 2009</summary>
        Api2009 = 9,
        /// <summary>ISO 14064-1:2018</summary>
        Iso14064 = 10,
        /// <summary>ISO 14067:2018</summary>
        Iso14067 = 11,
        /// <summary>CBAM - EU Carbon Border Adjustment Mechanism (آینده / future)</summary>
        Cbam = 12,
        /// <summary>LCA - Life Cycle Assessment (آینده / future)</summary>
        Lca = 13,
        /// <summary>سایر / تعریف کاربر</summary>
        Custom = 99
    }
}
