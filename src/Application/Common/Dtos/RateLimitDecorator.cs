namespace ContractorBackend.Application.Common.Dtos
{
    public class RateLimitDecorator
    {
        /// <summary>
        /// in Minutes
        /// </summary>
        public int TimeWindow { get; set; }
        public int MaxRequests { get; set; }
    }
}
