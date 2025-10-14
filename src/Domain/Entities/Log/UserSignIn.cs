using System;
using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Entities.Log
{
    public class UserSignIn
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string PersonnelCode { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public bool IsLogin { get; set; }
        [MaxLength(22)]
        public string CreatedByIP { get; set; }
        [MaxLength(1000)]
        public string CreatedByBrowserName { get; set; }
        [MaxLength(1000)]
        public string CreatedByBrowserNameSpecific { get; set; }

        [MaxLength(500)]
        public string ErrorReason { get; set; }
        [MaxLength(10)]
        public string SystemName { get; set; }

    }
}
