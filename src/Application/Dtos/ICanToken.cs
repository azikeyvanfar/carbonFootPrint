using System;
using System.Text.Json.Serialization;

namespace ContractorBackend.Application.Dtos
{
    public class ICanToken
    {
        [JsonPropertyName("token")]
        public string token { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }
    }
}
