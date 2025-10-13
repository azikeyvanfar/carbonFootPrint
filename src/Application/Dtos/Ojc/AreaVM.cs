using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class AreaVM : PageableDto
    {
        public string farsi { get; set; }
        public string val { get; set; }
    }
}