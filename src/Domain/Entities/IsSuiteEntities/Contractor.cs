using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.IsSuiteEntities
{
    public class Contractor : IsSuiteBaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;


        /// <summary>  
        /// کد شرکت                        
        /// </summary>
        public string ContractorCode { get; set; }  // supco_thprt_cod_thprt  
        /// <summary>  
        /// نام شرکت                        
        /// </summary>
        public string ContractorName { get; set; }  // des_thprt  
        /// <summary>  
        /// شماره قرارداد                        
        /// </summary>
        public long? ContractorContractNumber { get; set; }  // num_pgv_puror  
        /// <summary>  
        /// شرح قرارداد                        
        /// </summary>
        public string Description { get; set; }  // des_sho_puror  
        /// <summary>  
        /// تعداد پرسنل                        
        /// </summary>
        public string EmployeeCount { get; set; }  // cnt_empl  
        /// <summary>  
        /// کد ناحیه                        
        /// </summary>
        public string AreaCode { get; set; }  // lkp_cod_area_busun  
        /// <summary>  
        /// کد واحد                        
        /// </summary>
        public string BusinessUnitCode { get; set; }  // cod_busun  
        /// <summary>  
        /// ای دی واحد                        
        /// </summary>
        public long? BusinessUnitIsSuiteId { get; set; }  // busun_business_unit_id  
        /// <summary>  
        ///نام مدیر عامل                        
        /// </summary>
        public string CEOFullName { get; set; }  // nam_manager  
        /// <summary>  
        /// پرسنلی نماینده                        
        /// </summary>
        public long? EmployerRepresentitivePersonnelCode { get; set; }  // num_prsn_agent  
        /// <summary>  
        /// نام نماینده                        
        /// </summary>
        public string EmployerRepresentitiveFullName { get; set; }  // nam_agent  
        /// <summary>  
        /// پرسنلی ناظر                        
        /// </summary>
        public long? SupervisorPersonnelCode { get; set; }  // emply_num_prsn_emply  

        /// <summary>  
        /// نام ناظر                         
        /// </summary>
        public string SupervisorFullName { get; set; }  // full_nam  

        /// <summary>
        /// تاریخ شروع قرارداد
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// تاریخ شروع قرارداد
        /// </summary>
        public DateTime? EndDate { get; set; }


    }
}
