namespace ContractorBackend.Application.Dtos.Cpm
{
    public class ContractorDto
    {
        /// <summary>
        /// کد پرسنلی
        /// </summary>
        public string num_prsn_emplc { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string nam_first_emplc { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string nam_last_emplc { get; set; }
        /// <summary>
        /// موبایل
        /// </summary>
        public string num_mobil_emplc { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string num_national_emplc { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public string dat_birth_emplc { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
        public string nam_father_emplc { get; set; }
        /// <summary>
        /// تصویر
        /// </summary>
        public string image_date { get; set; }
        /// <summary>
        /// کد جنسیت
        /// </summary>
        public string lkp_cod_sex_emplc { get; set; }
        /// <summary>
        /// شرح جنسیت
        /// </summary>
        public string des_lkp_cod_sex_emplc { get; set; }
        /// <summary>
        /// شرح تیم
        /// </summary>
        public string num_team_teams { get; set; }
        /// <summary>
        /// کد تیم
        /// </summary>
        public string teams_team_id { get; set; }
        /// <summary>
        /// سن
        /// </summary>
        public string age { get; set; }
        /// <summary>
        /// کد تحصیلی
        /// </summary>
        public string cod_educa { get; set; }
        /// <summary>
        /// شرح تحصیلی
        /// </summary>
        public string des_edu { get; set; }
        /// <summary>
        /// سابقه به سال
        /// </summary>
        public string sen_int_y { get; set; }
        /// <summary>
        /// سابقه به ماه
        /// </summary>
        public string sen_int_m { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string num_ord_empca { get; set; }
        public string cod_cntr_empca { get; set; }
        public string num_crt_emplc { get; set; }
        /// <summary>
        /// آی دی واحد
        /// </summary>
        public string busun_business_unit_id { get; set; }
        /// <summary>
        /// کد واحد
        /// </summary>
        public string cod_busun { get; set; }
        /// <summary>
        ///  پیمانکار1 ---- 2 شرکتی
        /// </summary>
        public string type_prsn { get; set; }

    }
}
