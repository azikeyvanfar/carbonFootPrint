using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum OtpType
    {
        [Display(Name = "لود صفحات حساس")]
        SpecialPage = 1,
        [Display(Name = "گواهی کسر از حقوق")]
        SalaryDeductionCertificate = 2,
        [Display(Name = "گواهی کسر اشتغال به کار")]
        EmploymentCertificate = 3,
        [Display(Name = "نمایش فرم به افرادی که لاگین نکرده اند")]
        CheckForViewForm = 4,
    }
}
