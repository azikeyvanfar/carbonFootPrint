using System;

namespace ContractorBackend.Application.Dtos.Core
{
    public class UserDetailDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        /// <summary>
        /// عكس فرد
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// کد واحد سازمانی
        /// </summary>
        public string CodBusun { get; set; }

        /// <summary>
        /// شرح واحد سازمانی
        /// </summary>
        public string DesBusun { get; set; }

        /// <summary>
        /// کد مرکز هزینه
        /// </summary>
        public string CodCostCenter { get; set; }

        /// <summary>
        /// شرح مرکز هزینه
        /// </summary>
        public string DesCostCenter { get; set; }

        /// <summary>
        /// کد جنسیت
        /// </summary>
        public string CodSex { get; set; }

        /// <summary>
        /// عنوان جنسیت
        /// </summary>
        public string DesSex { get; set; }

        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public string CodMarid { get; set; }

        /// <summary>
        /// عنوان وضعیت تاهل
        /// </summary>
        public string DesMarid { get; set; }

        /// <summary>
        /// تعداد فرزندان
        /// </summary>
        public int? ChildCount { get; set; }

        /// <summary>
        /// نام پدر
        /// </summary>
        public string FatherName { get; set; }

        /// <summary>
        /// کد محل تولد
        /// </summary>
        public long? CodBirthPosition { get; set; }

        /// <summary>
        /// محل تولد
        /// </summary>
        public string DesBirthPosition { get; set; }

        /// <summary>
        /// کد محل صدور
        /// </summary>
        public long? CodRegBirthPlacePosition { get; set; }

        /// <summary>
        /// محل صدور
        /// </summary>
        public string DesRegBirthPlacePosition { get; set; }

        /// <summary>
        /// تاریخ صدور شناسنامه
        /// </summary>
        public DateTime? CrtyDate { get; set; }

        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string CrtNum { get; set; }

        /// <summary>
        /// شماره سریال شناسنامه
        /// </summary>
        public string CrtSerNum { get; set; }

        /// <summary>
        /// کد ملیت
        /// </summary>
        public string CodeNationaly { get; set; }

        /// <summary>
        /// عنوان ملیت
        /// </summary>
        public string DesNationaly { get; set; }

        /// <summary>
        /// کد مذهب
        /// </summary>
        public string CodeReligion { get; set; }

        /// <summary>
        /// عنوان مذهب
        /// </summary>
        public string DesReligion { get; set; }

        /// <summary>
        /// شماره تلفن منزل
        /// </summary>
        public string TellNum { get; set; }

        /// <summary>
        /// شماره تلفن اضطرازی
        /// </summary>
        public string EmergencyTellNum { get; set; }

        /// <summary>
        /// تلفن داخلی 1
        /// </summary>
        public string InternalTellNumFirst { get; set; }

        /// <summary>
        /// تلفن داخلی 2
        /// </summary>
        public string InternalTellNumSec { get; set; }

        /// <summary>
        /// کد ناحیه
        /// </summary>
        public string CodeArea { get; set; }

        /// <summary>
        /// شرح ناحیه
        /// </summary>
        public string DesArea { get; set; }

        /// <summary>
        /// کد پست
        /// </summary>
        public string CodePost { get; set; }

        /// <summary>
        ///  عنوان پست
        /// </summary>
        public string DesPost { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) کد دسته بندی پرسنل
        /// </summary>
        public string CodEmployedStatus { get; set; }

        /// <summary>
        /// وضعیت (شاغل/غیرشاغل) شرح دسته بندی پرسنل
        /// </summary>
        public string DesEmployedStatus { get; set; }

        /// <summary>
        ///  کد زمان کار
        /// </summary>
        public string CodWorkTime { get; set; }

        /// <summary>
        ///  عنوان زمان کار
        /// </summary>
        public string DesWorkTime { get; set; }

        /// <summary>
        /// کد شغل
        /// </summary>
        public string CodJob { get; set; }

        /// <summary>
        /// عنوان شغل
        /// </summary>
        public string DesJob { get; set; }

        /// <summary>
        /// کد رده شغل
        /// </summary>
        public string CodCategoryJob { get; set; }

        /// <summary>
        /// رده شغل
        /// </summary>
        public string DesCategoryJob { get; set; }

        /// <summary>
        /// سنوات داخل
        /// </summary>
        public string InYearsCount { get; set; }

        /// <summary>
        /// سنوات خارج
        /// </summary>
        public string OutYearsCount { get; set; }

        /// <summary>
        /// سنوات تطبیقی
        /// </summary>
        public string AllMatchYears { get; set; }

        /// <summary>
        /// کد معاونت
        /// </summary>
        public string CodAssistance { get; set; }

        /// <summary>
        /// شرح معاونت
        /// </summary>
        public string DesAssistance { get; set; }

        /// <summary>
        /// حالت اشتغال
        /// </summary>
        public string CodEmploymentType { get; set; }

        /// <summary>
        /// شرح حالت اشتغال
        /// </summary>
        public string DesEmploymentType { get; set; }

        /// <summary>
        /// کد حالت استخدام
        /// </summary>
        public string CodRecuitmentType { get; set; }

        /// <summary>
        /// شرح حالت استخدام
        /// </summary>
        public string DesRecuitmentType { get; set; }

        /// <summary>
        /// کد وضعیت پاداش
        /// </summary>
        public string CodReward { get; set; }

        /// <summary>
        /// شرح وضعیت پاداش
        /// </summary>
        public string DesReward { get; set; }

        /// <summary>
        /// کد تیم
        /// </summary>
        public string CodTeam { get; set; }

        /// <summary>
        /// امضا
        /// </summary>
        public byte[] Sign { get; set; }


        /// <summary>
        /// وضعیت اشتغال
        /// </summary>
        public string CodEmployeeStatus { get; set; }
        /// <summary>
        /// شرح وضعیت اشتغال
        /// </summary>
        public string DesEmployeeStatus { get; set; }

        /// <summary>
        /// ایدی واحد سازمانی
        /// </summary>
        public int? BusinessUnitId { get; set; }

        /// <summary>
        /// ایدی شغل
        /// </summary>
        public int? JobId { get; set; }

        /// <summary>
        /// ایدی معاونت
        /// </summary>
        public int? BusunMoavenatId { get; set; }

        /// <summary>
        /// ایدی پست
        /// </summary>
        public long PositionId { get; set; }

        /// <summary>
        /// ایدی تیم
        /// </summary>
        public long? TeamId { get; set; }

        /// <summary>
        /// ایدی مدیریت
        /// </summary>
        public long? BusunManageId { get; set; }
        /// <summary>
        /// کد مدیریت
        /// </summary>
        public string CodManage { get; set; }
        /// <summary>
        /// شرح مدیریت
        /// </summary>
        public string DesManage { get; set; }



    }
}
