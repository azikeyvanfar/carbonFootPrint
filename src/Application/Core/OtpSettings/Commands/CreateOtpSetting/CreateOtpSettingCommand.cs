using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.CreateOtpSetting
{
    public class CreateOtpSettingCommand : IRequest<bool>
    {
        public bool IsActive { get; set; }

        [Required]
        public string Otp { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset WaitConfirmTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset ActiveTime { get; set; }

        public string HashToken { get; set; }

        [Required]
        public OtpType otpType { get; set; } = OtpType.SpecialPage;
        public string SmsText { get; set; }
        public int IsPassword { get; set; } = 1;
    }

    public class CreateOtpSettingCommandHandler : IRequestHandler<CreateOtpSettingCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<OtpSetting> _repository;
        private readonly ISmsSender _smsSender;
        private readonly IApplicationUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityService _securityService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CreateOtpSettingCommandHandler(IMapper mapper, IRepository<OtpSetting> OtpSettingRepo, ISmsSender smsSender, IApplicationUserManager userManager,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ISecurityService securityService,
            IStringLocalizer<SharedResource> localizer)
        {
            _mapper = mapper;
            _repository = OtpSettingRepo;
            _smsSender = smsSender;
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
            _localizer = localizer;
        }
        public Task<bool> Handle(CreateOtpSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _userManager.GetCurrentUser();
                string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                bool flag = false;
                if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();

                    request.HashToken = _securityService.GetSha256Hash(token);
                    if (request.otpType == OtpType.SpecialPage)
                    {
                        request.ActiveTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("ActiveTimeOtp").Value));
                        request.WaitConfirmTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("WaitConfirmTimeOtp").Value));
                    }
                    else
                    {
                        request.ActiveTime = DateTime.Now.AddMinutes(Convert.ToDouble(1));
                    }
                    //otp ghabli ra ba vojode time hazf mikonim
                    var otpsetOld = _repository.GetAllAsNoTracking().Where(_ => _.IsActive == false && _.OtpType == request.otpType).OrderByDescending(_ => _.WaitConfirmTime).FirstOrDefault();
                    if (otpsetOld != null)
                    {
                        _repository.Delete(otpsetOld);
                    }
                    request.IsActive = false;
                    request.Otp = Utilities.GenerateOtp();

                    var entity = _mapper.Map<OtpSetting>(request);
                    flag = _repository.InsertEntity(entity);

                    #region sms
                    var smsText = request.SmsText;
                    if (request.otpType == OtpType.SpecialPage)
                    {
                        smsText = _localizer["pageOtpText"].Value + request.Otp;
                    }
                    else
                    {
                        if (smsText.Contains("A"))
                        {
                            smsText = smsText.Replace("A", request.Otp);
                        }
                        else
                        {
                            smsText = smsText + request.Otp;
                        }
                    }
                    _smsSender.SendSmsAsync(user,
                        new SmsRequest
                        {
                            Receivers = user.PhoneNumber,
                            SmsText = smsText,
                            IsPassword = request.IsPassword,
                            SenderId = "otp"
                        }, SmsType.OTP);
                    #endregion
                }
                return Task.FromResult(flag);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}