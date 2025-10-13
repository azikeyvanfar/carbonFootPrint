using System;

namespace ContractorBackend.Application.Dtos
{
    public class FileVM
    {
        public Guid DocId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
    }
}
