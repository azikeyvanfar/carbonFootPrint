using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Application.Dtos.Ghg
{
    /// <summary>
    /// متغیر فرمول - مبنای تولید پویای فرم ورود داده
    /// </summary>
    public class FormulaVariableDto
    {
        /// <summary>نام متغیر در عبارت فرمول</summary>
        public string Name { get; set; } = null!;
        /// <summary>برچسب نمایشی</summary>
        public string Label { get; set; } = null!;
        /// <summary>نوع متغیر</summary>
        public FormulaVariableType Type { get; set; }
        /// <summary>واحد</summary>
        public string? Unit { get; set; }
        /// <summary>کلید مرجع برای اتصال خودکار به ضریب/سوخت/پارامتر</summary>
        public string? RefKey { get; set; }
        /// <summary>گاز (برای ضرایب چندگانه)</summary>
        public string? Gas { get; set; }
        /// <summary>مقدار پیش‌فرض آزمایشی</summary>
        public double? SampleValue { get; set; }
        /// <summary>الزامی بودن</summary>
        public bool IsRequired { get; set; } = true;
    }

    public class CalculationFormulaDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FaName { get; set; } = null!;
        public EmissionFactorCategory? Category { get; set; }
        public string Expression { get; set; } = null!;
        public List<FormulaVariableDto> Variables { get; set; } = new();
        public string OutputUnit { get; set; } = null!;
        public EmissionStandard Standard { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public bool IsEnabled { get; set; }
        public int Version { get; set; }
    }

    public class AddCalculationFormulaDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FaName { get; set; } = null!;
        public EmissionFactorCategory? Category { get; set; }
        public string Expression { get; set; } = null!;
        public List<FormulaVariableDto> Variables { get; set; } = new();
        public string OutputUnit { get; set; } = null!;
        public EmissionStandard Standard { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int Version { get; set; } = 1;
    }

    public class UpdateCalculationFormulaDto : AddCalculationFormulaDto
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// درخواست آزمایش فرمول با مقادیر نمونه
    /// </summary>
    public class TestFormulaRequestDto
    {
        public Guid? FormulaId { get; set; }
        public string? Code { get; set; }
        public string? Expression { get; set; }
        public Dictionary<string, double> Values { get; set; } = new();
    }

    public class TestFormulaResultDto
    {
        public bool IsSuccess { get; set; }
        public double? Result { get; set; }
        public string? Error { get; set; }
        public string? Expression { get; set; }
        public List<FormulaVariableDto> Variables { get; set; } = new();
    }
}
