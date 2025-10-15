using System.Collections.Generic;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum IsSuiteUrlKeyEnum
    {
        /// <summary>
        /// پیمانکاران
        /// </summary>
        cpm_cpmper_employees_viw, // https://services.msc.ir/in/eis/ords/cpm/cpmper/cpm_cpmper_employees_viw/
        /// <summary>
        /// چک Otp ارسال شده به کاربر
        /// </summary>
        check_cod_fun,// http://services.msc.ir/ords/wse/per/check_cod_fun
        /// <summary>
        ///تحت تکفل پیمانکاران
        /// </summary>
        emp_cont_familys_viw, // https://services.msc.ir/in/eis/ords/cpm/cpmper/emp_cont_familys_viw/
        /// <summary>
        ///قرارداها
        /// </summary>
        cpm_cpmper_contract_info_viw, // https://services.msc.ir/in/eis/ords/cpm/cpmper/cpm_cpmper_contract_info_viw/

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
            /// پیمانکاران
            /// </summary>
            { IsSuiteUrlKeyEnum.cpm_cpmper_employees_viw,  new (){ Url = "ords/cpm/cpmper/cpm_cpmper_employees_viw/"  ,  Service = ServiceEnum. CPM}}, 
             /// <summary>
            /// چک Otp ارسال شده به کاربر
            /// </summary>
            { IsSuiteUrlKeyEnum.check_cod_fun,    new (){ Url = "ords/wse/per/check_cod_fun"  ,  Service = ServiceEnum. WSE}}, 
            /// <summary>
            ///تحت تکفل پیمانکاران
            /// </summary>
            { IsSuiteUrlKeyEnum.emp_cont_familys_viw,  new (){ Url = "ords/cpm/cpmper/emp_cont_familys_viw/"  ,  Service = ServiceEnum. CPM}},
            /// <summary>
            ///قراردادها
            /// </summary>
            { IsSuiteUrlKeyEnum.cpm_cpmper_contract_info_viw,  new (){ Url = "ords/cpm/cpmper/cpm_cpmper_contract_info_viw/"  ,  Service = ServiceEnum. CPM}},
            
            ///  *********************    Insert new Is-Suite APIs HERE   *********************************
        
        };


        public static Dictionary<IsSuiteUrlKeyEnum, SensitiveUrlValue> sensitiveUrls2 = new()
        {

        };


    }

}
