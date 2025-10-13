namespace ContractorBackend.Application.Dtos.Setting
{
    public class RateLimitCustomRuleDto
    {
        public bool? IsHeader { get; set; }
        public string Name { get; set; }
        public int? RateLimitTimeWindow { get; set; }
        public int? RateLimitMaxRequests { get; set; }

        public long ApplicationSettingId { get; set; }
    }
}
