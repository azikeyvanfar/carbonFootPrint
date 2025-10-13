using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.CreateOtpSettingWithPhone
{
    public class CreateOtpSettingWithPhoneCommand : IRequest<bool>
    {
        public bool IsActive { get; set; } = true;
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Otp { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset WaitConfirmTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset ActiveTime { get; set; }

        public string PersonnelCode { get; set; }
        [Required]
        public OtpType otpType { get; set; } = OtpType.SpecialPage;
        public string SmsText { get; set; }
        public int IsPassword { get; set; } = 1;
    }

    public class CreateOtpSettingWithPhoneCommandHandler : IRequestHandler<CreateOtpSettingWithPhoneCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<OtpSetting> _repository;
        private readonly ISmsSender _smsSender;
        private readonly IConfiguration _configuration;

        public CreateOtpSettingWithPhoneCommandHandler(IMapper mapper, IRepository<OtpSetting> OtpSettingRepo, ISmsSender smsSender, IConfiguration configuration)
        {
            _mapper = mapper;
            _repository = OtpSettingRepo;
            _smsSender = smsSender;
            _configuration = configuration;
        }
        public Task<bool> Handle(CreateOtpSettingWithPhoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool flag = false;

                request.ActiveTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("ActiveTimeOtp").Value));
                request.WaitConfirmTime = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("WaitConfirmTimeOtp").Value));
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

                if (smsText.Contains("A"))
                {
                    smsText = smsText.Replace("A", request.Otp);
                }
                else
                {
                    smsText = smsText + request.Otp;
                }
                _smsSender.SendSmsAsync(
                    new SmsRequest
                    {
                        Receivers = request.PhoneNumber,
                        SmsText = smsText,
                        IsPassword = request.IsPassword,
                        SenderId = "otp"
                    }, SmsType.OTP);
                #endregion
                return Task.FromResult(flag);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}