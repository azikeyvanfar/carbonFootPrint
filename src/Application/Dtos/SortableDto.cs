using System;

namespace ContractorBackend.Application.Dtos
{
    /// <summary>
    /// add CreatedDateTime/ModifiedDateTime to Dtos in order to call OrderBy from Client
    /// </summary>
    public class SortableDto
    {
        public DateTimeOffset? CreatedDateTime { get; set; }

        public DateTimeOffset? ModifiedDateTime { get; set; }
    }
}
