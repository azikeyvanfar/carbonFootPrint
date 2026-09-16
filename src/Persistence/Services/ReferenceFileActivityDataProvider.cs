using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Persistence.Services
{
    /// <summary>
    /// تأمین‌کننده داده فعالیت از فایل‌های مرجع (وضعیت فعلی).
    /// داده‌ها از شیت‌های ورک‌بوک MSC-GHG Atlas (Combustion/Vent/Fugitive/Electricity/Wastewater)
    /// و بسته فایل‌های datas (ردپای کربن پسماند، سرباره، آنالیز گاز) استخراج و در ساختار
    /// ورک‌بوک یکپارچه شده‌اند. در آینده پیاده‌سازی WebService جایگزین همین قرارداد می‌شود.
    /// </summary>
    public class ReferenceFileActivityDataProvider : IActivityDataProvider
    {
        public string SourceName => "ReferenceFiles";

        public string Description => "داده‌های اولیه استخراج شده از ورک‌بوک MSC-GHG Atlas و بسته فایل‌های داده 1403 (پسماند، سرباره، آنالیز گاز طبیعی)";

        public Task<List<ActivityDataItem>> GetActivityDataAsync(int persianYear)
        {
            if (persianYear != 1403)
                return Task.FromResult(new List<ActivityDataItem>());

            var items = new List<ActivityDataItem>();
            items.AddRange(GetCombustionData());
            items.AddRange(GetVentData());
            items.AddRange(GetFugitiveData());
            items.AddRange(GetElectricityData());
            items.AddRange(GetWasteData());
            items.AddRange(GetWastewaterData());
            return Task.FromResult(items);
        }

        public Task<List<ProductFootprintItem>> GetProductFootprintsAsync(int persianYear)
        {
            if (persianYear != 1403)
                return Task.FromResult(new List<ProductFootprintItem>());

            // نتایج شیت Units CF - ردپای کربن محصولات (tCO2e/t)
            var products = new List<ProductFootprintItem>
            {
                new() { Name = "Steel Slab", FaName = "تختال (اسلب)", AreaName = "Steel Making", CarbonFootprint = 2.330168172395239, AnnualProduction = 7379115, Boundary = "Cradle-to-Gate", Standard = "ISO 14067:2018" },
                new() { Name = "Hot Rolled Coil", FaName = "کویل گرم (کلاف گرم)", AreaName = "Hot Rolling", CarbonFootprint = 2.421717, AnnualProduction = 3166500, Boundary = "Cradle-to-Gate", Standard = "ISO 14067:2018" },
                new() { Name = "Cold Rolled Coil", FaName = "کویل سرد نورد شده نهایی", AreaName = "Cold Rolling", CarbonFootprint = 2.575086, AnnualProduction = 1100000, Boundary = "Cradle-to-Gate", Standard = "ISO 14067:2018" },
                new() { Name = "Galvanized Coil", FaName = "کویل گالوانیزه", AreaName = "Cold Rolling", CarbonFootprint = 2.686904, AnnualProduction = 500000, Boundary = "Cradle-to-Gate", Standard = "ISO 14067:2018" },
                new() { Name = "Tin Plated Coil", FaName = "کویل قلع اندود", AreaName = "Cold Rolling", CarbonFootprint = 2.705102, AnnualProduction = 150000, Boundary = "Cradle-to-Gate", Standard = "ISO 14067:2018" }
            };
            return Task.FromResult(products);
        }

        // ---------- شیت Combustion: (ناحیه، مرکز هزینه، سوخت، مصرف) ----------
        private static IEnumerable<ActivityDataItem> GetCombustionData()
        {
            var rows = new (int? area, int? cc, string fuel, double q, string? src)[]
            {
                (9, null, "Methane", 0.016, "Purchase Order"),
                (1, null, "Butane", 2.387, "Purchase Order"),
                (2, null, "Butane", 6.023, "Purchase Order"),
                (3, null, "Butane", 2.794, "Purchase Order"),
                (4, null, "Butane", 14.562, "Purchase Order"),
                (5, null, "Butane", 0.594, "Purchase Order"),
                (6, null, "Butane", 0.099, "Purchase Order"),
                (7, null, "Butane", 0.692, "Purchase Order"),
                (9, null, "Butane", 4.559, "Purchase Order"),
                (7, null, "Acetylene", 0.011, "Purchase Order"),
                (9, null, "Acetylene", 0.063, "Purchase Order"),
                (5, 5120, "Gas oil", 12300000, null),
                (1, 1120, "Natural Gas", 39288.571, null),
                (1, 1140, "Natural Gas", 17136.383, null),
                (1, 1210, "Natural Gas", 93739.761, null),
                (2, 2210, "Natural Gas", 961.128, null),
                (2, 2220, "Natural Gas", 2883.388, null),
                (2, 2402, "Natural Gas", 15378.068, null),
                (2, 2410, "Natural Gas", 18718.137, null),
                (2, 2510, "Natural Gas", 3303.085, null),
                (3, 3101, "Natural Gas", 208.339, null),
                (3, 3210, "Natural Gas", 200005.167, null),
                (3, 3310, "Natural Gas", 1041.694, null),
                (3, 3410, "Natural Gas", 3125.082, null),
                (3, 3420, "Natural Gas", 833.354, null),
                (3, 3430, "Natural Gas", 625.016, null),
                (3, 3440, "Natural Gas", 625.016, null),
                (3, 3450, "Natural Gas", 833.354, null),
                (3, 3460, "Natural Gas", 1041.694, null),
                (4, 4230, "Natural Gas", 2967.854, null),
                (4, 4235, "Natural Gas", 3954.131, null),
                (4, 4410, "Natural Gas", 10891.096, null),
                (4, 4415, "Natural Gas", 4522.444, null),
                (4, 4420, "Natural Gas", 4283.64, null),
                (4, 4710, "Natural Gas", 361.167, null),
                (4, 4820, "Natural Gas", 5621.582, null),
                (5, 5120, "Natural Gas", 264967.796, null),
                (5, 5140, "Natural Gas", 38346.213, null),
                (5, 5220, "Natural Gas", 330.939, null),
                (5, 5330, "Natural Gas", 3852.432, null),
                (5, 5362, "Natural Gas", 14053.245, null),
                (5, 5365, "Natural Gas", 6168.77, null),
                (5, 5364, "Natural Gas", 2956.975, null)
            };

            foreach (var r in rows)
            {
                yield return new ActivityDataItem
                {
                    CategorySign = "C",
                    AreaCode = r.area,
                    CostCenterCode = r.cc,
                    FuelName = r.fuel,
                    FactorRefKey = r.fuel,
                    Quantity = r.q,
                    DataSource = r.src != null ? ActivityDataSource.PurchaseOrder : ActivityDataSource.Bill,
                    EmissionSource = r.cc.HasValue ? $"احتراق سوخت - مرکز هزینه {r.cc}" : $"احتراق سوخت - {r.fuel}"
                };
            }
        }

        // ---------- شیت Vent: (ناحیه، مرکز هزینه، فرآیند، تولید/گاز) ----------
        private static IEnumerable<ActivityDataItem> GetVentData()
        {
            var rows = new (int area, int cc, string process, double production, double? gasFeed)[]
            {
                (1, 1120, "Lime Making", 263251, null),
                (2, 2210, "EAFs", 7742718, null),
                (1, 1140, "Dolomite Making", 229204, null),
                (1, 1310, "Direct Reduction 1", 0, 1203870.004),
                (1, 1315, "Direct Reduction 2", 0, 565966.172)
            };

            foreach (var r in rows)
            {
                yield return new ActivityDataItem
                {
                    CategorySign = "V",
                    AreaCode = r.area,
                    CostCenterCode = r.cc,
                    FactorRefKey = r.process,
                    Quantity = r.production,
                    Quantity2 = r.gasFeed,
                    DataSource = ActivityDataSource.Measurement,
                    EmissionSource = $"انتشار فرآیندی CO2 - {r.process}"
                };
            }
        }

        // ---------- شیت Fugitive: (ناحیه، واحد، شیر، فلنج، PSV) ----------
        private static IEnumerable<ActivityDataItem> GetFugitiveData()
        {
            var rows = new (int area, string facility, int valves, int flanges, int psvs)[]
            {
                (1, "PRS1,2 - Pelletizing", 49, 25, 4),
                (1, "PRS3 - Lime Making", 27, 17, 2),
                (1, "PRS4 - Direct Reduction", 15, 22, 0),
                (2, "PRS5 - EAFs", 31, 16, 2),
                (2, "PRS6,7 - Casting Machines", 44, 31, 4),
                (3, "PRS8 - Hot Rolling", 27, 21, 3),
                (4, "PRS9,10 - Cold Rolling", 25, 18, 2),
                (5, "PRS11 - PowerPlant 1", 40, 17, 3),
                (5, "PRS12 - Industrial Water Production", 26, 15, 2),
                (5, "PRS13 - PowerPlant 2", 5, 8, 0),
                (9, "PRS14 - Fire Fighting", 18, 12, 2),
                (4, "PRS15 - Galvanizing Line", 27, 24, 2),
                (4, "PRS16 - ECL", 27, 24, 2),
                (5, "PRS17 - DM Water Production", 20, 14, 2),
                (1, "PRS18 - Dolomite Making", 20, 14, 2),
                (7, "PRS19 - Central Workshop", 20, 11, 1),
                (3, "PRS20 - Hot Rolling", 17, 18, 2),
                (1, "PRS21 - Direct Reduction", 4, 5, 0),
                (5, "PRS22 - Waste 21 Unit", 17, 18, 2),
                (5, "HoseinAbad Station - Methane Distribution", 7, 6, 0),
                (5, "Batching Point - Methane Distribution", 9, 6, 0),
                (5, "70000 Station - Methane Distribution", 72, 52, 2),
                (5, "200000 Station - Methane Distribution", 135, 129, 3),
                (5, "Valve Station - Methane Distribution", 2, 1, 0)
            };

            foreach (var r in rows)
            {
                yield return new ActivityDataItem
                {
                    CategorySign = "F",
                    AreaCode = r.area,
                    FactorRefKey = "CH4",
                    Quantity = r.valves,
                    Quantity2 = r.flanges,
                    Quantity3 = r.psvs,
                    ControlEfficiency = 0,
                    DataSource = ActivityDataSource.PAndId,
                    EmissionSource = $"نشتی متان تجهیزات - {r.facility}"
                };
            }
        }

        // ---------- شیت Electricity ----------
        private static IEnumerable<ActivityDataItem> GetElectricityData()
        {
            var rows = new (string type, double kwh, string source)[]
            {
                ("Total Average", 3359544285, "Calculation"),
                ("Direct Contract", 1015200000, "Bill"),
                ("Solar", 21819320, "Bill"),
                ("MSC Production", 1070502976, "Calculation"),
                ("MSC Production", 549536612, "Bill"),
                ("MSC Production", 1780192400, "Bill")
            };

            foreach (var r in rows)
            {
                yield return new ActivityDataItem
                {
                    CategorySign = "E",
                    AreaCode = 5,
                    FactorRefKey = r.type,
                    Quantity = r.kwh,
                    Unit = "kWh/y",
                    DataSource = ParseSource(r.source),
                    EmissionSource = $"مصرف برق - {r.type}"
                };
            }
        }

        // ---------- پسماند: فایل ردپاي كربن.xlsx (بسته datas) - نحوه مدیریت نگاشت شده به Sale/Landfill/Stockpile ----------
        private static IEnumerable<ActivityDataItem> GetWasteData()
        {
            var rows = new (int area, string unit, string name, string type, bool hazardous, double tons, string management, bool onsite)[]
            {
                (1, "آهک سازی", "ریزدانه سنگ آهک ضایعاتی", "Non-Metal", false, 10545, "زیرسازی", true),
                (1, "آهک سازی", "ریزدانه سنگ دولومیت ضایعاتی", "Non-Metal", false, 7173, "استفاده مجدد در فولادسازی", true),
                (1, "آهک سازی", "خروجی غبارگیر آهک", "Non-Metal", false, 30000, "فروش", false),
                (2, "کوره های قوس", "سرباره EAF", "Metal", false, 2441000, "فرآوری و استفاده مجدد", true),
                (2, "ریخته گری", "لجن ریخته گری", "Metal", false, 12250, "انباشت", true),
                (2, "ریخته گری", "پوسته اکسیدی", "Metal", false, 18645, "انباشت", true),
                (2, "کوره های قوس", "غبار بگ هوس (FTP)", "Metal", false, 143000, "انباشت", true),
                (2, "کوره های پاتیلی", "سرباره LF", "Metal", false, 111386, "انباشت", true),
                (2, "حمل مواد", "غبار شارژ", "Metal", false, 55000, "استفاده مجدد", true),
                (2, "حمل مواد", "فاین زیر سرندی", "Metal", false, 1464, "انباشت", true),
                (3, "خط نورد", "پوسته اکسیدی نورد گرم", "Metal", false, 82000, "فروش", false),
                (3, "خط نورد", "لجن کلاریفایر", "Metal", false, 12774, "فرآوری و تولید کنسانتره", true),
                (4, "واحد بازیابی اسید", "پودر اکسیدی واحد بازیابی", "Metal", false, 7800, "فروش/فرآوری", false),
                (4, "اسیدشویی", "لجن اسیدشویی", "Non-Metal", true, 300, "امحاء", false),
                (4, "تاندم میل و تمپرمیل", "لجن تاندم میل و تمپرمیل", "Non-Metal", true, 600, "امحاء", false),
                (4, "گالوانیزه و ورق رنگی", "لجن واحد گالوانیزه و رنگی", "Non-Metal", true, 20, "امحاء", false),
                (9, "همه نواحی", "روغن و امولسیون ضایعاتی", "Non-Metal", true, 300, "فروش", false)
            };

            foreach (var r in rows)
            {
                var subKey = r.management.Contains("فروش") ? "Sale" : "Landfill";
                yield return new ActivityDataItem
                {
                    CategorySign = r.onsite ? "W" : "Z",
                    AreaCode = r.area,
                    FactorRefKey = r.type,
                    FactorSubKey = subKey,
                    Quantity = r.tons,
                    Unit = "ton",
                    DataSource = ActivityDataSource.Measurement,
                    EmissionSource = $"{r.name} ({r.unit}) - {r.management}",
                    Description = r.hazardous ? "پسماند خطرناک" : null
                };
            }
        }

        // ---------- فاضلاب: شیت Wastewater - مصرف به تفکیک مرکز هزینه ----------
        private static IEnumerable<ActivityDataItem> GetWastewaterData()
        {
            var rows = new (int area, int cc, double m3)[]
            {
                (1, 1130, 22078),
                (1, 1140, 12588),
                (1, 1210, 352300),
                (1, 1310, 6873060.388),
                (1, 1315, 2407062.033),
                (1, 1320, 312907.99),
                (1, 1325, 109585.672),
                (1, 1330, 90961.628),
                (2, 2210, 4653985.043),
                (2, 2220, 1951867.195),
                (2, 2240, 2771048.517),
                (2, 2410, 4431461.934),
                (2, 2510, 3931024.555),
                (3, 3101, 310647.437),
                (3, 3210, 3698910.298),
                (3, 3310, 976169.524),
                (3, 3410, 734348.991),
                (3, 3450, 342522.473),
                (4, 4220, 1263248.297),
                (4, 4230, 416569.243),
                (4, 4410, 1937960.727),
                (4, 4310, 2423281.522),
                (4, 4810, 1068382.338),
                (5, 5120, 436064.026),
                (5, 5220, 1551053.849),
                (5, 5310, 10328990.599),
                (5, 5320, 2314179.615),
                (5, 5350, 3567131.756),
                (5, 5130, 2935431.207),
                (5, 5140, 2301890.14)
            };

            foreach (var r in rows)
            {
                yield return new ActivityDataItem
                {
                    CategorySign = "Y",
                    AreaCode = r.area,
                    CostCenterCode = r.cc,
                    FactorRefKey = "Wastewater",
                    Quantity = r.m3,
                    Unit = "m3/y",
                    DataSource = ActivityDataSource.Estimation,
                    EmissionSource = "تصفیه فاضلاب - مرکز هزینه " + r.cc
                };
            }
        }

        private static ActivityDataSource ParseSource(string s) => s switch
        {
            "Bill" => ActivityDataSource.Bill,
            "Measurement" => ActivityDataSource.Measurement,
            "Calculation" => ActivityDataSource.Calculation,
            "P&IDs" => ActivityDataSource.PAndId,
            _ => ActivityDataSource.Estimation
        };
    }
}
