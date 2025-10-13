using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum MenuType
    {
        [Display(Name = "صفحه")]
        Page = 0,

        [Display(Name = "فرم داینامیک")]
        Form = 1,

        [Display(Name = "لینک خارجی")]
        ExternalUrl = 2
    }
}
