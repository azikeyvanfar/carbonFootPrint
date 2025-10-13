using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class PageRouteWithClaimDto
    {
        public PageRouteWithClaimDto()
        {
            //Claims = new(); // if claims where Type of List 
        }
        public Guid Id { get; set; }
        public Guid PageRouteId { get; set; }
        public string Route { get; set; }
        public RoleType RoleType { get; set; }
        public string RoleTypeId { get; set; }
        public string RouteName { get; set; }
        public IEnumerable<ClaimDto> Claims { get; set; }

    }

    public class ClaimDto
    {
        public Guid Id { get; set; }
        public Guid GeneralClaimId { get; set; }
        public string ClaimValue { get; set; }
        public RoleType ClaimRoleType { get; set; }
        public string ClaimRoleTypeId { get; set; }
        public string ClaimName { get; set; }
        public Guid PageRouteId { get; set; }
    }
}
