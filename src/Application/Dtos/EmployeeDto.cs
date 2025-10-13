using System;

namespace ContractorBackend.Domain.Entities
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;


        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string? PersonnelCode { get; set; }
        /// <summary>
        /// نام
        /// </summary> 
        public string? FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary> 
        public string? LastName { get; set; }
        ///// <summary>
        ///// شماره موبایل
        ///// </summary>
        //public long? PhoneNumber { get; set; }  // به کاربران نمایش داده نشود
        /// <summary>
        /// کد ملی
        /// </summary>
        public string? NationalCode { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// کد تیم
        /// </summary>
        public string? CodTeam { get; set; }
        /// <summary>
        /// ایدی تیم
        /// </summary>
        public long? TeamId { get; set; }
        /// <summary>
        /// سن
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// کد شغل
        /// </summary>
        public string? CodJob { get; set; }
        /// <summary>
        /// عنوان شغل
        /// </summary>
        public string? DesJob { get; set; }
        /// <summary>
        /// کد تحصیلات
        /// </summary>
        public string? CodEducation { get; set; }
        /// <summary>
        /// عنوان تحصیلات
        /// </summary>
        public string? DesEducation { get; set; }
        /// <summary>
        /// سال سنوات کاری
        /// </summary>
        public int? WorkHistoryYear { get; set; }
        /// <summary>
        /// ماه سنوات کاری
        /// </summary>
        public int? WorkHistorymonth { get; set; }
        /// <summary>
        /// شماره قرارداد
        /// </summary>
        public int? num_ord_empca { get; set; }
        /// <summary>
        /// شماره قرارداد
        /// </summary>
        public int? ContractNumber { get; set; }
        /// <summary>
        /// کد شرکت
        /// </summary>
        public string ContractorCode { get; set; }


        /// <summary>
        /// عکس فرد
        /// </summary>
        public byte[]? Image { get; set; }


    }
}
