using System;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class AreaDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// نام فارسی
        /// </summary>
        public string farsi { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string val { get; set; }

        public DateTimeOffset? CreatedDateTime { get; set; }
        public DateTimeOffset? ModifiedDateTime { get; set; }
    }
}