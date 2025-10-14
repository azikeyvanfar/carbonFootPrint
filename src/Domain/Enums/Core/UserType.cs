using System.ComponentModel.DataAnnotations;
namespace ContractorBackend.Domain.Enums.Core
{
    /// <summary>
    /// نوع یوز های
    /// </summary>
    public enum  UserType
    {
        [Display(Name = "پیمانکار")]
        Contractor,
        [Display(Name="شرکتی")]
        Company,
    }
}
