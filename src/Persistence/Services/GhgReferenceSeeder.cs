using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Ghg;
using ContractorBackend.Domain.Enums.Ghg;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Services
{
    /// <summary>
    /// بذر داده‌های مرجع ماژول ردپای کربن - استخراج شده از:
    /// - شیت Settings ورک‌بوک MSC-GHG Atlas (نواحی، دسته‌ها، سوخت‌ها، ضرایب، GWP، پارامترها)
    /// - فرمول‌های شیت‌های محاسبه و دستورالعمل EMSPR (Mng_carbon.pdf)
    /// ساختار برای افزودن CBAM و LCA در آینده آماده است.
    /// </summary>
    public class GhgReferenceSeeder
    {
        private readonly IApplicationDbContext _context;

        public GhgReferenceSeeder(IApplicationDbContext context) => _context = context;

        public async Task SeedAsync()
        {
            await SeedAreasAsync();
            await SeedCostCentersAsync();
            await SeedEmissionCategoriesAsync();
            await SeedGasesAndGwpsAsync();
            await SeedParametersAsync();
            await SeedFuelsAsync();
            await SeedEmissionFactorsAsync();
            await SeedFormulasAsync();
            await SeedPeriodsAsync();
        }

        // ---------- نواحی: Settings!B341 + تعداد کارکنان Settings!B385 ----------
        private async Task SeedAreasAsync()
        {
            if (await _context.GhgAreas.AnyAsync()) return;

            var areas = new (int code, string name, string fa, string committee, int emp, int contractor)[]
            {
                (1, "Iron Making", "آهن‌سازی", "IRM", 910, 662),
                (2, "Steel Making", "فولادسازی", "SMC", 1640, 1232),
                (3, "Hot Rolling", "نورد گرم", "HSM", 873, 412),
                (4, "Cold Rolling", "نورد سرد", "CRM", 1785, 225),
                (5, "Utility", "انرژی و سیالات", "ENF", 655, 228),
                (6, "Transportation", "حمل و نقل", "TRA", 1233, 3897),
                (7, "Maintenance", "تعمیرات", "CEM", 0, 0),
                (9, "TAS", "ستاد", "TAI", 3305, 8673)
            };

            foreach (var a in areas)
            {
                _context.GhgAreas.Add(new GhgArea
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Code = a.code,
                    Name = a.name,
                    FaName = a.fa,
                    CommitteeCode = a.committee,
                    EmployeeCount = a.emp,
                    ContractorCount = a.contractor
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- مراکز هزینه: شیت Unit List ----------
        private async Task SeedCostCentersAsync()
        {
            if (await _context.CostCenters.AnyAsync()) return;

            var areas = await _context.GhgAreas.AsNoTracking().ToListAsync();
            var areaByCode = areas.ToDictionary(a => a.Code);

            // (code, name, unitProcess, areaCode) — از شیت Unit List
            var centers = new (int code, string name, string? unit, int area)[]
            {
                (1107, "Raw Material Maintenance", null, 1),
                (1110, "Iron Ore and Pellet Yard", "Yard", 1),
                (1120, "Limestone Calcination Plant", "Lime Making", 1),
                (1130, "Lime Hydration Plant", "Lime Hydration", 1),
                (1140, "Dolomite Plant", "Dolomite Making", 1),
                (1210, "Pelletizing Plant", "Pelletizing", 1),
                (1310, "Direct Reduction 1", "Direct Reduction", 1),
                (1315, "Direct Reduction 2", "Direct Reduction", 1),
                (1320, "Material Handling of Direct Reduction 1", "Direct Reduction", 1),
                (1325, "Material Handling of Direct Reduction 2", "Direct Reduction", 1),
                (1330, "Briquetting Plant 1", "Briquetting", 1),
                (1335, "Briquetting Plant 2", "Briquetting", 1),
                (2110, "Scrap Handling", "EAFs", 2),
                (2120, "Charge Handling", "EAFs", 2),
                (2130, "EAF Common Services", "EAFs", 2),
                (2210, "Electric Arc Furnaces", "EAFs", 2),
                (2220, "Degassing Plant", "De-gassing", 2),
                (2240, "Ladle Furnaces", "Laddle Furnaces", 2),
                (2250, "Desulphur Plant", "Laddle Furnaces", 2),
                (2402, "Continuous Casting Common Services", "Casting Machines", 2),
                (2410, "Continuous Casting Machine", "Casting Machines", 2),
                (2510, "Slab Cooling and Conditioning", "Slab Cooling", 2),
                (3101, "Roll Shop", "Hot Rolling Mill", 3),
                (3210, "Slab Stockyard and Slab Reheating Furnaces", "Hot Rolling Mill", 3),
                (3310, "Hot Rolling Mill", "Hot Rolling Mill", 3),
                (3410, "Coil Stockyard", "Shearing Line 1", 3),
                (3420, "Light Gauge Shearing Line N. 1", "Shearing Line 1", 3),
                (3430, "Light Gauge Shearing Line N. 2", "Shearing Line 2", 3),
                (3440, "Heavy Gauge Shearing Line", "Heavy Shearing Line", 3),
                (3450, "Skin Pass Mill", "Skin Pass Mill", 3),
                (4220, "Pickling Line No 1", "Pickling Line 1,2", 4),
                (4225, "Pickling Line No 2", "Pickling Line 1,2", 4),
                (4230, "Regeneration Plant No 1", "Pickling Line 1,2", 4),
                (4235, "Regeneration Plant No 2", "Pickling Line 1,2", 4),
                (4300, "Pickled Coil (Dummy C.C.)", "Tandem Mill", 4),
                (4310, "Tandem Mill", "Tandem Mill", 4),
                (4315, "Two Stand Reversible Mill", "Two Stand Reversible Mill", 4),
                (4405, "Electrolytic Cleaning Line", "Electrolytic Cleaning Line", 4),
                (4410, "Box Annealing No.1 (HNX)", "Box Annealing 1", 4),
                (4415, "Box Annealing No.3 (H2)", "Box Annealing 3", 4),
                (4420, "Box Annealing No.2 (H2)", "Box Annealing 2", 4),
                (4510, "Skinpass Mill No: 1 CRM", "Skin Pass Line 1,2", 4),
                (4515, "Skinpass Mill No: 2 CRM", "Skin Pass Line 1,2", 4),
                (4620, "Slitting Line", "Slitting Line", 4),
                (4630, "Light Gauge Shearing Line (CRM)", "Light Shearing Line (CRM)", 4),
                (4640, "Heavy Gauge Shearing Line (CRM)", "Heavy Shearing Line (CRM)", 4),
                (4660, "Packing And Shipping CRM", "Packaging", 4),
                (4710, "Tinning Line", "Tinning Line", 4),
                (4810, "Galvanizing Line", "Galvanizing Line", 4),
                (4820, "Coating Line", "Coating Line", 4),
                (5105, "Demineralized Water Production (RO3)", "RO3", 5),
                (5110, "Demineralized Water Production", "DM Water Production", 5),
                (5120, "Steam Production", "Steam Production", 5),
                (5130, "Electric Power Production No 1", "PowerPlant 1", 5),
                (5140, "Electric Power Production No 2", "PowerPlant 2", 5),
                (5150, "Electric Power Production No 3", "PowerPlant 3", 5),
                (5210, "Compressed Air Production", "Compressed Air Production", 5),
                (5220, "Oxygen & Hydrogen Plant", "Oxygen & Hydrogen", 5),
                (5310, "Industrial Water Purchase & Production", "Industrial Water Production", 5),
                (5320, "Drinking Water Production and Distribution", "Drinking Water", 5),
                (5330, "Water Conditioning", "Water Conditioning", 5),
                (5340, "Hot Strip Mill Water Treatment", "Hot Strip Mill Treatment", 5),
                (5350, "Industrial & Chemical Sewage Treatment", "Sewage Treatment", 5),
                (5364, "Demineralized Water & Steam Production (RO1)", "RO1", 5),
                (5365, "Demineralized Water & Steam Production (RO2)", "RO2", 5),
                (5420, "Steam Distribution", "Steam Distribution", 5),
                (5430, "Electrical Power Purchase and Distribution", "Electricity Distribution", 5),
                (6110, "Railway Transportation", null, 6),
                (6210, "Road & Lifting Transports", null, 6),
                (6410, "Slag Process & Handling", null, 6),
                (7210, "Mechanical Maintenance Center", null, 7),
                (7220, "Electrical Center", null, 7),
                (7230, "Refractory Center", null, 7),
                (9100, "Top Management", null, 9),
                (9220, "Purchase", null, 9),
                (9560, "Safety And Fire Fighting", null, 9),
                (9570, "Ecology", null, 9),
                (9630, "Central Laboratory", null, 9),
                (9730, "Waste Processing", null, 9)
            };

            foreach (var c in centers)
            {
                _context.CostCenters.Add(new CostCenter
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Code = c.code,
                    Name = c.name,
                    UnitProcess = c.unit,
                    AreaId = areaByCode.TryGetValue(c.area, out var area) ? area.Id : null
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- دسته‌های انتشار: Settings!B162 ----------
        private async Task SeedEmissionCategoriesAsync()
        {
            if (await _context.EmissionCategories.AnyAsync()) return;

            var cats = new (string name, string fa, string sign, int catNo, int scope, int priority)[]
            {
                ("Combustion", "احتراقی", "C", 1, 1, 1),
                ("Vent", "فرآیندی", "V", 1, 1, 2),
                ("Fugitive", "فرار", "F", 1, 1, 3),
                ("Mobile Combustion", "حمل و نقل داخلی", "M", 1, 1, 4),
                ("Electricity", "برق", "E", 2, 2, 5),
                ("Steam", "بخار", "S", 2, 2, 6),
                ("Compressed air", "هوای فشرده", "A", 2, 2, 7),
                ("Up-Transportation", "حمل و نقل بالادست", "T", 3, 3, 8),
                ("Down-Transportation", "حمل و نقل پایین‌دست", "D", 3, 3, 9),
                ("Commuting", "رفت و آمد پرسنل", "P", 3, 3, 10),
                ("Business travel", "سفرهای کاری", "B", 3, 3, 11),
                ("Purchased Material", "مواد اولیه (خوراک)", "R", 4, 3, 12),
                ("Ancillary Input", "مواد اولیه (مصرفی)", "L", 4, 3, 13),
                ("Purchased Services", "خرید خدمات", "X", 4, 3, 14),
                ("Waste Onsite", "پسماند (داخل سایت)", "W", 1, 1, 15),
                ("Waste Offsite", "پسماند (خارج سایت)", "Z", 4, 3, 16),
                ("Wastewater", "تصفیه فاضلاب", "Y", 1, 1, 17),
                ("Energy related", "مرتبط با انرژی", "N", 4, 3, 18)
            };

            foreach (var c in cats)
            {
                _context.EmissionCategories.Add(new EmissionCategory
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Name = c.name,
                    FaName = c.fa,
                    Sign = c.sign,
                    CategoryNo = c.catNo,
                    Scope = c.scope,
                    Priority = c.priority
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- گازها و GWP: Settings!B23 (IPCC AR6) ----------
        private async Task SeedGasesAndGwpsAsync()
        {
            if (await _context.GlobalWarmingPotentials.AnyAsync()) return;

            var gases = new (string key, string name, string fa, string formula, double gwp)[]
            {
                ("CO2", "Carbon Dioxide", "دی‌اکسید کربن", "CO2", 1),
                ("CH4", "Methane (C,B)", "متان (احتراقی/فراری)", "CH4", 27),
                ("N2O", "Nitrous Oxide", "نیتروس اکسید", "N2O", 273),
                ("SF6", "Sulfur Hexafluoride", "هگزا فلوراید گوگرد", "SF6", 24300),
                ("CFC-12", "CFC-12", "CFC-12", "CCl2F2", 12500),
                ("CH4-FV", "Methane (F,V)", "متان (فراری/فرآیندی)", "CH4", 29.8)
            };

            foreach (var g in gases)
            {
                var gas = new GhgGas
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Name = g.name,
                    FaName = g.fa,
                    ChemicalFormula = g.formula
                };
                _context.GhgGases.Add(gas);
                _context.GlobalWarmingPotentials.Add(new GlobalWarmingPotential
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    GasId = gas.Id,
                    GasKey = g.key,
                    Value = g.gwp,
                    AssessmentReport = "AR6",
                    TimeHorizon = 100,
                    ValidFromYear = 1403
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- پارامترها: Settings!B12 ----------
        private async Task SeedParametersAsync()
        {
            if (await _context.GhgParameters.AnyAsync()) return;

            var parameters = new (string key, string name, string? fa, double value, string? unit, string? source)[]
            {
                ("WorkingHours", "Working hours (hrs/yr)", "تعداد ساعت کاری در سال", 8760, "hrs/yr", "MSC"),
                ("WorkingDaysShift", "Working days for Shift Employees", "روزهای کاری پرسنل شیفتی", 365, "days", "MSC"),
                ("WorkingDaysDay", "Working days for Day Employees", "روزهای کاری پرسنل روزکار", 250, "days", "MSC"),
                ("MethaneWtPct", "Methane weight percent in natural gas", "درصد وزنی متان در گاز طبیعی", 0.9028, "%", "Chemical composition of natural gas delivered to MSC"),
                ("DollarToRial", "Dollar to Rial exchange rate", "نرخ برابری دلار به ریال", 373005, "Rial", "cbi.ir")
            };

            foreach (var p in parameters)
            {
                _context.GhgParameters.Add(new GhgParameter
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Key = p.key,
                    Name = p.name,
                    FaName = p.fa,
                    Value = p.value,
                    Unit = p.unit,
                    Source = p.source,
                    Year = 1403
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- سوخت‌ها: Settings!B29 ----------
        private async Task SeedFuelsAsync()
        {
            if (await _context.Fuels.AnyAsync()) return;

            var fuels = new (string name, string? fa, double lhv, string lhvUnit, string unit, string source)[]
            {
                ("Natural Gas", "گاز طبیعی", 15, "GJ/kNm3", "kNm3", "Chemical composition of natural gas delivered to MSC"),
                ("Gas oil", "گازوئیل", 0.01, "GJ/Lit", "Lit", "API Compendium 2009"),
                ("Methane", "متان", 5, "GJ/ton", "ton", "engineeringtoolbox.com"),
                ("Butane", "بوتان", 4, "GJ/ton", "ton", "engineeringtoolbox.com"),
                ("Acetylene", "استیلن", 5, "GJ/ton", "ton", "engineeringtoolbox.com"),
                ("Oil", "روغن", 0.0357, "GJ/Lit", "Lit", "API Compendium 2009")
            };

            foreach (var f in fuels)
            {
                _context.Fuels.Add(new Fuel
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Name = f.name,
                    FaName = f.fa,
                    Lhv = f.lhv,
                    LhvUnit = f.lhvUnit,
                    ConsumptionUnit = f.unit,
                    Source = f.source,
                    ValidFromYear = 1403
                });
            }
            await _context.SaveChangesAsync();
        }

        // ---------- ضرایب انتشار: جداول شیت Settings ----------
        private async Task SeedEmissionFactorsAsync()
        {
            if (await _context.EmissionFactors.AnyAsync()) return;
            var y = 1403;
            var list = new List<EmissionFactor>();

            void Add(EmissionFactorCategory cat, string refKey, string? label, string? subKey, string gas, double value, string unit, EmissionStandard std, string? source)
            {
                list.Add(new EmissionFactor
                {
                    Id = Guid.NewGuid(), IsActive = true, Category = cat, RefKey = refKey, RefLabel = label,
                    SubKey = subKey, Gas = gas, Value = value, Unit = unit, Standard = std, Source = source, ValidFromYear = y
                });
            }

            // EF: Fuel Combustion (Settings!B46) - t/GJ
            Add(EmissionFactorCategory.FuelCombustion, "Natural Gas", "گاز طبیعی", null, "CO2", 0.0561, "tCO2/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Natural Gas", "گاز طبیعی", null, "CH4", 1e-06, "tCH4/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Natural Gas", "گاز طبیعی", null, "N2O", 1e-07, "tN2O/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Gas oil", "گازوئیل", null, "CO2", 0.0741, "tCO2/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Gas oil", "گازوئیل", null, "CH4", 3e-06, "tCH4/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Gas oil", "گازوئیل", null, "N2O", 6e-07, "tN2O/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Methane", "متان", null, "CO2", 7, "tCO2/GJ", EmissionStandard.Custom, "Stoichiometry for CO2 EF");
            Add(EmissionFactorCategory.FuelCombustion, "Methane", "متان", null, "CH4", 1e-06, "tCH4/GJ", EmissionStandard.Ipcc2006, "Similarity to NG");
            Add(EmissionFactorCategory.FuelCombustion, "Methane", "متان", null, "N2O", 1e-07, "tN2O/GJ", EmissionStandard.Ipcc2006, "Similarity to NG");
            Add(EmissionFactorCategory.FuelCombustion, "Butane", "بوتان", null, "CO2", 6, "tCO2/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Butane", "بوتان", null, "CH4", 2.8e-06, "tCH4/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Butane", "بوتان", null, "N2O", 5.6e-07, "tN2O/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Acetylene", "استیلن", null, "CO2", 6, "tCO2/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Acetylene", "استیلن", null, "CH4", 0, "tCH4/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Acetylene", "استیلن", null, "N2O", 0, "tN2O/GJ", EmissionStandard.Defra2024, "Emission Factors for GHG Inventories 2014");
            Add(EmissionFactorCategory.FuelCombustion, "Oil", "روغن", null, "CO2", 7, "tCO2/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Oil", "روغن", null, "CH4", 3e-06, "tCH4/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");
            Add(EmissionFactorCategory.FuelCombustion, "Oil", "روغن", null, "N2O", 6e-07, "tN2O/GJ", EmissionStandard.Ipcc2006, "2006 IPCC Guidelines");

            // EF: Electricity Generation gr/kWh (Settings!B110)
            string[][] elec =
            {
                new[] { "Government Average", "میانگین وزارت نیرو", "722.755", "0.017", "0.003" },
                new[] { "Private Average", "میانگین بخش خصوصی", "656.593", "0.013", "0.002" },
                new[] { "Total Average", "میانگین کل کشور", "687.342", "14", "0.002" },
                new[] { "Direct Contract", "خرید برق دوجانبه 1", "619.741", "12", "0.002" },
                new[] { "Solar", "نیروگاه خورشیدی", "0.0803154", "0", "0" },
                new[] { "Wind", "نیروگاه بادی", "0", "0", "0" },
                new[] { "MSC Production", "نیروگاه‌های برق مبارکه", "0", "0", "0" }
            };
            foreach (var e in elec)
            {
                Add(EmissionFactorCategory.ElectricityGeneration, e[0], e[1], null, "CO2", double.Parse(e[2]), "gr/kWh", EmissionStandard.Custom, "ترازنامه انرژی 1401 - وزارت نیرو");
                Add(EmissionFactorCategory.ElectricityGeneration, e[0], e[1], null, "CH4", double.Parse(e[3]), "gr/kWh", EmissionStandard.Custom, "ترازنامه انرژی 1401 - وزارت نیرو");
                Add(EmissionFactorCategory.ElectricityGeneration, e[0], e[1], null, "N2O", double.Parse(e[4]), "gr/kWh", EmissionStandard.Custom, "ترازنامه انرژی 1401 - وزارت نیرو");
            }

            // EF: Employee Commuting t/km (Settings!B61)
            Add(EmissionFactorCategory.EmployeeCommuting, "Car", "خودرو سواری", null, "CO2", 0.0001087399586, "tCO2/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Car", "خودرو سواری", null, "CH4", 3.10685596e-12, "tCH4/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Car", "خودرو سواری", null, "N2O", 1.864113576e-12, "tN2O/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Mini Bus", "مینی‌بوس", null, "CO2", 0.00059340948836, "tCO2/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Mini Bus", "مینی‌بوس", null, "CH4", 1.6155650992e-11, "tCH4/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Mini Bus", "مینی‌بوس", null, "N2O", 1.4291537416e-11, "tN2O/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Bus", "اتوبوس", null, "CO2", 0.000774849876424, "tCO2/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Bus", "اتوبوس", null, "CH4", 6.835083112e-12, "tCH4/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            Add(EmissionFactorCategory.EmployeeCommuting, "Bus", "اتوبوس", null, "N2O", 2.174799172e-11, "tN2O/km", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");

            // EF: Business Travels t/km/person (Settings!B69)
            (string, string, double, double, double)[] travel = {
                ("Car", "خودرو", 0.0001087399586, 3.10685596e-12, 1.864113576e-12),
                ("Bus", "اتوبوس", 4, 0, 1.3048795032e-09),
                ("Air - Short Haul", "پرواز کوتاه", 0.000128623836744, 3.9767756288e-09, 4.1010498672e-09),
                ("Air - Medium Haul", "پرواز متوسط", 8.0156883768e-05, 3.728227152e-10, 2.5476218872e-09),
                ("Air - Long Haul", "پرواز بلند", 0.000101283504296, 3.728227152e-10, 3.2311301984e-09),
                ("Train", "قطار", 7.0214944696e-05, 5.7166149664e-09, 1.6155650992e-09)
            };
            foreach (var t in travel)
            {
                Add(EmissionFactorCategory.BusinessTravel, t.Item1, t.Item2, null, "CO2", t.Item3, "tCO2/km/person", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
                Add(EmissionFactorCategory.BusinessTravel, t.Item1, t.Item2, null, "CH4", t.Item4, "tCH4/km/person", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
                Add(EmissionFactorCategory.BusinessTravel, t.Item1, t.Item2, null, "N2O", t.Item5, "tN2O/km/person", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            }

            // EF: Material Transportation t/km/ton (Settings!B80)
            (string, string, double, double, double)[] mat = {
                ("Road", "جاده‌ای", 0.00011507067476516331, 1.0274167389746723e-09, 3.219239115453973e-09),
                ("Rail", "ریلی", 1.5068778838295193e-05, 1.1644056375046285e-09, 3.424722463248907e-10),
                ("Marine", "دریایی", 5.616544839728209e-05, 2.2329190460382873e-08, 1.438383434564541e-09),
                ("Air", "هوایی", 0.0006198747658480524, 0, 1.9109951344928904e-08)
            };
            foreach (var t in mat)
            {
                Add(EmissionFactorCategory.MaterialTransportation, t.Item1, t.Item2, null, "CO2", t.Item3, "tCO2/km/ton", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
                Add(EmissionFactorCategory.MaterialTransportation, t.Item1, t.Item2, null, "CH4", t.Item4, "tCH4/km/ton", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
                Add(EmissionFactorCategory.MaterialTransportation, t.Item1, t.Item2, null, "N2O", t.Item5, "tN2O/km/ton", EmissionStandard.GhgProtocol, "GHG Protocol Mobile Combustion V2.7");
            }

            // EF: On-Site Transportation t/L (Settings!B89)
            (string, string, double, double, double)[] onsite = {
                ("Petrol", "بنزین", 0.00233955, 2.7333333333333335e-07, 2.252830188679245e-08),
                ("Gas oil", "گازوئیل", 0.00272417, 1.05e-07, 1.06e-07),
                ("CNG", "CNG", 0.00044855, 2.2333333333333335e-08, 7.547169811320755e-10)
            };
            foreach (var t in onsite)
            {
                Add(EmissionFactorCategory.OnSiteTransportation, t.Item1, t.Item2, null, "CO2", t.Item3, "tCO2/L", EmissionStandard.Defra2024, "DEFRA-UK 2024");
                Add(EmissionFactorCategory.OnSiteTransportation, t.Item1, t.Item2, null, "CH4", t.Item4, "tCH4/L", EmissionStandard.Defra2024, "DEFRA-UK 2024");
                Add(EmissionFactorCategory.OnSiteTransportation, t.Item1, t.Item2, null, "N2O", t.Item5, "tN2O/L", EmissionStandard.Defra2024, "DEFRA-UK 2024");
            }

            // EF: Waste Management tCO2e/ton (Settings!B98)
            (string, string, double, double)[] waste = {
                ("Metal", "فلزی", 0.00641061, 0.00888386),
                ("Non-Metal", "غیرفلزی", 0.00098485, 0.01951726),
                ("Plastic", "پلاستیکی", 0.00641061, 0.00888386)
            };
            foreach (var w in waste)
            {
                Add(EmissionFactorCategory.WasteManagement, w.Item1, w.Item2, "Sale", "CO2e", w.Item3, "tCO2e/ton", EmissionStandard.Defra2024, "DEFRA-UK 2024");
                Add(EmissionFactorCategory.WasteManagement, w.Item1, w.Item2, "Landfill", "CO2e", w.Item4, "tCO2e/ton", EmissionStandard.Defra2024, "DEFRA-UK 2024");
            }

            // EF: Energy Related upstream tCO2e (Settings!B124)
            (string, string, string, double, EmissionStandard, string)[] energy = {
                ("Grid Electricity", "برق شبکه", "kWh", 0.0001395, EmissionStandard.Iea2023, "IEA Life Cycle Upstream 2023"),
                ("Natural Gas", "گاز طبیعی", "kNm3", 0.3366, EmissionStandard.Defra2024, "DEFRA-UK 2024"),
                ("Gas oil", "گازوئیل", "Lit", 0.00062665, EmissionStandard.Defra2024, "DEFRA-UK 2024"),
                ("Petrol", "بنزین", "Lit", 0.00060664, EmissionStandard.Defra2024, "DEFRA-UK 2024"),
                ("Methane", "متان", "ton", 0.42316368, EmissionStandard.Defra2024, "DEFRA-UK 2024 (per ton)"),
                ("Butane", "بوتان", "ton", 0.34430947, EmissionStandard.Defra2024, "DEFRA-UK 2024"),
                ("Acetylene", "استیلن", "ton", 0.30295197, EmissionStandard.Defra2024, "Assume like other petroleum gas")
            };
            foreach (var e in energy)
                Add(EmissionFactorCategory.EnergyRelated, e.Item1, e.Item2, null, "CO2e", e.Item4, $"tCO2e/{e.Item3}", e.Item5, e.Item6);

            // EF: Fugitive equipment gr/hr (Settings!B138)
            (string, string, double)[] fug = {
                ("Valves", "شیرها", 4.0369688),
                ("Flanges", "فلنج‌ها", 1.3154168),
                ("PSVs", "شیرهای اطمینان", 104.0086456),
                ("Compressors", "کمپرسورها", 228.02069840000001)
            };
            foreach (var f in fug)
                Add(EmissionFactorCategory.FugitiveEquipment, f.Item1, f.Item2, null, "CH4", f.Item3, "gr/hr", EmissionStandard.Tceq, "TCEQ Equipment Leak Fugitives");

            // EF: Utilities tCO2e (Settings!B148)
            (string, string, string, double)[] utils = {
                ("Industrial Water", "آب صنعتی", "m3", 0.0009311605554735877),
                ("Light Water", "آب سبک", "m3", 0.0028315867747164543),
                ("Saturated Steam", "بخار اشباع", "ton", 0.2),
                ("DM Water", "آب DM", "m3", 0.010561392382622716),
                ("Hot Steam", "بخار داغ", "ton", 0.15762189674642696),
                ("Compressed Air", "هوای فشرده", "Nm3", 0.00010580813208312243),
                ("Oxygen", "اکسیژن", "Nm3", 0.00040242608599150845),
                ("Nitrogen", "نیتروژن", "Nm3", 0.0004022620665563955),
                ("Argon", "آرگون", "Nm3", 0.0004386321781713693),
                ("Mix Electricity", "برق ترکیبی", "kWh", 0.0007522602636133827)
            };
            foreach (var u in utils)
                Add(EmissionFactorCategory.Utility, u.Item1, u.Item2, null, "CO2e", u.Item4, $"tCO2e/{u.Item3}", EmissionStandard.Calculated, "Shall be copied from Tab Utility CF");

            // EF: Vent - process emission factors (شیت Vent)
            Add(EmissionFactorCategory.Wastewater, "Wastewater", "فاضلاب", null, "CH4", 7.198578969235908e-07, "tCH4/m3", EmissionStandard.Ipcc2006, "TOW×Bo×MCF/Flow - IPCC 2006");
            (string, string, string, double, string)[] vents = {
                ("Lime Making", "آهک سازی", "Production", 0.757, "ton/ton"),
                ("EAFs", "کوره‌های قوس", "Production", 0.05522787937732804, "ton/ton"),
                ("Dolomite Making", "دولومیت سازی", "Production", 0.47732, "ton/ton"),
                ("Direct Reduction 1", "احیای مستقیم 1", "GasFeed", 2.01, "ton/kNm3"),
                ("Direct Reduction 2", "احیای مستقیم 2", "GasFeed", 2.01, "ton/kNm3")
            };
            foreach (var v in vents)
            {
                var cat = v.Item3 == "Production" ? "ton CO2/ton product" : "ton CO2/kNm3 gas";
                Add(EmissionFactorCategory.FuelCombustion, v.Item1, v.Item2, v.Item3, "CO2", v.Item4, cat, EmissionStandard.Ipcc2006, "IPCC 2006 / GHG Protocol Iron and Steel 2008");
            }

            // EF: Purchased Materials tCO2e/t (Settings!B287)
            (string, string, double)[] materials = {
                ("Intermediate Product", "محصول میانی", 0),
                ("Natural Gas", "گاز طبیعی (kNm3)", 0.3366),
                ("Aluminium", "آلومینیوم", 17.443233),
                ("Bauxite", "بوکسیت", 0.028963),
                ("Bentonite", "بنتونیت", 0.53435931),
                ("Briquett", "بریکت", 0.0002932),
                ("Calcium aluminate", "آلومینات کلسیم", 3.2347212),
                ("Calcium Carbide", "کربید کلسیم", 4.1287543),
                ("Calcium Silicon", "سیلیکوکلسیم", 10.659982),
                ("Concentrate", "کنسانتره", 0.045920768),
                ("Coke", "کک", 0.224),
                ("Copper", "مس", 5.4339325),
                ("Dolomite", "دولومیت", 0.12350643),
                ("Dolomite stone", "سنگ دولومیت", 0.077324334),
                ("DRI", "آهن اسفنجی", 2.0206583),
                ("Electrode", "الکترود", 0.65),
                ("Ferrochromium", "فروکروم", 5.0669466),
                ("Ferromanganese", "فرومنگنز", 2.8822192),
                ("Ferromolybdenum", "فرومولیبدن", 7.8191103),
                ("Ferroniobium", "فرونیوبیم", 10.612955),
                ("Ferrosilicon", "فروسیلیس", 7.9518282),
                ("Graphite", "گرافیت", 0.036394854),
                ("Iron Ore Crude", "سنگ آهن خام", 0.008958),
                ("Lime", "آهک", 1.046067),
                ("Limestone", "سنگ آهک", 0.003173279),
                ("Nickel", "نیکل", 16.34932),
                ("Oxygen Scavenger", "اسکنژنر اکسیژن", 2.8822192),
                ("Paint", "رنگ", 4.0838399),
                ("Pellet", "گندله", 0.096629016),
                ("Scrap", "قراضه", 0.0035756633),
                ("Steel", "فولاد", 1.400116),
                ("Sodium Silicate", "سیلیکات سدیم", 1.0939892),
                ("Tin", "قلع", 3.9732564),
                ("Zinc", "روی", 2.9592949)
            };
            foreach (var m in materials)
                Add(EmissionFactorCategory.PurchasedMaterial, m.Item1, m.Item2, null, "CO2e", m.Item3, "tCO2e/t", EmissionStandard.Ecoinvent, "IPCC 2021 GWP100a AR6 Ecoinvent V3.11 SimaPro 2025");

            _context.EmissionFactors.AddRange(list);
            await _context.SaveChangesAsync();
        }

        // ---------- فرمول‌ها: استخراج شده از شیت‌های محاسبه و EMSPR ----------
        private async Task SeedFormulasAsync()
        {
            if (await _context.CalculationFormulas.AnyAsync()) return;

            var formulas = new List<CalculationFormula>();

            void Add(string code, string name, string fa, EmissionFactorCategory? cat, string expression, string variablesJson, string outputUnit, EmissionStandard std, string reference)
            {
                formulas.Add(new CalculationFormula
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Code = code,
                    Name = name,
                    FaName = fa,
                    Category = cat,
                    Expression = expression,
                    VariablesJson = variablesJson,
                    OutputUnit = outputUnit,
                    Standard = std,
                    Reference = reference,
                    IsEnabled = true,
                    Version = 1
                });
            }

            // احتراق - رابطه (1) بند 6.3.2 دستورالعمل EMSPR: انتشار = مصرف × LHV × EF
            Add("GHG-COMBUSTION-GAS", "Fuel Combustion Emission per Gas", "انتشار احتراق سوخت به تفکیک گاز",
                EmissionFactorCategory.FuelCombustion,
                "Consumption * LHV * EF",
                """[{"name":"Consumption","label":"مصرف سالانه سوخت","type":1,"unit":"unit of fuel"},{"name":"LHV","label":"ارزش حرارتی پایین سوخت","type":2},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"Natural Gas","gas":"CO2"}]""",
                "tGas/y", EmissionStandard.Iso14064, "EMSPR 6.3.2 Eq(1) - Combustion sheet");

            // معادل CO2 - رابطه (2): CO2e = CO2 + CH4×GWP + N2O×GWP
            Add("GHG-COMBUSTION-CO2E", "CO2 Equivalent of Combustion", "معادل دی‌اکسید کربن احتراق",
                EmissionFactorCategory.FuelCombustion,
                "Co2 + Ch4 * GwpCH4 + N2o * GwpN2O",
                """[{"name":"Co2","label":"انتشار CO2","type":6},{"name":"Ch4","label":"انتشار CH4","type":6},{"name":"N2o","label":"انتشار N2O","type":6},{"name":"GwpCH4","label":"GWP متان","type":5,"refKey":"CH4"},{"name":"GwpN2O","label":"GWP نیتروس اکسید","type":5,"refKey":"N2O"}]""",
                "tCO2e/y", EmissionStandard.Iso14064, "EMSPR 6.3.2 Eq(2)");

            // برق - رابطه (5): E = kWh × EF(gr/kWh) / 10^6
            Add("GHG-ELECTRICITY", "Electricity Emission", "انتشار مصرف برق",
                EmissionFactorCategory.ElectricityGeneration,
                "Consumption * EF / 1000000",
                """[{"name":"Consumption","label":"مصرف برق","type":1,"unit":"kWh/y"},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"Total Average","gas":"CO2","unit":"gr/kWh"}]""",
                "tGas/y", EmissionStandard.Iso14064, "EMSPR 6.3.6 Eq(5) - Electricity sheet");

            // فرار - رابطه (6): E = Σ(count×EF) × (1-η) × CH4% × Hours / 10^6
            Add("GHG-FUGITIVE-EQUIPMENT", "Equipment Leak Fugitive Emission", "انتشار فرار نشتی تجهیزات",
                EmissionFactorCategory.FugitiveEquipment,
                "(Valves * EfValves + Flanges * EfFlanges + Psvs * EfPsvs) * (1 - ControlEfficiency) * MethaneWtPct * WorkingHours / 1000000",
                """[{"name":"Valves","label":"تعداد شیرها","type":1},{"name":"Flanges","label":"تعداد فلنج‌ها","type":1},{"name":"Psvs","label":"تعداد PSV","type":1},{"name":"ControlEfficiency","label":"راندمان کنترل (0-1)","type":1},{"name":"EfValves","label":"EF شیرها","type":3,"refKey":"Valves","gas":"CH4"},{"name":"EfFlanges","label":"EF فلنج‌ها","type":3,"refKey":"Flanges","gas":"CH4"},{"name":"EfPsvs","label":"EF PSV","type":3,"refKey":"PSVs","gas":"CH4"},{"name":"MethaneWtPct","label":"درصد وزنی متان","type":4},{"name":"WorkingHours","label":"ساعت کاری سالانه","type":4}]""",
                "tCH4/y", EmissionStandard.Tceq, "EMSPR 6.3.8 Eq(6) - Fugitive sheet");

            // پسماند: E = مقدار × EF
            Add("GHG-WASTE", "Waste Management Emission", "انتشار مدیریت پسماند",
                EmissionFactorCategory.WasteManagement,
                "Amount * EF",
                """[{"name":"Amount","label":"مقدار پسماند","type":1,"unit":"ton"},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"Metal","gas":"CO2e"}]""",
                "tCO2e/y", EmissionStandard.Defra2024, "Waste sheet");

            // حمل و نقل مواد: E = وزن × مسافت × EF
            Add("GHG-TRANSPORT-MATERIAL", "Material Transportation Emission", "انتشار حمل و نقل مواد",
                EmissionFactorCategory.MaterialTransportation,
                "Weight * Distance * EF",
                """[{"name":"Weight","label":"وزن بار","type":1,"unit":"ton"},{"name":"Distance","label":"مسافت","type":1,"unit":"km"},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"Road","gas":"CO2"}]""",
                "tGas/y", EmissionStandard.GhgProtocol, "Upstream/Downstream Transportation sheets");

            // رفت و آمد پرسنل / سفر کاری: E = مسافت × افراد × EF
            Add("GHG-COMMUTING", "Commuting and Business Travel Emission", "انتشار رفت و آمد پرسنل و سفرهای کاری",
                EmissionFactorCategory.EmployeeCommuting,
                "Distance * Persons * EF",
                """[{"name":"Distance","label":"مسافت","type":1,"unit":"km"},{"name":"Persons","label":"تعداد افراد/سفر","type":1,"unit":"person"},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"Car","gas":"CO2"}]""",
                "tGas/y", EmissionStandard.GhgProtocol, "Commuting/Business Travel sheets");

            // مواد خریداری شده: E = مقدار × EF
            Add("GHG-PURCHASED-MATERIAL", "Purchased Material Emission", "انتشار مواد و خدمات خریداری شده",
                EmissionFactorCategory.PurchasedMaterial,
                "Amount * EF",
                """[{"name":"Amount","label":"مقدار","type":1,"unit":"ton"},{"name":"EF","label":"ضریب انتشار","type":3,"refKey":"DRI","gas":"CO2e"}]""",
                "tCO2e/y", EmissionStandard.Ecoinvent, "Purchased Material sheet");

            // فاضلاب: CH4 = TOW × Bo × MCF (IPCC)
            Add("GHG-WASTEWATER-CH4", "Wastewater Methane Emission", "انتشار متان تصفیه فاضلاب",
                EmissionFactorCategory.Wastewater,
                "TOW * Bo * MCF",
                """[{"name":"TOW","label":"COD ورودی (kgCOD/y)","type":1},{"name":"Bo","label":"حداکثر تولید متان (0.25)","type":4},{"name":"MCF","label":"ضریب تصحیح متان (0.1)","type":4}]""",
                "kgCH4/y", EmissionStandard.Ipcc2006, "IPCC 2006 - Wastewater sheet");

            // فاضلاب - ضریب به ازای مترمکعب: EF = CH4 / Flow
            Add("GHG-WASTEWATER-EF", "Wastewater Emission per m3", "انتشار فاضلاب به ازای مترمکعب",
                EmissionFactorCategory.Wastewater,
                "Ch4Emission / Flow",
                """[{"name":"Ch4Emission","label":"انتشار متان (tCH4/y)","type":6},{"name":"Flow","label":"دبی سالانه (m3/y)","type":1}]""",
                "tCH4/m3", EmissionStandard.Ipcc2006, "Wastewater sheet");

            _context.CalculationFormulas.AddRange(formulas);
            await _context.SaveChangesAsync();
        }

        // ---------- دوره‌ها ----------
        private async Task SeedPeriodsAsync()
        {
            if (await _context.GhgPeriods.AnyAsync()) return;

            _context.GhgPeriods.Add(new GhgPeriod
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                PersianYear = 1403,
                GregorianYear = 2024,
                Title = "دوره گزارش‌دهی سال 1403 (ورود داده اولیه از فایل‌های مرجع)",
                Status = GhgPeriodStatus.Draft,
                Description = "داده‌های اولیه از ورک‌بوک MSC-GHG Atlas و بسته فایل‌های داده (FilePackage) استخراج شده است."
            });
            await _context.SaveChangesAsync();
        }
    }
}
