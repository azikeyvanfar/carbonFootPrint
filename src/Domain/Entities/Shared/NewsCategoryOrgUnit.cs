using System;

namespace ContractorBackend.Domain.Entities.Shared
{
    public class NewsCategoryOrgUnit
    {
        public Guid Id { get; set; }
        public Guid NewsCategoryId { get; set; }
        public NewsCategory NewsCategory { get; set; }
        public Guid OrgUnitId { get; set; }
        //public BusinessUnit OrgUnit { get; set; }
    }
}
