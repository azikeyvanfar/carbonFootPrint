using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Dtos;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Persistence.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Core.ApplicationSettingPage.Commands.CreateApplicationSettingsClient
{

    public class CreateApplicationSettingsClientCommand : IRequest<bool>
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


    }


    public class CreateApplicationSettingClientCommandHandler : IRequestHandler<CreateApplicationSettingsClientCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HttpClientMethods _httpClient;
        private readonly IHttpContextAccessor _accessor;


        public CreateApplicationSettingClientCommandHandler(
            IApplicationDbContext dbContext,
            IMapper mapper,
            IConfiguration configuration,
            HttpClientMethods httpClient,
            IHttpContextAccessor accessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            _httpClient = httpClient;
            _accessor = accessor;
        }
        public async Task<bool> Handle(CreateApplicationSettingsClientCommand request, CancellationToken cancellationToken)
        {
            var removeList = _dbContext.ApplicationSettings.Include(x => x.RateLimitCustomRules).Where(x => x.IsAdmin == request.IsAdmin);
            if (removeList.Any())
            {
                _dbContext.ApplicationSettings.RemoveRange(removeList);
            }
            var entity = _mapper.Map<ApplicationSetting>(request);
            _dbContext.ApplicationSettings.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);


            #region Set Settings To appsettings.json
            _configuration["thresholdDays"] = entity.thresholdDays.ToString();
            _configuration["ActiveOtp"] = entity.ActiveOtp ? "true" : "false";   // only in client
            _configuration["ActiveTimeOtp"] = entity.ActiveTimeOtp.ToString();  // only in client
            _configuration["WaitConfirmTimeOtp"] = entity.WaitConfirmTimeOtp.ToString();  // only in client
            _configuration.GetSection("ApplicationSettings")["TwoStepLogin"] = entity.TwoStepLogin ? "true" : "false";
            _configuration["DeActiveUserAfterDays"] = entity.DeActiveUserAfterDays.ToString();
            _configuration["MaxAccessFailedDeActive"] = entity.MaxAccessFailedDeActive.ToString();
            _configuration["NotAllowedPreviouslyUsedPasswords"] = entity.NotAllowedPreviouslyUsedPasswords.ToString();
            _configuration["NotAllowedCountHourChangePass"] = entity.NotAllowedCountHourChangePass.ToString();
            _configuration["ChangePasswordReminderDays"] = entity.ChangePasswordReminderDays.ToString();


            _configuration.SetDefaultRateLimit(new RateLimitDecorator
            {
                TimeWindow = request.RateLimitTimeWindow,
                MaxRequests = request.RateLimitMaxRequests
            });

            var list = request.RateLimitCustomRules.Select(x => new RateLimitCustomRuleDto
            {
                IsHeader = x.IsHeader,
                Name = x.Name,
                RateLimitTimeWindow = x.RateLimitTimeWindow,
                RateLimitMaxRequests = x.RateLimitMaxRequests
            }).ToList();
            _configuration.SetRateLimitCustomRules(list);
            #endregion

            return true;
        }

    }
}