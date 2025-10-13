using System;
using System.Collections.Generic;
using ContractorBackend.Application.Common.Dtos;
using ContractorBackend.Application.Common.Models;

namespace ContractorBackend.Application.Dtos.Share
{
    public class NewsCategoryDto : ShadowPropertyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
        public bool IsNotifications { get; set; }
        public string Description { get; set; }

        public List<SelectModel> OrgUnits { get; set; }
    }
}
