using System;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos
{
    public class PageRouteClaimDto
    {
        public Guid Id { get; set; }

        public Guid GeneralClaimId { get; set; }
        public string ClaimValue { get; set; }
        public RoleType ClaimRoleType { get; set; }
        public string ClaimRoleTypeId { get; set; }
        public Guid PageRouteId { get; set; }
        public string Route { get; set; }
        public string RouteName { get; set; }
        public string ClaimName { get; set; }


    }
}
