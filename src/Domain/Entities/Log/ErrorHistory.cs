using System;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Log
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
        public LogType ProjectType { get; set; }

        public long? UserId { get; set; }
        public DateTime? TimeStamp { get; set; } = DateTime.Now;

    }
}
