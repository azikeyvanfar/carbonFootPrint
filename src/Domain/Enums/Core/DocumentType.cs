using System.ComponentModel.DataAnnotations;


namespace ContractorBackend.Domain.Enums.Core
{
    public enum DocumentType
    {
        /// <summary>
        ///  Folder
        /// </summary>

        [Display(Name = "Folder")]
        Folder = 1,

        /// <summary>
        /// File
        /// </summary>

        [Display(Name = "File")]
        File = 2,
    }
}
