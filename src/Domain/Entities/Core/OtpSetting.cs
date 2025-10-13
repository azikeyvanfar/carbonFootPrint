using System;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    public class OtpSetting : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string Otp { get; set; }
        public DateTimeOffset WaitConfirmTime { get; set; }
        public DateTimeOffset ActiveTime { get; set; }
        public OtpType OtpType { get; set; }
        public string HashToken { get; set; }
        [MaxLength(500)]
        public string PersonnelCode { get; set; }
    }
}
