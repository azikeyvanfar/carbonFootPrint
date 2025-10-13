using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    public class RolePageRouteAccessDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public Guid PageRouteId { get; set; }
        public string RouteName { get; set; }
        // public string MenuName { get; set; }
        public List<RoleClaimDto> ClaimPageRoutes { get; set; }

        // public long RoleId { get; set; }
    }
}
