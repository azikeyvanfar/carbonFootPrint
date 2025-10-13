using System.Collections.Generic;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum IsSuiteUrlKeyEnum
    {
        /// <summary>
        /// قرارداد کار
        /// </summary>
        pds_per_contracts_viw = 1, // http://services.msc.ir/ords/pds/per/pds_per_contracts_viw/

        /// <summary>
        /// چک Otp ارسال شده به کاربر
        /// </summary>
        check_cod_fun,// http://services.msc.ir/ords/wse/per/check_cod_fun

        /// <summary>
        /// اطلاعات کاربر
        /// </summary>
        pds_per_employees_info_spc_viw, // http://services.msc.ir/ords/pds/per/pds_per_employees_info_spc_viw/

        /// <summary>
        /// واحدها	       
        /// </summary>
        ojc_cpm_business_units_viw, // http://services.msc.ir/ords/ojc/cpmweb/ojc_cpm_business_units_viw/

        /// <summary>
        /// ناحیه ها
        /// </summary>
        lkp_cod_area_busun, // http://services.msc.ir/ords/ojc/cpmweb/lkp_cod_area_busun/?limit=500000

        /// <summary>
        /// گزارش تعداد خطرات به تفکيک طبقه و سطح ریسک
        /// </summary>
        grade, // http://services.msc.ir/ords/cpm/web/grade/?P_AREA=11&limit=50000

        /// <summary>
        /// محاسبه fr-sr بر اساس واحد کارکنان فولاد
        /// </summary> 
        frqsrq, // http://services.msc.ir/ords/cpm/web/frqsrq/?P_DAT_MNTH=140203

        /// <summary>
        /// محاسبه fr-sr بر اساس ناجیه کارکنان فولاد
        /// </summary> 
        frqsrq_area, // http://services.msc.ir/ords/cpm/web/frqsrq_area/?P_DAT_MNTH=140203

        /// <summary>
        /// اطلاعات قرارداد ها(پیمانکاران)
        /// </summary>
        cpm_cpmweb_contracts_viw, // http://services.msc.ir/ords/cpm/cpmweb/cpm_cpmweb_contracts_viw/

        /// <summary>
        /// اطلاعات پرسنل پیمانکار
        /// </summary>
        cpm_cpmweb_empl_contr_viw, // http://services.msc.ir/ords/cpm/cpmweb/cpm_cpmweb_empl_contr_viw/

        /// <summary>
        /// گزارش ارزیابی عملکرد ایمنی پیمانکاران
        /// </summary>
        contractors_cpm_performance, // http://services.msc.ir/ords/cpm/web/contractors_cpm_performance/


        /// <summary>
        ///   اطلاعات شیفت کاری شخص
        /// </summary>
        aac_cpm_shift_info_viw, // http://services.msc.ir/ords/aac/cpmweb/aac_cpm_shift_info_viw/
        /// <summary>
        /// تقویم کاری
        /// </summary>
        aac_cpm_calendar_viw, // http://services.msc.ir/ords/aac/cpmweb/aac_cpm_calendar_viw/
        /// <summary>
        /// درخت کمیته های BPMS
        /// </summary>
        activeUnitsOfChart, //  http://services.msc.ir/bpms/OCM/api/v1.0/activeUnitsOfChart
        /// <summary>
        ///  کاربران فولاد
        /// </summary>
        pds_cpm_employees_viw, // http://services.msc.ir/ords/pds/cpmweb/pds_cpm_employees_viw/
        /// <summary>
        /// گزارش متوسط زمان تعیین تکلیف
        /// </summary>
        getCommiteeAvgDcd, // http://services.msc.ir/bpms/GRN/api/v1.0/GreenCardProcess/getCommiteeAvgDcd
        /// <summary>
        /// وب سرويس عملكرد بودجه بهای تمام شده
        /// - اطلاعات مصرفی متریال های حسابداری صنعتی
        /// </summary>
        ccper, // http://services.msc.ir/ords/coa/ext/ccper/?P_DAT_FROM=140301&P_DAT_TO=140310
        /// <summary>
        /// وب سرويس اقلام هزینه
        /// - متریالهای موجود در حسابداری صنعتی
        /// </summary>
        expit, // http://services.msc.ir/ords/coa/ext/expit/
        /// <summary>
        /// لیست واحد های سازمانی با مرکز هزینه مشترک
        /// </summary>
        ojc_busn_share_cc_viw, // http://services.msc.ir/ords/ojc/cpmweb/ojc_busn_share_cc_viw/
        /// <summary>
        /// لیست مراکز هزینه برای واحد سازمانی پدر در زمان درخواستی
        /// </summary>
        ojc_per_busun_fun, // http://services.msc.ir/ords/ojc/cpmweb/ojc_per_busun_fun/




    }


    public class ApiValue
    {
        public string Url { get; set; }
        public ServiceEnum Service { get; set; }
    }
    public class SensitiveUrlValue
    {
        public string Url { get; set; }
        public string ClaimValue { get; set; }
    }

    public static class IsSuiteUrlClass
    {

        public static Dictionary<IsSuiteUrlKeyEnum, ApiValue> dict = new() {  
           
            /// <summary>
            /// چک Otp ارسال شده به کاربر
            /// </summary>
            { IsSuiteUrlKeyEnum.check_cod_fun,    new (){ Url = "ords/wse/per/check_cod_fun"  ,  Service = ServiceEnum. WSE}}, 
                 
            /// <summary>
            /// اطلاعات کاربر
            /// </summary>
            { IsSuiteUrlKeyEnum.pds_per_employees_info_spc_viw,  new (){ Url = "ords/pds/per/pds_per_employees_info_spc_viw/"  ,  Service = ServiceEnum. PDS}}, 
            
            /// <summary>
            /// واحدها	       
            /// </summary>
            { IsSuiteUrlKeyEnum.ojc_cpm_business_units_viw,  new (){ Url = "ords/ojc/cpmweb/ojc_cpm_business_units_viw/"  ,  Service = ServiceEnum. OJC}}, 
                           
            /// <summary>
            /// ناحیه ها
            /// </summary>
            { IsSuiteUrlKeyEnum.lkp_cod_area_busun,  new (){ Url = "ords/ojc/cpmweb/lkp_cod_area_busun/"  ,  Service = ServiceEnum. OJC}}, 

            /// <summary>
            /// گزارش تعداد خطرات به تفکيک طبقه و سطح ریسک
            /// </summary>
            { IsSuiteUrlKeyEnum.grade,  new (){ Url = "ords/cpm/web/grade/" ,  Service = ServiceEnum. CPM}},

            /// <summary>
            /// محاسبه fr-sr - detail
            /// </summary>
            { IsSuiteUrlKeyEnum.frqsrq,  new (){ Url = "ords/cpm/web/frqsrq/" ,  Service = ServiceEnum. CPM}},
              
            /// <summary>
            /// محاسبه fr-sr 
            /// </summary>
            { IsSuiteUrlKeyEnum.frqsrq_area,  new (){ Url = "ords/cpm/web/frqsrq_area/",  Service = ServiceEnum. CPM}},
            
            /// <summary>
            /// اطلاعات قرارداد ها(پیمانکاران)
            /// </summary>
            { IsSuiteUrlKeyEnum.cpm_cpmweb_contracts_viw, new (){ Url = "ords/cpm/cpmweb/cpm_cpmweb_contracts_viw/" ,  Service = ServiceEnum. CPM}},

            /// <summary>
            /// اطلاعات پرسنل پیمانکار
            /// </summary>
            { IsSuiteUrlKeyEnum.cpm_cpmweb_empl_contr_viw, new (){ Url = "ords/cpm/cpmweb/cpm_cpmweb_empl_contr_viw/" ,  Service = ServiceEnum. CPM}},
            
            /// <summary>
            /// گزارش ارزیابی عملکرد ایمنی پیمانکاران
            /// </summary> 
            { IsSuiteUrlKeyEnum.contractors_cpm_performance, new (){ Url = "ords/cpm/web/contractors_cpm_performance/" ,  Service = ServiceEnum. CPM}},

           
            /// <summary>
            ///   اطلاعات شیفت کاری شخص
            /// </summary>
            { IsSuiteUrlKeyEnum.aac_cpm_shift_info_viw, new (){ Url = "ords/aac/cpmweb/aac_cpm_shift_info_viw/" ,   Service = ServiceEnum. AAC}},
            
            /// <summary>
            /// تقویم کاری
            /// </summary>
            { IsSuiteUrlKeyEnum.aac_cpm_calendar_viw, new (){ Url = "ords/aac/cpmweb/aac_cpm_calendar_viw/",   Service = ServiceEnum. AAC}},

            /// <summary>
            /// درخت کمیته های BPMS
            /// </summary>
            { IsSuiteUrlKeyEnum.activeUnitsOfChart, new (){ Url = "bpms/OCM/api/v1.0/activeUnitsOfChart" ,   Service = ServiceEnum.OCM}},
           
            /// <summary>
            ///  کاربران فولاد
            /// </summary>
            { IsSuiteUrlKeyEnum.  pds_cpm_employees_viw, new (){ Url = "ords/pds/cpmweb/pds_cpm_employees_viw/" ,   Service = ServiceEnum.PDS}},
            
            /// <summary>
            /// گزارش متوسط زمان تعیین تکلیف
            /// </summary>
            { IsSuiteUrlKeyEnum.getCommiteeAvgDcd, new (){ Url = "bpms/GRN/api/v1.0/GreenCardProcess/getCommiteeAvgDcd" ,   Service = ServiceEnum.GRN}},
            /// <summary>
            /// وب سرويس عملكرد بودجه بهای تمام شده
            /// اطلاعات مصرفی متریال های حسابداری صنعتی
            /// </summary>
            { IsSuiteUrlKeyEnum.ccper,  new (){ Url = "ords/coa/ext/ccper/" ,   Service = ServiceEnum.COA}},
            /// <summary>
            /// وب سرويس اقلام هزینه
            /// متریالهای موجود در حسابداری صنعتی
            /// </summary>
            { IsSuiteUrlKeyEnum.expit,  new (){ Url = "ords/coa/ext/expit/" ,   Service = ServiceEnum.COA}},

            /// <summary>
            /// لیست مراکز هزینه برای واحد سازمانی در زمان درخواستی
            /// </summary> 
            { IsSuiteUrlKeyEnum.ojc_per_busun_fun,  new (){ Url = "ords/ojc/cpmweb/ojc_per_busun_fun/" ,   Service = ServiceEnum.OJC}},
      
            /// <summary>
            /// 
            /// </summary> 
            { IsSuiteUrlKeyEnum.ojc_busn_share_cc_viw,  new (){ Url = "ords/ojc/cpmweb/ojc_busn_share_cc_viw/" ,   Service = ServiceEnum.OJC}},
       

            ///  *********************    Insert new Is-Suite APIs HERE   *********************************
        
        
        
        };


        public static Dictionary<IsSuiteUrlKeyEnum, SensitiveUrlValue> sensitiveUrls2 = new()
        {

        };


    }

}
