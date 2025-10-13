using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum RoleType
    {
        [Display(Name = "نقش هایی که به WebApi دسترسی دارند")]
        Manager = 1,
        [Display(Name = "نقش هایی که به WebApi دسترسی دارند")]
        Employee = 2
    }
}
