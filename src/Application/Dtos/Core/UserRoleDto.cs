using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Dtos.Core
{
    public class UserRoleDto
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public string PersonnelCode { get; set; }
        public RoleType UserRoleType { get; set; }
        public string UserRoleTypeId { get; set; }
        [Required]
        public long RoleId { get; set; }
        public string RoleTitle { get; set; }
    }
}