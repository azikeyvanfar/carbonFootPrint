using System;
using System.Collections.Generic;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos
{
    public class PageRouteDto
    {
        public PageRouteDto()
        {
            Claims = new();
        }
        public Guid Id { get; set; }

        public string RouteName { get; set; }

        public string Route { get; set; }

        public string Icon { get; set; }

        public RoleType RoleType { get; set; }

        public string RoleTypeId { get; set; }
        public bool IsOtp { get; set; }
        public List<SelectModel> Claims { get; set; }
    }
}
