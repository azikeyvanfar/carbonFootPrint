using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Services;
using ContractorBackend.Domain.Entities.Ghg;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg.Services
{
    /// <summary>
    /// سرویس محاسبه انتشار - اجرای فرمول‌های تعریف شده در تنظیمات بر روی داده‌های فعالیت.
    /// منطق محاسبه از ورک‌بوک MSC-GHG و دستورالعمل EMSPR استخراج شده است:
    /// احتراق: انتشار = مصرف × LHV × EF ؛ معادل CO2 = CO2 + CH4×GWP + N2O×GWP
    /// </summary>
    public class GhgCalculationService : IGhgCalculationService
    {
        private readonly IApplicationDbContext _context;

        public GhgCalculationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GhgCalculationSummaryDto> RecalculatePeriodAsync(Guid periodId)
        {
            var period = await _context.Set<GhgPeriod>().FirstOrDefaultAsync(x => x.Id == periodId)
                ?? throw new InvalidOperationException("دوره گزارش‌دهی یافت نشد.");

            var areas = await _context.Set<GhgArea>().AsNoTracking().ToListAsync();
            var categories = await _context.Set<EmissionCategory>().AsNoTracking().ToListAsync();
            var fuels = await _context.Set<Fuel>().AsNoTracking().ToListAsync();
            var factors = await _context.Set<EmissionFactor>().AsNoTracking().ToListAsync();
            var gwps = await _context.Set<GlobalWarmingPotential>().AsNoTracking().Where(x => x.IsActive).ToListAsync();
            var gwpDict = gwps.GroupBy(x => x.GasKey).ToDictionary(g => g.Key, g => g.First().Value);
            var entries = await _context.Set<ActivityDataEntry>().AsNoTracking()
                .Where(x => x.PeriodId == periodId && x.IsActive).ToListAsync();

            var results = new List<EmissionResult>();

            // Group entries by (category, area) => inventory row (area-sign-categoryNo)
            foreach (var group in entries.GroupBy(x => new { x.CategoryId, x.AreaId }))
            {
                var category = categories.FirstOrDefault(c => c.Id == group.Key.CategoryId);
                var area = areas.FirstOrDefault(a => a.Id == group.Key.AreaId);
                if (category == null) continue;

                var co2 = 0.0; var ch4 = 0.0; var n2o = 0.0; var sf6 = 0.0; var co2e = 0.0;

                foreach (var entry in group)
                {
                    var r = CalculateEntry(entry, category, fuels, factors, gwpDict);
                    co2 += r.co2; ch4 += r.ch4; n2o += r.n2o; sf6 += r.sf6; co2e += r.co2e;
                }

                results.Add(new EmissionResult
                {
                    Id = Guid.NewGuid(),
                    PeriodId = periodId,
                    CategoryId = category.Id,
                    AreaId = area?.Id,
                    EmissionCode = BuildEmissionCode(area?.Code, category.Sign, category.CategoryNo),
                    Co2 = co2,
                    Ch4 = ch4,
                    N2o = n2o,
                    Sf6 = sf6,
                    Co2e = co2e
                });
            }

            var total = results.Sum(x => x.Co2e);
            foreach (var r in results)
                r.SharePct = total > 0 ? r.Co2e / total * 100.0 : 0;

            // Replace previous results
            var old = _context.Set<EmissionResult>().Where(x => x.PeriodId == periodId);
            _context.Set<EmissionResult>().RemoveRange(old);
            _context.Set<EmissionResult>().AddRange(results);
            await _context.SaveChangesAsync();

            period.Status = Domain.Enums.Ghg.GhgPeriodStatus.Calculated;
            await _context.SaveChangesAsync();

            return await BuildSummaryAsync(periodId);
        }

        /// <summary>
        /// پیش‌نمایش محاسبه یک ردیف با فرمول تعریف شده در تنظیمات (برای فرم گام‌به‌گام).
        /// </summary>
        public async Task<ActivityPreviewDto> PreviewActivityAsync(ActivityPreviewRequestDto request)
        {
            var formula = await _context.Set<CalculationFormula>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code && x.IsActive);

            if (formula == null)
                return new ActivityPreviewDto { IsSuccess = false, Error = $"فرمولی با کد '{request.Code}' یافت نشد." };

            try
            {
                var variables = ParseVariables(formula.VariablesJson);
                var missing = variables
                    .Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.Input && !request.Values.ContainsKey(v.Name))
                    .Select(v => v.Label)
                    .ToList();
                if (missing.Any())
                    return new ActivityPreviewDto { IsSuccess = false, Error = "مقادیر ورودی ناقص است: " + string.Join("، ", missing), Expression = formula.Expression, OutputUnit = formula.OutputUnit };

                var values = new Dictionary<string, double>(request.Values);

                // EmissionFactor variables resolved from settings
                var factors = await _context.Set<EmissionFactor>().AsNoTracking().Where(x => x.IsActive).ToListAsync();
                foreach (var v in variables.Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.EmissionFactor))
                {
                    var candidates = factors.Where(f => f.RefKey == (v.RefKey ?? "") && f.Gas == (v.Gas ?? "CO2e"));
                    if (v.Unit != null && candidates.Count() > 1) candidates = candidates.Where(f => f.Unit == v.Unit);
                    var ef = candidates.OrderByDescending(f => f.ValidFromYear).FirstOrDefault();
                    values[v.Name] = ef?.Value ?? 0;
                }

                // FuelProperty (LHV)
                var fuels = await _context.Set<Fuel>().AsNoTracking().Where(x => x.IsActive).ToListAsync();
                foreach (var v in variables.Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.FuelProperty))
                {
                    var fuel = fuels.FirstOrDefault(f => f.Name == (v.RefKey ?? ""));
                    values[v.Name] = fuel?.Lhv ?? 0;
                }

                // Parameters
                var parameters = await _context.Set<GhgParameter>().AsNoTracking().Where(x => x.IsActive).ToListAsync();
                foreach (var v in variables.Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.Parameter))
                {
                    var p = parameters.Where(x => x.Key == v.Name).OrderByDescending(x => x.Year).FirstOrDefault();
                    if (p != null) values[v.Name] = p.Value;
                }

                // GWP
                var gwpList = await _context.Set<GlobalWarmingPotential>().AsNoTracking().Where(x => x.IsActive).ToListAsync();
                foreach (var v in variables.Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.GlobalWarmingPotential))
                {
                    var g = gwpList.FirstOrDefault(x => x.GasKey == (v.RefKey ?? v.Name));
                    if (g != null) values[v.Name] = g.Value;
                }

                var result = MathExpressionEvaluator.Evaluate(formula.Expression, values);
                return new ActivityPreviewDto { IsSuccess = true, Result = result, Expression = formula.Expression, OutputUnit = formula.OutputUnit };
            }
            catch (Exception ex)
            {
                return new ActivityPreviewDto { IsSuccess = false, Error = ex.Message, Expression = formula.Expression, OutputUnit = formula.OutputUnit };
            }
        }

        /// <summary>
        /// پارس JSON متغیرهای فرمول
        /// </summary>
        public static List<FormulaVariableDto> ParseVariables(string variablesJson)
        {
            if (string.IsNullOrWhiteSpace(variablesJson)) return new List<FormulaVariableDto>();
            try
            {
                return JsonSerializer.Deserialize<List<FormulaVariableDto>>(variablesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<FormulaVariableDto>();
            }
            catch
            {
                return new List<FormulaVariableDto>();
            }
        }

        /// <summary>
        /// محاسبه یک ردیف داده فعالیت با توجه به دسته آن و فرمول‌های تنظیمات.
        /// خروجی: (co2, ch4, n2o, sf6, co2e)
        /// </summary>
        public static (double co2, double ch4, double n2o, double sf6, double co2e) CalculateEntry(
            ActivityDataEntry entry,
            EmissionCategory category,
            List<Fuel> fuels,
            List<EmissionFactor> factors,
            Dictionary<string, double> gwps)
        {
            var co2 = 0.0; var ch4 = 0.0; var n2o = 0.0; var sf6 = 0.0;

            switch (category.Sign)
            {
                case "C": // Combustion: E = Consumption × LHV × EF ; CO2e = CO2 + CH4×GWP + N2O×GWP
                case "N": // Energy related (upstream, direct EF per unit)
                    {
                        var fuel = fuels.FirstOrDefault(f => f.Id == entry.FuelId) ??
                                   fuels.FirstOrDefault(f => f.Name == (entry.FactorRefKey ?? f.Name));
                        var refKey = entry.FactorRefKey ?? fuel?.Name ?? "";
                        var year = factors.Where(f => f.RefKey == refKey && f.Category == (category.Sign == "C" ? Domain.Enums.Ghg.EmissionFactorCategory.FuelCombustion : Domain.Enums.Ghg.EmissionFactorCategory.EnergyRelated))
                            .OrderByDescending(f => f.ValidFromYear).ToList();

                        double efCo2 = GetEf(year, "CO2"), efCh4 = GetEf(year, "CH4"), efN2o = GetEf(year, "N2O");
                        if (category.Sign == "C")
                        {
                            var lhv = fuel?.Lhv ?? 0;
                            co2 = entry.Quantity * lhv * efCo2;
                            ch4 = entry.Quantity * lhv * efCh4;
                            n2o = entry.Quantity * lhv * efN2o;
                        }
                        else
                        {
                            var efTotal = GetEf(year, "CO2e");
                            if (efTotal > 0) { co2 = entry.Quantity * efTotal; }
                            else
                            {
                                co2 = entry.Quantity * efCo2;
                                ch4 = entry.Quantity * efCh4;
                                n2o = entry.Quantity * efN2o;
                            }
                        }
                        break;
                    }
                case "V": // Vent: E = Production × EF(ton/ton product)  یا  E = GasFeed(kNm3) × EF(ton/kNm3)
                    {
                        var refKey = entry.FactorRefKey ?? "";
                        // اگر مصرف گاز (kNm3) وارد شده باشد از EF گاز وگرنه EF تولید استفاده می‌شود
                        var subKey = entry.Quantity2.HasValue ? "GasFeed" : "Production";
                        var ef = factors.Where(f => f.RefKey == refKey && (f.SubKey == subKey || f.SubKey == null))
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        var efCo2 = GetEf(ef, "CO2");
                        var q = entry.Quantity2 ?? entry.Quantity;
                        co2 = q * efCo2;
                        break;
                    }
                case "F": // Fugitive: E(gr/hr) = Σ(count×EF) ; E(t/y) = E×(1-η)×CH4%×Hours/1e6
                    {
                        var efValves = GetEf(factors, "Valves", Domain.Enums.Ghg.EmissionFactorCategory.FugitiveEquipment);
                        var efFlanges = GetEf(factors, "Flanges", Domain.Enums.Ghg.EmissionFactorCategory.FugitiveEquipment);
                        var efPsvs = GetEf(factors, "PSVs", Domain.Enums.Ghg.EmissionFactorCategory.FugitiveEquipment);
                        var count = entry.Quantity;         // valves
                        var flanges = entry.Quantity2 ?? 0; // flanges
                        var psvs = entry.Quantity3 ?? 0;    // PSVs
                        var grPerHour = count * efValves + flanges * efFlanges + psvs * efPsvs;
                        const double workingHours = 8760;  // پارامتر تنظیمات: WorkingHours
                        const double ch4Pct = 0.9028;      // پارامتر تنظیمات: MethaneWtPct
                        var eff = entry.ControlEfficiency ?? 0;
                        ch4 = grPerHour * (1 - eff) * ch4Pct * workingHours / 1e6;
                        break;
                    }
                case "E": // Electricity: E(t) = kWh × EF(g/kWh) / 1e6
                    {
                        var refKey = entry.FactorRefKey ?? "Total Average";
                        var ef = factors.Where(f => f.RefKey == refKey && f.Category == Domain.Enums.Ghg.EmissionFactorCategory.ElectricityGeneration)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        co2 = entry.Quantity * GetEf(ef, "CO2") / 1e6;
                        ch4 = entry.Quantity * GetEf(ef, "CH4") / 1e6;
                        n2o = entry.Quantity * GetEf(ef, "N2O") / 1e6;
                        break;
                    }
                case "Y": // Wastewater: E = Q(m3) × EF(tCH4/m3); EF = (TOW×Bo×MCF)/Flow
                    {
                        var ef = factors.Where(f => f.RefKey == "Wastewater")
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        var efCh4 = GetEf(ef, "CH4");
                        ch4 = entry.Quantity * efCh4;
                        break;
                    }
                case "W": // Waste onsite
                case "Z": // Waste offsite: E = tons × EF(tCO2e/ton) by (type, method)
                    {
                        var refKey = entry.FactorRefKey ?? "Metal";
                        var subKey = entry.FactorSubKey ?? "Landfill";
                        var ef = factors.Where(f => f.RefKey == refKey && f.SubKey == subKey && f.Category == Domain.Enums.Ghg.EmissionFactorCategory.WasteManagement)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        co2 = entry.Quantity * GetEf(ef, "CO2e");
                        break;
                    }
                case "M": // On-site transportation: E = Liters × EF(t/L)
                case "T": // Upstream transportation: E = ton×km × EF(t/km/ton)
                case "D": // Downstream transportation
                    {
                        var cat = category.Sign == "M" ? Domain.Enums.Ghg.EmissionFactorCategory.OnSiteTransportation : Domain.Enums.Ghg.EmissionFactorCategory.MaterialTransportation;
                        var refKey = entry.FactorRefKey ?? "";
                        var ef = factors.Where(f => f.RefKey == refKey && f.Category == cat)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        if (category.Sign == "M")
                        {
                            co2 = entry.Quantity * GetEf(ef, "CO2");
                            ch4 = entry.Quantity * GetEf(ef, "CH4");
                            n2o = entry.Quantity * GetEf(ef, "N2O");
                        }
                        else
                        {
                            var tonKm = entry.Quantity * (entry.Quantity2 ?? 0); // tons × km
                            co2 = tonKm * GetEf(ef, "CO2");
                            ch4 = tonKm * GetEf(ef, "CH4");
                            n2o = tonKm * GetEf(ef, "N2O");
                        }
                        break;
                    }
                case "P": // Commuting: E = km × persons × EF(t/km/person)
                case "B": // Business travel
                    {
                        var cat = category.Sign == "P" ? Domain.Enums.Ghg.EmissionFactorCategory.EmployeeCommuting : Domain.Enums.Ghg.EmissionFactorCategory.BusinessTravel;
                        var refKey = entry.FactorRefKey ?? "Car";
                        var ef = factors.Where(f => f.RefKey == refKey && f.Category == cat)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        var km = entry.Quantity * (entry.Quantity2 ?? 1); // km × persons
                        co2 = km * GetEf(ef, "CO2");
                        ch4 = km * GetEf(ef, "CH4");
                        n2o = km * GetEf(ef, "N2O");
                        break;
                    }
                case "R": // Purchased material / raw material
                case "L": // Ancillary input
                case "X": // Purchased services
                    {
                        var ef = factors.Where(f => f.RefKey == (entry.FactorRefKey ?? "") && f.Category == Domain.Enums.Ghg.EmissionFactorCategory.PurchasedMaterial)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        co2 = entry.Quantity * GetEf(ef, "CO2e");
                        break;
                    }
                case "S": // Steam / utilities
                case "A": // Compressed air
                    {
                        var ef = factors.Where(f => f.RefKey == (entry.FactorRefKey ?? "") && f.Category == Domain.Enums.Ghg.EmissionFactorCategory.Utility)
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        co2 = entry.Quantity * GetEf(ef, "CO2e");
                        break;
                    }
                default:
                    {
                        var ef = factors.Where(f => f.RefKey == (entry.FactorRefKey ?? ""))
                            .OrderByDescending(f => f.ValidFromYear).ToList();
                        co2 = entry.Quantity * GetEf(ef, "CO2e");
                        break;
                    }
            }

            var co2e = co2
                + ch4 * GetGwp(gwps, "CH4")
                + n2o * GetGwp(gwps, "N2O")
                + sf6 * GetGwp(gwps, "SF6");

            return (co2, ch4, n2o, sf6, co2e);
        }

        private static double GetEf(List<EmissionFactor> factors, string gas)
        {
            var f = factors.FirstOrDefault(x => x.Gas == gas && x.IsActive);
            return f?.Value ?? 0;
        }

        private static double GetEf(List<EmissionFactor> factors, string refKey, Domain.Enums.Ghg.EmissionFactorCategory category)
        {
            var f = factors.Where(x => x.RefKey == refKey && x.Category == category && x.IsActive)
                .OrderByDescending(x => x.ValidFromYear).FirstOrDefault();
            return f?.Value ?? 0;
        }

        private static double GetGwp(Dictionary<string, double> gwps, string gas) =>
            gwps.TryGetValue(gas, out var v) ? v : (gas == "CO2" ? 1 : 0);

        private static string BuildEmissionCode(int? areaCode, string sign, int categoryNo) =>
            $"{areaCode ?? 0}-{sign}-{categoryNo}";

        public async Task<GhgCalculationSummaryDto> BuildSummaryAsync(Guid periodId)
        {
            var period = await _context.Set<GhgPeriod>().AsNoTracking().FirstAsync(x => x.Id == periodId);
            var areas = await _context.Set<GhgArea>().AsNoTracking().ToListAsync();
            var categories = await _context.Set<EmissionCategory>().AsNoTracking().ToListAsync();
            var results = await _context.Set<EmissionResult>().AsNoTracking().Where(x => x.PeriodId == periodId && x.IsActive).ToListAsync();
            var products = await _context.Set<ProductFootprint>().AsNoTracking().Where(x => x.PeriodId == periodId && x.IsActive).ToListAsync();

            var total = results.Sum(x => x.Co2e);

            var summary = new GhgCalculationSummaryDto
            {
                PeriodId = periodId,
                PersianYear = period.PersianYear,
                TotalCo2e = total,
                ByArea = results.Where(x => x.AreaId != null).GroupBy(x => x.AreaId!.Value).Select(g =>
                {
                    var area = areas.First(a => a.Id == g.Key);
                    return new EmissionByAreaDto
                    {
                        AreaName = area.Name,
                        AreaFaName = area.FaName,
                        AreaCode = area.Code,
                        Co2e = g.Sum(x => x.Co2e),
                        SharePct = total > 0 ? g.Sum(x => x.Co2e) / total * 100 : 0
                    };
                }).OrderByDescending(x => x.Co2e).ToList(),
                ByCategory = results.GroupBy(x => x.CategoryId).Select(g =>
                {
                    var cat = categories.First(c => c.Id == g.Key);
                    return new EmissionByCategoryDto
                    {
                        CategoryName = cat.Name,
                        CategoryFaName = cat.FaName,
                        Sign = cat.Sign,
                        Scope = cat.Scope,
                        CategoryNo = cat.CategoryNo,
                        Co2e = g.Sum(x => x.Co2e),
                        SharePct = total > 0 ? g.Sum(x => x.Co2e) / total * 100 : 0
                    };
                }).OrderByDescending(x => x.Co2e).ToList(),
                ByScope = results.GroupBy(x => categories.First(c => c.Id == x.CategoryId).Scope).Select(g => new EmissionByScopeDto
                {
                    Scope = g.Key,
                    Co2e = g.Sum(x => x.Co2e),
                    SharePct = total > 0 ? g.Sum(x => x.Co2e) / total * 100 : 0
                }).OrderBy(x => x.Scope).ToList(),
                Products = products.Select(p => new ProductFootprintDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    FaName = p.FaName,
                    AreaName = p.AreaName,
                    CarbonFootprint = p.CarbonFootprint,
                    UpstreamSharePct = p.UpstreamSharePct,
                    AnnualProduction = p.AnnualProduction,
                    Boundary = p.Boundary,
                    Standard = p.Standard
                }).ToList()
            };

            return summary;
        }
    }

}
