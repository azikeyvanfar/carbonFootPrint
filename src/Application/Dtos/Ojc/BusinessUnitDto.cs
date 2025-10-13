using System;

namespace ContractorBackend.Application.Dtos.Ojc
{
    public class BusinessUnitDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;



        /// <summary>
        /// ایدی واحد
        /// </summary>
        public long? IsSuiteId { get; set; }
        /// <summary>
        /// کد واحد
        /// </summary>
        public string cod_busun { get; set; }

        /// <summary>
        /// شرح واحد
        /// </summary>
        public string des_busun { get; set; }

        /// <summary>
        /// کد ناحیه
        /// </summary>
        public string lkp_cod_area_busun { get; set; }

        /// <summary>
        /// شرح ناحیه
        /// </summary>
        public string des_lkp_cod_area_busun { get; set; }

        /// <summary>
        /// کد معاونت / مدیریت
        /// </summary>
        public string lkp_typ_astnt_busun { get; set; }

        /// <summary>
        /// شرح معاونت / مدیریت
        /// </summary>
        public string des_lkp_typ_astnt_busun { get; set; }

        /// <summary>
        /// تاریخ شروع اعتبار
        /// </summary>
        public DateTime dat_str_busun { get; set; }

        /// <summary>
        /// تاریخ پایان اعتبار
        /// </summary>
        public DateTime? dat_end_busun { get; set; }

        /// <summary>
        ///  آیدی واحد پدر
        /// </summary>
        public long? busun_business_unit_id { get; set; }

        /// <summary>
        /// کد واحد پرنت-پدر
        /// </summary>
        public string cod_busun_up { get; set; }

        /// <summary>
        /// شرح واحد پرنت
        /// </summary>
        public string des_busun_up { get; set; }


        /// <summary>
        /// ایدی مرکز هزینه
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

        ///// <summary>
        ///// فعال/غیرفعال بودن واحد
        ///// </summary>
        //public bool flg_is_active { get; set; } IsActive

        /// <summary>
        /// فعال/غیرفعال بودن درخواست واحد
        /// </summary>
        public bool flg_is_req_active { get; set; }


        public DateTimeOffset? CreatedDateTime { get; set; }
        public DateTimeOffset? ModifiedDateTime { get; set; }
    }
}
