using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos
{
    /// <summary>
    /// اطلاعات کامل کاربر
    /// </summary>
    public class UserVM
    {
        public long Id { get; set; }
        public string PersonnelCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public DateTime? BirthDate { get; set; }
        public string? BirthDateOnly { get; set; }


        public string Email { get; set; }
        public string AssistanceName { get; set; }

        public bool IsCurrentlyShared { get; set; }

        public string PhoneNumber { get; set; }

        public string ProfileImage { get; set; }

        /// <summary>
        /// جنسیت
        /// </summary>
        public string? DesSex { get; set; }

        /// <summary>
        /// عنوان وضعیت تاهل
        /// </summary>
        public string? DesMarid { get; set; }

        /// <summary>
        ///  عنوان پست
        /// </summary>
        public string? DesPost { get; set; }


        /// <summary>
        /// رده شغل
        /// </summary>
        public string? DesCategoryJob { get; set; }

        /// <summary>
        /// سنوات داخل
        /// </summary>
        public string? InYearsCount { get; set; }

        /// <summary>
        /// سنوات خارج
        /// </summary>
        public string? OutYearsCount { get; set; }

        /// <summary>
        /// سنوات تطبیقی
        /// </summary>
        public string? AllMatchYears { get; set; }

        /// <summary>
        /// کد معاونت
        /// </summary>
        public string? CodAssistance { get; set; }

        /// <summary>
        /// شرح معاونت
        /// </summary>
        public string? DesAssistance { get; set; }


        /// <summary>
        /// کد ملی
        /// </summary>
        public string NationalCode { get; set; }

        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string? FirstNameEng { get; set; }

        /// <summary>
        /// نام خانوادگی انگلیسی
        /// </summary>
        public string? LastNameEng { get; set; }
        /// <summary>
        /// شرح مرکز هزینه
        /// </summary>
        public string? DesCostCenter { get; set; }


        public List<string> RoleNames { get; set; }
        public List<long> RoleIds { get; set; }




    }


}
