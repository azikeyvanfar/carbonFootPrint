using System.Collections.Generic;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class UserWithRoleDto
    {
        public UserWithRoleDto()
        {
            //Roles = new();
        }
        public long UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DesPost { get; set; }
        public string PersonnelCode { get; set; }
        public IEnumerable<RoleUserDto> Roles { get; set; }
    }
    public class RoleUserDto
    {
        public long RoleId { get; set; }
        public string PersonnelCode { get; set; }
        public RoleType RoleType { get; set; }
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public long UserId { get; set; }
    }
}