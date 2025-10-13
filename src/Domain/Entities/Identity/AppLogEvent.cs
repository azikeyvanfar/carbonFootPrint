using System;
using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class AppLogEvent
    {
        public int Id { get; set; }
        [MaxLength(450)]
        public string UserName { get; set; }
        public long? UserId { get; set; }
        [MaxLength(500)]
        public string UserDisplayName { get; set; }
        [MaxLength(100)]
        public string CreatedByIp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }
        [MaxLength(10)]
        public string? SystemName { get; set; }
    }
}
