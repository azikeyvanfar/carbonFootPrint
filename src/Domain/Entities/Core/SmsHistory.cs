using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Domain.Entities.Core
{
    public class SmsHistory : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        public long? UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string SmsResponseMessage { get; set; }
    }
}
