using System;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Core
{
    public class ErrorHistory : BaseEntity, ICreationTrackingEntity, IModificationTrackingEntity
    {
        public long Id { get; set; }
        [MaxLength(500)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Route { get; set; }
        public string Description { get; set; }

        public string Message { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }
        [MaxLength(10)]
        public string SystemName { get; set; }

        public long? UserId { get; set; }
        public DateTime? TimeStamp { get; set; } = DateTime.Now;

    }
}
