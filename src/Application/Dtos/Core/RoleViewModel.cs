using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    public class RoleViewModel
    {
        public long RoleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAdministrator { get; set; }
        public List<string> Pathes { get; set; }
    }
}
