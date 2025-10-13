using System;
using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Entities.Core
{
    public class FileExtension
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; }
        public string FileTypes { get; set; }
        [Range(0, int.MaxValue)]
        public int MinSizeByte { get; set; }
        [Range(0, int.MaxValue)]
        public int MaxSizeByte { get; set; }
        /// <summary>
        /// تعداد فایل مجاز برای اپلود
        /// </summary>
        [Range(0, int.MaxValue)]
        public int MaxAllowedCount { get; set; }
    }
}
