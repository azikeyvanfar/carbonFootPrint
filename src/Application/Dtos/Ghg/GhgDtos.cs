using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Application.Dtos.Ghg
{
    // ---------- Reference data DTOs ----------

    public class GhgAreaDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public string FaName { get; set; } = null!;
        public string? CommitteeCode { get; set; }
        public int? EmployeeCount { get; set; }
        public int? ContractorCount { get; set; }
    }

    public class CostCenterDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public string? UnitProcess { get; set; }
        public Guid? AreaId { get; set; }
        public string? AreaName { get; set; }
    }

    public class FuelDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; } = null!;
        public string? FaName { get; set; }
        public double Lhv { get; set; }
        public string LhvUnit { get; set; } = null!;
        public string ConsumptionUnit { get; set; } = null!;
        public string? Source { get; set; }
        public int ValidFromYear { get; set; }
    }

    public class EmissionCategoryDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; } = null!;
        public string FaName { get; set; } = null!;
        public string Sign { get; set; } = null!;
        public int CategoryNo { get; set; }
        public int Scope { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
    }

    public class GhgParameterDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? FaName { get; set; }
        public double Value { get; set; }
        public string? Unit { get; set; }
        public string? Source { get; set; }
        public int Year { get; set; }
    }

    public class AddGhgParameterDto
    {
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? FaName { get; set; }
        public double Value { get; set; }
        public string? Unit { get; set; }
        public string? Source { get; set; }
        public int Year { get; set; }
    }

    public class UpdateGhgParameterDto : AddGhgParameterDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class GlobalWarmingPotentialDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string GasKey { get; set; } = null!;
        public double Value { get; set; }
        public string AssessmentReport { get; set; } = null!;
        public int TimeHorizon { get; set; }
        public int ValidFromYear { get; set; }
    }

    // ---------- Period / activity data DTOs ----------

    public class GhgPeriodDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public int PersianYear { get; set; }
        public int GregorianYear { get; set; }
        public string Title { get; set; } = null!;
        public GhgPeriodStatus Status { get; set; }
        public string? Description { get; set; }
    }

    public class ActivityDataEntryDto
    {
        public Guid Id { get; set; }
        public Guid PeriodId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public Guid? AreaId { get; set; }
        public string? AreaName { get; set; }
        public Guid? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public Guid? FuelId { get; set; }
        public string? FuelName { get; set; }
        public string? FactorRefKey { get; set; }
        public string? FactorSubKey { get; set; }
        public string? EmissionSource { get; set; }
        public double Quantity { get; set; }
        public double? Quantity2 { get; set; }
        public double? Quantity3 { get; set; }
        public string? Unit { get; set; }
        public double? ControlEfficiency { get; set; }
        public ActivityDataSource DataSource { get; set; }
        public string? Description { get; set; }
    }

    public class AddActivityDataEntryDto
    {
        public Guid PeriodId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? AreaId { get; set; }
        public Guid? CostCenterId { get; set; }
        public Guid? FuelId { get; set; }
        public string? FactorRefKey { get; set; }
        public string? FactorSubKey { get; set; }
        public string? EmissionSource { get; set; }
        public double Quantity { get; set; }
        public double? Quantity2 { get; set; }
        public double? Quantity3 { get; set; }
        public string? Unit { get; set; }
        public double? ControlEfficiency { get; set; }
        public ActivityDataSource DataSource { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateActivityDataEntryDto : AddActivityDataEntryDto
    {
        public Guid Id { get; set; }
    }

    // ---------- Preview / calculation DTOs ----------

    public class ActivityPreviewRequestDto
    {
        public Guid? FormulaCode { get; set; }
        public string Code { get; set; } = null!;
        public Dictionary<string, double> Values { get; set; } = new();
    }

    public class ActivityPreviewDto
    {
        public bool IsSuccess { get; set; }
        public double? Result { get; set; }
        public string? Error { get; set; }
        public string? Expression { get; set; }
        public string? OutputUnit { get; set; }
    }

    public class GhgCalculationSummaryDto
    {
        public Guid PeriodId { get; set; }
        public int PersianYear { get; set; }
        public double TotalCo2e { get; set; }
        public List<EmissionByAreaDto> ByArea { get; set; } = new();
        public List<EmissionByCategoryDto> ByCategory { get; set; } = new();
        public List<EmissionByScopeDto> ByScope { get; set; } = new();
        public List<ProductFootprintDto> Products { get; set; } = new();
    }

    public class EmissionByAreaDto
    {
        public string AreaName { get; set; } = null!;
        public string? AreaFaName { get; set; }
        public int AreaCode { get; set; }
        public double Co2e { get; set; }
        public double SharePct { get; set; }
    }

    public class EmissionByCategoryDto
    {
        public string CategoryName { get; set; } = null!;
        public string? CategoryFaName { get; set; }
        public string Sign { get; set; } = null!;
        public int Scope { get; set; }
        public int CategoryNo { get; set; }
        public double Co2e { get; set; }
        public double SharePct { get; set; }
    }

    public class EmissionByScopeDto
    {
        public int Scope { get; set; }
        public double Co2e { get; set; }
        public double SharePct { get; set; }
    }

    public class ProductFootprintDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? FaName { get; set; }
        public string? AreaName { get; set; }
        public double CarbonFootprint { get; set; }
        public double? UpstreamSharePct { get; set; }
        public double? AnnualProduction { get; set; }
        public string? Boundary { get; set; }
        public string? Standard { get; set; }
    }

    // ---------- Wizard (step-by-step form) DTOs ----------

    /// <summary>
    /// ساختار فرم گام‌به‌گام ورود داده - هر گام یک دسته انتشار با فیلدهای داینامیک حاصل از فرمول‌ها
    /// </summary>
    public class ActivityWizardDto
    {
        public List<WizardStepDto> Steps { get; set; } = new();
    }

    public class WizardStepDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string FaName { get; set; } = null!;
        public string Sign { get; set; } = null!;
        public int Scope { get; set; }
        public int Priority { get; set; }
        public string? Description { get; set; }
        public List<WizardFieldDto> Fields { get; set; } = new();
    }

    public class WizardFieldDto
    {
        public string Name { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string? Unit { get; set; }
        public string Type { get; set; } = "number";
        public bool IsRequired { get; set; }
        public string? RefKey { get; set; }
        public string? Gas { get; set; }
        public string? HelpText { get; set; }
    }
}
