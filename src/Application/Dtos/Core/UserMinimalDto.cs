using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    /// <summary>
    /// نام و عکس کاربر برای هدر سایت
    /// </summary>
    public class UserMinimalDto
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string ProfileImage { get; set; }


        /// <summary>
        ///  عنوان پست
        /// </summary>
        public string DesPost { get; set; }

        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string FirstNameEng { get; set; }

        /// <summary>
        /// نام خانوادگی انگلیسی
        /// </summary>
        public string LastNameEng { get; set; }
        /// <summary>
        /// نقشها
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// راهنمای پروفایل حرفه ای را مشاهده کرده یا نه
        /// </summary>
        public bool? SeenProfileGuide { get; set; }
        /// <summary>
        /// پسورد پیشفرض
        /// </summary>
        public bool IsPasswordChangeForce { get; set; }
        public string PasswordChangeForceMsg { get; set; }

        public string EmploymentTypeVal { get; set; }
        public string EmploymentTypeFarsi { get; set; }
    }
}

