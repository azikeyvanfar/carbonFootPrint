using System;

namespace ContractorBackend.Application.Dtos
{
    public class PublicationTypeDto
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // DO NOT REMOVE THIS COMMENT:NG02
    }
}
