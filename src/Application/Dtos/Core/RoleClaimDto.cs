using System;

namespace ContractorBackend.Application.Dtos.Core
{
    public class RoleClaimDto
    {
        public long RoleId { get; set; }

        public string RoleName { get; set; }

        public Guid PageRouteClaimId { get; set; }

        public Guid PageRouteId { get; set; }

        public string Route { get; set; }

        public string RouteName { get; set; }

        public Guid GeneralClaimId { get; set; }

        public string ClaimValue { get; set; }
    }
}
