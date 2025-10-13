using System;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Dtos
{
    public class AttachmentDto
    {
        public string Name { get; set; }
        public Guid RootId { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
        public string? Alt { get; set; }
    }
}
