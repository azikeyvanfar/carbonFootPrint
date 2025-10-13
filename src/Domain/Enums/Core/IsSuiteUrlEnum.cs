using System.Collections.Generic;

namespace ContractorBackend.Domain.Enums.Core
{
    public enum IsSuiteUrlKeyEnum
    {
        /// <summary>
        /// پیمانکاران
        /// </summary>
        cpmCpmperEmployeesViw, // http://services.msc.ir/in/eis/ords/cpm/cpmper/cpm_cpmper_employees_viw/
        /// <summary>
        /// چک Otp ارسال شده به کاربر
        /// </summary>
        check_cod_fun,// http://services.msc.ir/ords/wse/per/check_cod_fun

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
            { IsSuiteUrlKeyEnum.cpmCpmperEmployeesViw,  new (){ Url = "ords/cpm/cpmper/cpm_cpmper_employees_viw/"  ,  Service = ServiceEnum. CPM}}, 
             /// <summary>
            /// چک Otp ارسال شده به کاربر
            /// </summary>
            { IsSuiteUrlKeyEnum.check_cod_fun,    new (){ Url = "ords/wse/per/check_cod_fun"  ,  Service = ServiceEnum. WSE}}, 

            ///  *********************    Insert new Is-Suite APIs HERE   *********************************
        
        };


        public static Dictionary<IsSuiteUrlKeyEnum, SensitiveUrlValue> sensitiveUrls2 = new()
        {

        };


    }

}
