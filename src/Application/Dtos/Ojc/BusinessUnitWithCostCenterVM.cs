using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class BusinessUnitWithCostCenterVM : PageableDto
    {
        /// <summary>
        /// شناسه قلم هزینه 
        /// </summary>
        public long? business_unit_id { get; set; } //BusinessUnitId 
        /// <summary>
        /// کد قلم هزینه 
        /// </summary>
        public string cod_busun { get; set; } //BusinessUnitCode 
        /// <summary>
        /// نام قلم هزینه 
        /// </summary>
        public string des_busun { get; set; } //BusinessUnitName 
        /// <summary>
        /// شناسه مرکز هزینه
        /// </summary>
        public long? cost_center_id { get; set; } //CostCenterId 
        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string cod_cc_ccntr { get; set; } //CostCenterCode 
        /// <summary>
        /// نام مرکز هزینه
        /// </summary>
        public string des_cc_ccntr { get; set; } //CostCenterName 


        public string lkp_cod_area_busun { get; set; } //CostCenterName 
        public string des_lkp_cod_area_busun { get; set; } //CostCenterName 


    }
}
