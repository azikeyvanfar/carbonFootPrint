using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class BusinessUnitWithCostCenterDto : PageableDto
    {
        /// <summary>
        /// شناسه قلم هزینه 
        /// </summary>
        public long? BusinessUnitId { get; set; }
        /// <summary>
        /// کد قلم هزینه 
        /// </summary>
        public string BusinessUnitCode { get; set; }
        /// <summary>
        /// نام قلم هزینه 
        /// </summary>
        public string BusinessUnitName { get; set; }
        /// <summary>
        /// شناسه مرکز هزینه
        /// </summary>
        public long? CostCenterId { get; set; }
        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string CostCenterCode { get; set; }
        /// <summary>
        /// نام مرکز هزینه
        /// </summary>
        public string CostCenterName { get; set; }

    }
}
