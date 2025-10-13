using System;

namespace ContractorBackend.Application.Dtos.Core
{
    public class FileExtensionDto
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; }
        public string FileTypes { get; set; }
        public int MinSizeByte { get; set; }
        public int MaxSizeByte { get; set; }
        /// <summary>
        /// تعداد فایل مجاز برای اپلود
        /// </summary>
        public int MaxAllowedCount { get; set; }
    }
}
