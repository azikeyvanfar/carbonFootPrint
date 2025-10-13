using System;

namespace ContractorBackend.Application.Dtos
{
    public class SubcriterionDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        public string Name { get; set; }

    }
}
