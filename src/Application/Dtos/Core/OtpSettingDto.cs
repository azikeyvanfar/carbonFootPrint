using System;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class OtpSettingDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public string Otp { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset WaitConfirmTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset ActiveTime { get; set; }
        public OtpType otpType { get; set; }

        public string HashToken { get; set; }
        public string PersonnelCode { get; set; }
    }
}
