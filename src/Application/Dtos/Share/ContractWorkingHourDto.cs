using System;

namespace ContractorBackend.Domain.Entities.IsSuiteEntities
{
    public class ContractWorkingHourDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// قرارداد
        /// </summary>
        public Guid? ContractorId { get; set; }
        /// <summary>
        /// جمع ساعت کارکرد
        /// </summary>
        public float TotalHours { get; set; }
        /// <summary>
        /// ماه قرارداد 
        /// 2024-05
        /// </summary>
        public DateTime ContractDate { get; set; }

        /// <summary>
        /// قرارداد
        /// </summary>
        public string ContractorName { get; set; }
        /// <summary>  
        /// شماره قرارداد                        
        /// </summary>
        public long? ContractorContractNumber { get; set; }  // num_pgv_puror  

        /// <summary>
        /// سازنده
        /// </summary>
        public long CreatedByUserId { get; set; }
        /// <summary>
        /// نام سازنده
        /// </summary>
        public string CreatedByUserFullName { get; set; }
    }
}
