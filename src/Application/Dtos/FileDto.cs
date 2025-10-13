using System;
using System.IO;

namespace ContractorBackend.Application.Dtos
{
    public class FileDto
    {
        public byte[]? Content { get; set; }
        public FileStream? FileContents { get; set; }
        public string? ContentType { get; set; }
        public string? FileDownloadName { get; set; }
    }

    public class FileBase64Dto
    {
        public Guid? Id { get; set; }
        public string? Content { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
    public class FileRecordDto
    {
        public Guid? Id { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
}
