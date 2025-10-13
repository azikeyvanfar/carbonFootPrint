using System.ComponentModel.DataAnnotations;

namespace ContractorBackend.Domain.Enums.Core
{
    /// <summary>
    /// in this case 0 means insert local and it's the DEFAULT value
    /// LEAVE IT BE
    /// </summary>
    public enum EntityStatus
    {
        /// <summary>
        /// ذخیره داخلی
        /// </summary>
        [Display(Name = "ذخیره داخلی")]
        InsertLocal = 0,
        /// <summary>
        /// ذخیره در سیستم Is-Suite
        /// </summary>
        [Display(Name = "ذخیره در سیستم Is-Suite")]
        SuccessFullSend = 1,
        /// <summary>
        /// خطا در ذخیره سیستم Is-Suite
        /// </summary>
        [Display(Name = "خطا در ذخیره سیستم Is-Suite")]
        FailedSend = 2
    }
}
