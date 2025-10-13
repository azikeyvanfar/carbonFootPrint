using System;

namespace ContractorBackend.Application.Dtos.Core
{
    public class ShadowPropertyDto
    {
        public long? LastModifiedByUserId { get; set; }
        public string LastModifiedByFullName { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
