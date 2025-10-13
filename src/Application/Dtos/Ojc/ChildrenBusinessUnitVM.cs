using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class ChildrenBusinessUnitVM : PageableDto
    {
        /// <summary>
        /// شناسه قلم هزینه 
        /// </summary>
        public long? business_unit_id { get; set; }
        /// <summary>
        /// کد قلم هزینه 
        /// </summary>
        public string cod_busun { get; set; }
        /// <summary>
        /// نام قلم هزینه 
        /// </summary>
        public string des_busun { get; set; }
        /// <summary>
        /// شناسه مرکز هزینه
        /// </summary>
        public long? cost_center_id_busun { get; set; }
        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string cod_cc_busun { get; set; }
        /// <summary>
        /// نام مرکز هزینه
        /// </summary>
        public string des_cc_busun { get; set; }

        /// <summary>
        /// تاریخ شروع 
        /// </summary>
        public string dat_str { get; set; }
        /// <summary>
        /// تاریخ پایان 
        /// </summary>
        public string dat_end { get; set; }



    }
}
