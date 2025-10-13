using System;

namespace ContractorBackend.Application.Dtos.Share
{
    /// <summary>
    /// IS-SUITE اطلاعات کاربر
    /// </summary>
    public class UserInfoDto
    {
        public long id { get; set; }
        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string num_prsn_emply { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string nam_first_emply { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string nam_last_emply { get; set; }

        /// <summary>
        /// کد ملی
        /// </summary>

        public string cod_nat_emply { get; set; }

        /// <summary>
        /// عكس فرد
        /// </summary>
        public byte[] image_prsn_emply { get; set; }

        /// <summary>
        /// کد واحد سازمانی
        /// </summary>
        public string cod_busun_emply { get; set; }

        /// <summary>
        /// شرح واحد سازمانی
        /// </summary>
        public string des_busun { get; set; }

        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string cod_cc_emply { get; set; }

        /// <summary>
        /// شرح مرکز هزینه
        /// </summary>
        public string des_cc { get; set; }

        /// <summary>
        /// کد جنسیت
        /// </summary>
        public string lkp_cod_sex_emply { get; set; }

        /// <summary>
        /// عنوان جنسیت
        /// </summary>
        public string des_lkp_cod_sex_emply { get; set; }

        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? dat_birth_emply { get; set; }

        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public string lkp_cod_mrid_emply { get; set; }

        /// <summary>
        /// عنوان وضعیت تاهل
        /// </summary>
        public string des_lkp_cod_mrid_emply { get; set; }

        /// <summary>
        /// تعداد فرزندان
        /// </summary>
        public int? tot_child_emply { get; set; }

        /// <summary>
        /// نام پدر
        /// </summary>
        public string nam_fathr_emply { get; set; }

        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string nam_first_eng_emply { get; set; }

        /// <summary>
        /// نام خانوادگی انگلیسی
        /// </summary>
        public string nam_last_eng_emply { get; set; }

        /// <summary>
        /// کد محل تولد
        /// </summary>
        public long? gepos_geographic_position_id_a { get; set; }

        /// <summary>
        /// محل تولد
        /// </summary>
        public string des_gepos_geog_position_id_a { get; set; }

        /// <summary>
        /// کد محل صدور
        /// </summary>
        public long? gepos_geographic_position_id_b { get; set; }

        /// <summary>
        /// محل صدور
        /// </summary>
        public string des_gepos_geog_position_id_b { get; set; }

        /// <summary>
        /// تاریخ صدور شناسنامه
        /// </summary>
        public DateTime? dat_crt_emply { get; set; }

        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string num_crt_emply { get; set; }


        /// <summary>
        /// شماره سریال شناسنامه
        /// </summary>
        public string num_ser_crt_emply { get; set; }


        /// <summary>
        /// کد ملیت
        /// </summary>
        public string lkp_cod_natlty_emply { get; set; }


        /// <summary>
        /// عنوان ملیت
        /// </summary>
        public string des_lkp_cod_natlty_emply { get; set; }


        /// <summary>
        /// کد مذهب
        /// </summary>
        public string lkp_cod_rlgn_emply { get; set; }


        /// <summary>
        /// عنوان مذهب
        /// </summary>
        public string des_lkp_cod_rlgn_emply { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string num_mobil_emply { get; set; }

        /// <summary>
        /// شماره تلفن منزل
        /// </summary>
        public string num_tel_emply { get; set; }


        /// <summary>
        /// شماره تلفن اضطرازی
        /// </summary>
        public string num_tel_emrgy_emply { get; set; }

        /// <summary>
        /// تلفن داخلی 1
        /// </summary>
        public string num_tel_int_emply { get; set; }

        /// <summary>
        /// تلفن داخلی 2
        /// </summary>
        public string num_tel_int2_emply { get; set; }

        /// <summary>
        /// آدرس پست الکترونيکي
        /// </summary>
        public string des_email_emply { get; set; }

        /// <summary>
        /// کد ناحیه
        /// </summary>
        public string lkp_cod_area_busun { get; set; }

        /// <summary>
        /// شرح ناحیه
        /// </summary>
        public string des_lkp_cod_area_busun { get; set; }

        /// <summary>
        /// مسئول بالاتر
        /// </summary>
        public string up_lvl_emp { get; set; }

        /// <summary>
        /// کد پست
        /// </summary>
        public string cod_post_emply { get; set; }

        /// <summary>
        ///  عنوان پست
        /// </summary>
        public string des_per_postd { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) کد دسته بندی پرسنل
        /// </summary>
        public string lkp_sta_emplt_emply { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) شرح دسته بندی پرسنل
        /// </summary>
        public string des_lkp_sta_emplt_emply { get; set; }

        /// <summary>
        ///  کد زمان کار
        /// </summary>
        public string lkp_cod_wrktm { get; set; }

        /// <summary>
        ///  عنوان زمان کار
        /// </summary>
        public string des_wrktm { get; set; }

        /// <summary>
        /// کد شغل
        /// </summary>
        public string cod_job_emply { get; set; }

        /// <summary>
        /// عنوان شغل
        /// </summary>
        public string des_job { get; set; }

        /// <summary>
        /// کد رده شغل
        /// </summary>
        public string lkp_cod_class_job { get; set; }

        /// <summary>
        /// رده شغل
        /// </summary>
        public string des_lkp_cod_class_job { get; set; }

        /// <summary>
        /// سنوات داخل
        /// </summary>
        public string sen_int { get; set; }

        /// <summary>
        /// سنوات خارج
        /// </summary>
        public string sen_out { get; set; }

        /// <summary>
        /// سنوات تطبیقی
        /// </summary>
        public string tatbigh_all { get; set; }

        /// <summary>
        /// کد معاونت
        /// </summary>
        public string cod_movenat { get; set; }

        /// <summary>
        /// شرح معاونت
        /// </summary>
        public string des_movenat { get; set; }

        /// <summary>
        /// حالت اشتغال
        /// </summary>
        public string pycnd_cod_sta_pycnd { get; set; }

        /// <summary>
        /// شرح حالت اشتغال
        /// </summary>
        public string des_pycnd { get; set; }

        /// <summary>
        /// کد حالت استخدام
        /// </summary>
        public string lkp_cod_emplt_typ_emply { get; set; }

        /// <summary>
        /// شرح حالت استخدام
        /// </summary>
        public string des_cod_emplt { get; set; }

        /// <summary>
        /// کد وضعیت پاداش
        /// </summary>
        public string lkp_grp_obg_emply { get; set; }

        /// <summary>
        /// شرح وضعیت پاداش
        /// </summary>
        public string des_lkp_grp_obg_emply { get; set; }

        /// <summary>
        /// کد تیم
        /// </summary>
        public string num_team_teams { get; set; }

        /// <summary>
        /// کد تیم
        /// </summary>
        //public string? cod_team_teams { get; set; }

        /// <summary>
        /// امضا
        /// </summary>
        public byte[] sign_prsn_emply { get; set; }





        /// <summary>
        /// ایدی واحد سازمانی
        /// </summary>
        public int? business_unit_id { get; set; }

        /// <summary>
        /// ایدی شغل
        /// </summary>
        public int? job_id { get; set; }

        /// <summary>
        /// ایدی معاونت
        /// </summary>
        public int? id_busun_moavenat { get; set; }

        /// <summary>
        /// ایدی پست
        /// </summary>
        public long position_id { get; set; }

        /// <summary>
        /// ایدی تیم
        /// </summary>
        public long? team_id { get; set; }

        /// <summary>
        /// ایدی مدیریت
        /// </summary>
        public long? id_busun_manage { get; set; }
        /// <summary>
        /// کد مدیریت
        /// </summary>
        public string cod_manage { get; set; }
        /// <summary>
        /// شرح مدیریت
        /// </summary>
        public string des_manage { get; set; }

        /// <summary>
        /// user view Hash code for sync with is-suite
        /// </summary>
        public string cod_hash { get; set; }



    }














    /// <summary>
    /// IS-SUITE اطلاعات کاربر
    /// بدون عکس و امضا
    /// </summary>
    public class UserInfoNoImageDto
    {
        public long id { get; set; }
        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string num_prsn_emply { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string nam_first_emply { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string nam_last_emply { get; set; }

        /// <summary>
        /// کد ملی
        /// </summary>

        public string cod_nat_emply { get; set; }

        /// <summary>
        /// کد واحد سازمانی
        /// </summary>
        public string cod_busun_emply { get; set; }

        /// <summary>
        /// شرح واحد سازمانی
        /// </summary>
        public string des_busun { get; set; }

        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string cod_cc_emply { get; set; }

        /// <summary>
        /// شرح مرکز هزینه
        /// </summary>
        public string des_cc { get; set; }

        /// <summary>
        /// کد جنسیت
        /// </summary>
        public string lkp_cod_sex_emply { get; set; }

        /// <summary>
        /// عنوان جنسیت
        /// </summary>
        public string des_lkp_cod_sex_emply { get; set; }

        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? dat_birth_emply { get; set; }

        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public string lkp_cod_mrid_emply { get; set; }

        /// <summary>
        /// عنوان وضعیت تاهل
        /// </summary>
        public string des_lkp_cod_mrid_emply { get; set; }

        /// <summary>
        /// تعداد فرزندان
        /// </summary>
        public int? tot_child_emply { get; set; }

        /// <summary>
        /// نام پدر
        /// </summary>
        public string nam_fathr_emply { get; set; }

        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string nam_first_eng_emply { get; set; }

        /// <summary>
        /// نام خانوادگی انگلیسی
        /// </summary>
        public string nam_last_eng_emply { get; set; }

        /// <summary>
        /// کد محل تولد
        /// </summary>
        public long? gepos_geographic_position_id_a { get; set; }

        /// <summary>
        /// محل تولد
        /// </summary>
        public string des_gepos_geog_position_id_a { get; set; }

        /// <summary>
        /// کد محل صدور
        /// </summary>
        public long? gepos_geographic_position_id_b { get; set; }

        /// <summary>
        /// محل صدور
        /// </summary>
        public string des_gepos_geog_position_id_b { get; set; }

        /// <summary>
        /// تاریخ صدور شناسنامه
        /// </summary>
        public DateTime? dat_crt_emply { get; set; }

        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string num_crt_emply { get; set; }

        /// <summary>
        /// شماره سریال شناسنامه
        /// </summary>
        public string num_ser_crt_emply { get; set; }

        /// <summary>
        /// کد ملیت
        /// </summary>
        public string lkp_cod_natlty_emply { get; set; }

        /// <summary>
        /// عنوان ملیت
        /// </summary>
        public string des_lkp_cod_natlty_emply { get; set; }

        /// <summary>
        /// کد مذهب
        /// </summary>
        public string lkp_cod_rlgn_emply { get; set; }

        /// <summary>
        /// عنوان مذهب
        /// </summary>
        public string des_lkp_cod_rlgn_emply { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string num_mobil_emply { get; set; }

        /// <summary>
        /// شماره تلفن منزل
        /// </summary>
        public string num_tel_emply { get; set; }


        /// <summary>
        /// شماره تلفن اضطرازی
        /// </summary>
        public string num_tel_emrgy_emply { get; set; }

        /// <summary>
        /// تلفن داخلی 1
        /// </summary>
        public string num_tel_int_emply { get; set; }

        /// <summary>
        /// تلفن داخلی 2
        /// </summary>
        public string num_tel_int2_emply { get; set; }

        /// <summary>
        /// آدرس پست الکترونيکي
        /// </summary>
        public string des_email_emply { get; set; }

        /// <summary>
        /// کد ناحیه
        /// </summary>
        public string lkp_cod_area_busun { get; set; }

        /// <summary>
        /// شرح ناحیه
        /// </summary>
        public string des_lkp_cod_area_busun { get; set; }

        /// <summary>
        /// مسئول بالاتر
        /// </summary>
        public string up_lvl_emp { get; set; }

        /// <summary>
        /// کد پست
        /// </summary>
        public string cod_post_emply { get; set; }

        /// <summary>
        ///  عنوان پست
        /// </summary>
        public string des_per_postd { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) کد دسته بندی پرسنل
        /// </summary>
        public string lkp_sta_emplt_emply { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) شرح دسته بندی پرسنل
        /// </summary>
        public string des_lkp_sta_emplt_emply { get; set; }

        /// <summary>
        ///  کد زمان کار
        /// </summary>
        public string lkp_cod_wrktm { get; set; }

        /// <summary>
        ///  عنوان زمان کار
        /// </summary>
        public string des_wrktm { get; set; }

        /// <summary>
        /// کد شغل
        /// </summary>
        public string cod_job_emply { get; set; }

        /// <summary>
        /// عنوان شغل
        /// </summary>
        public string des_job { get; set; }

        /// <summary>
        /// کد رده شغل
        /// </summary>
        public string lkp_cod_class_job { get; set; }

        /// <summary>
        /// رده شغل
        /// </summary>
        public string des_lkp_cod_class_job { get; set; }

        /// <summary>
        /// سنوات داخل
        /// </summary>
        public string sen_int { get; set; }

        /// <summary>
        /// سنوات خارج
        /// </summary>
        public string sen_out { get; set; }

        /// <summary>
        /// سنوات تطبیقی
        /// </summary>
        public string tatbigh_all { get; set; }

        /// <summary>
        /// کد معاونت
        /// </summary>
        public string cod_movenat { get; set; }

        /// <summary>
        /// شرح معاونت
        /// </summary>
        public string des_movenat { get; set; }

        /// <summary>
        /// حالت اشتغال
        /// </summary>
        public string pycnd_cod_sta_pycnd { get; set; }

        /// <summary>
        /// شرح حالت اشتغال
        /// </summary>
        public string des_pycnd { get; set; }

        /// <summary>
        /// کد حالت استخدام
        /// </summary>
        public string lkp_cod_emplt_typ_emply { get; set; }

        /// <summary>
        /// شرح حالت استخدام
        /// </summary>
        public string des_cod_emplt { get; set; }

        /// <summary>
        /// کد وضعیت پاداش
        /// </summary>
        public string lkp_grp_obg_emply { get; set; }

        /// <summary>
        /// شرح وضعیت پاداش
        /// </summary>
        public string des_lkp_grp_obg_emply { get; set; }

        /// <summary>
        /// کد تیم
        /// </summary>
        public string num_team_teams { get; set; }

        /// <summary>
        /// ایدی واحد سازمانی
        /// </summary>
        public int? business_unit_id { get; set; }

        /// <summary>
        /// ایدی شغل
        /// </summary>
        public int? job_id { get; set; }

        /// <summary>
        /// ایدی معاونت
        /// </summary>
        public int? id_busun_moavenat { get; set; }

        /// <summary>
        /// ایدی پست
        /// </summary>
        public long position_id { get; set; }

        /// <summary>
        /// ایدی تیم
        /// </summary>
        public long? team_id { get; set; }

        /// <summary>
        /// ایدی مدیریت
        /// </summary>
        public long? id_busun_manage { get; set; }
        /// <summary>
        /// کد مدیریت
        /// </summary>
        public string cod_manage { get; set; }
        /// <summary>
        /// شرح مدیریت
        /// </summary>
        public string des_manage { get; set; }


    }
}
