using Gridify;

namespace ContractorBackend.Application.Common.Models
{
    public class SearchQueryRequest : GridifyQuery
    {
        /// <summary>
        /// To Do : search parameters
        /// </summary>
        public string Search { get; set; }
    }
}
