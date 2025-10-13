using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Shared;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Domain.Entities.Identity
{
    public class User : IdentityUser<long>, IEntity<long>, IAuditableEntity
    {
        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string PersonnelCode { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [StringLength(450)]
        public string? FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [StringLength(450)]
        public string? LastName { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string NationalCode { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string? FirstNameEng { get; set; }
        /// <summary>
        /// نام خانوادگی انگلیسی
        /// </summary>
        public string? LastNameEng { get; set; }
        /// <summary>
        /// آدرس پست الکترونيکي
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// مسئول بالاتر
        /// </summary>
        public string? UpLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastVisitDateTime { get; set; }
        public DateTimeOffset? LastLoggedIn { get; set; }
        public string OTP { get; set; }
        public DateTime? OtpCreationDate { get; set; }

        /// <summary>
        /// پسورد پیشفرض
        /// </summary>
        public bool IsPasswordChangeForce { get; set; } = true;
        public int AccessFailedDeActive { get; set; } = 0;

        public Guid? EmployeeId { get; set; }


        /// <summary>
        /// پیغام ریست پسورد
        /// </summary>

        public string? PasswordChangeForceMsg { get; set; }


        #region Navigation
        //public UserDetail UserDetails { get; set; }
        //public Employee? Employee { get; set; }
        public virtual ICollection<UserUsedPassword> UserUsedPasswords { get; set; } = new List<UserUsedPassword>();
        public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
        public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
        public virtual ICollection<JwtUserToken> JwtUserTokens { get; set; } = new List<JwtUserToken>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public virtual ICollection<SmsHistory> SmsHistories { get; set; } = new List<SmsHistory>();
        // public virtual ICollection<AssessmentScheduling> AssessmentSchedulings { get; set; } = new HashSet<AssessmentScheduling>();
        public virtual ICollection<QuestionAnswer> QuestionAnswerQuestioner { get; set; } = new List<QuestionAnswer>();
        public virtual ICollection<QuestionAnswer> QuestionAnswerRespond { get; set; } = new List<QuestionAnswer>();
        // public virtual ICollection<RiskFacilitationAccess> RiskFacilitationAccesses { get; set; } = new List<RiskFacilitationAccess>();
        #endregion
    }
}