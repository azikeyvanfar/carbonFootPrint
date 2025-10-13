using System;

namespace ContractorBackend.Domain.Entities
{
    public class EmployeeVM
    {
        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public long? num_prsn_emply { get; set; }
        /// <summary>
        /// نام
        /// </summary> 
        public string? nam_first_emply { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary> 
        public string? nam_last_emply { get; set; }
        ///// <summary>
        ///// شماره موبایل
        ///// </summary>
        public string num_mobil_emply { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string? cod_nat_emply { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? dat_birth_emply { get; set; }



        /// <summary>
        /// نام پدر
        /// </summary>
        public string? nam_fathr_emply { get; set; }
        /// <summary>
        /// کد جنسیت
        /// </summary>
        public string? lkp_cod_sex_emply { get; set; }
        /// <summary>
        /// چنسیت
        /// </summary>
        public string? des_lkp_cod_sex_emply { get; set; }



        /// <summary>
        /// کد تیم
        /// </summary>
        public string? num_team_emply { get; set; }
        /// <summary>
        /// ایدی تیم
        /// </summary>
        public long? team_id { get; set; }
        /// <summary>
        /// سن
        /// </summary>
        public int? age { get; set; }
        /// <summary>
        /// کد شغل
        /// </summary>
        public string? cod_job { get; set; }
        /// <summary>
        /// عنوان شغل
        /// </summary>
        public string? des_job { get; set; }
        /// <summary>
        /// کد تحصیلات
        /// </summary>
        public string? cod_educa { get; set; }
        /// <summary>
        /// عنوان تحصیلات
        /// </summary>
        public string? des_educa { get; set; }
        /// <summary>
        /// سال سنوات کاری
        /// </summary>
        public int? sen_int_y { get; set; }
        /// <summary>
        /// ماه سنوات کاری
        /// </summary>
        public int? sen_int_m { get; set; }
        /// <summary>
        /// شماره قرارداد
        /// </summary>
        public int? num_ord_empca { get; set; }
        /// <summary>
        /// کد شرکت
        /// </summary>
        public string cod_cntr_empca { get; set; }




        /// <summary>
        /// ایدی واحد (is-suite)
        /// </summary>
        public int? busun_business_unit_id { get; set; }
        /// <summary>
        /// کد سه حرفی واحد
        /// </summary>
        public string? cod_busun_emply { get; set; }





        /// <summary>
        /// محل کار
        /// </summary>
        public string? workplace { get; set; }


        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string num_crt_emply { get; set; }

        /// <summary>
        /// عکس فرد
        /// </summary>
        public byte[]? image_date { get; set; }
    }
}

