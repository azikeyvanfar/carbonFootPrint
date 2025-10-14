using System;

namespace ContractorBackend.Domain.Entities.Shared
{
    public class HFJob
    {
        public long Id { get; set; }
        public long StateId { get; set; }
        public string StateName { get; set; }
        public string InvocationData { get; set; }
        public string Arguments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
