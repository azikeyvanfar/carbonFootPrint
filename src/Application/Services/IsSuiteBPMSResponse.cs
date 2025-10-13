using System.Collections.Generic;

namespace ContractorBackend.Application.Services
{
    public class IsSuiteBPMSResponse<T> where T : class
    {
        public List<T> Items { get; set; } = new();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

    }
}