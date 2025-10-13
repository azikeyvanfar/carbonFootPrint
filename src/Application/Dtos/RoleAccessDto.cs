using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos
{
    public class RoleAccessDto
    {
        public RoleAccessDto()
        {
            RolePageRouteList = new();
        }
        public long RoleId { get; set; }

        public RoleType RoleType { get; set; }

        public string RoleTypeId { get; set; }

        public string RoleName { get; set; }

        public List<RolePageRoute> RolePageRouteList { get; set; }
    }

    public class RolePageRoute
    {
        public RolePageRoute()
        {
            PageRouteClaimList = new();
        }
        public Guid PageRouteId { get; set; }
        public string Route { get; set; }
        public string RouteName { get; set; }
        public List<PageRouteClaimVm> PageRouteClaimList { get; set; }

    }

    public class PageRouteClaimVm
    {
        public string ClaimValue { get; set; }

        public Guid GeneralClaimId { get; set; }
    }
}
