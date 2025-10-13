using System;
using System.Collections.Generic;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos
{
    public class RoleDto
    {
        public RoleDto()
        {
            ActionList = new();
        }
        public long RoleId { get; set; }

        public string Title { get; set; }

        public RoleType RoleType { get; set; }

        public bool IsActive { get; set; } = true;

        public string Description { get; set; }

        public bool HasAccess { get; set; }
        public bool IsAdministrator { get; set; }
        public List<SelectModel> ActionList { get; set; }


        /// <summary>
        /// حوزه سیستم
        /// </summary>
        public Guid? RoleScopeId { get; set; }
        /// <summary>
        /// حوزه سیستم
        /// </summary>
        public string RoleScopeName { get; set; }
    }
}
