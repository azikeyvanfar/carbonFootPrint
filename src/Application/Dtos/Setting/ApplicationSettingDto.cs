using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Setting
{
    public class ApplicationSettingDto
    {
        /// <summary>
        /// Is For Admin Panel or NOT
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// تعداد روزهایی که کاربر لاگین نکرده و بعد از ان اطلاعاتش سینک میشود
        /// </summary>
        public int thresholdDays { get; set; }


        #region OTP
        /// <summary>
        /// فلگ فعالسازی otp سمت پروژه
        /// </summary>
        public bool ActiveOtp { get; set; }
        /// <summary>
        /// زمان فعال بودن کد otp گرفته شده به دقیقه
        /// </summary>
        public int ActiveTimeOtp { get; set; }
        /// <summary>
        /// زمان التظار تایید کد otp به دقیقه
        /// </summary>
        public int WaitConfirmTimeOtp { get; set; }
        #endregion


        #region RateLimit
        /// <summary>
        /// بازه زمانی به دقیقه
        /// </summary>
        public int RateLimitTimeWindow { get; set; }
        /// <summary>
        /// تعداد درخواست در بازه زمانی
        /// </summary>
        public int RateLimitMaxRequests { get; set; }
        /// <summary>
        /// قوانین کاستومایز شده بر اساس ادرس اکشن یا هدر درخواست
        /// </summary> 
        public List<RateLimitCustomRuleDto> RateLimitCustomRules { get; set; }

        #endregion


        /// <summary>
        /// فلگ لاگین دو مرحله ای
        /// </summary>
        public bool TwoStepLogin { get; set; }



        #region User Configs
        /// <summary>
        /// غیرفعال کردن یوزر پس از n روز لاگین نکردن
        /// </summary>
        public int DeActiveUserAfterDays { get; set; }
        /// <summary>
        /// غیرفعال کردن کاربر پس از n  بار تلاش ناموفق ورود
        /// </summary>
        public int MaxAccessFailedDeActive { get; set; }
        /// <summary>
        /// اجازه ندادن به کاربر برای انتخاب دوباره n تعداد پسورد های قبلی
        /// </summary>
        public int NotAllowedPreviouslyUsedPasswords { get; set; }
        /// <summary>
        /// اجازه ندادن به کاربر برای تغییر پسورد پس از گذشت n از تغییر پسورد قبلی
        /// </summary>
        public int NotAllowedCountHourChangePass { get; set; }
        /// <summary>
        /// اجبار به تغییر پسورد پس از n روز
        /// </summary>
        public int ChangePasswordReminderDays { get; set; }
        #endregion



        #region CPM Settings
        public Guid? SIChecklistId { get; set; }
        #endregion

    }
}
