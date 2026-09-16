using System;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Application.Dtos.Ghg
{
    public class EmissionFactorDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public EmissionFactorCategory Category { get; set; }
        public string RefKey { get; set; } = null!;
        public string? RefLabel { get; set; }
        public string? SubKey { get; set; }
        public string Gas { get; set; } = null!;
        public double Value { get; set; }
        public string Unit { get; set; } = null!;
        public EmissionStandard Standard { get; set; }
        public string? Source { get; set; }
        public int ValidFromYear { get; set; }
    }

    public class AddEmissionFactorDto
    {
        public EmissionFactorCategory Category { get; set; }
        public string RefKey { get; set; } = null!;
        public string? RefLabel { get; set; }
        public string? SubKey { get; set; }
        public string Gas { get; set; } = "CO2e";
        public double Value { get; set; }
        public string Unit { get; set; } = null!;
        public EmissionStandard Standard { get; set; } = EmissionStandard.Ipcc2006;
        public string? Source { get; set; }
        public int ValidFromYear { get; set; }
    }

    public class UpdateEmissionFactorDto : AddEmissionFactorDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
