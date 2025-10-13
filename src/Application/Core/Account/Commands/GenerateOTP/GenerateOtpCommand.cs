using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.GenerateOTP
{
    public class GenerateOtpCommand : IRequest<bool>
    {
        public string Username { get; set; } = null!;
        public OtpService Service { get; set; }
    }

    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, bool>
    {
        private readonly IApplicationUserManager _userManager;
        private readonly ISmsSender _smsSender;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly int smsValidToInMinutes = 4;

        public GenerateOtpCommandHandler(
            IApplicationUserManager userManager,
            ISmsSender smsSender,
            IStringLocalizer<SharedResource> localizer
            )
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _localizer = localizer;
        }
        public async Task<bool> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(_ => _.UserName == request.Username, cancellationToken: cancellationToken);

            if (user is null)
            {
                throw new CustomException(_localizer["WrongUsernamePassword"]);
            }

            //if (user.OtpCreationDate.HasValue && user.OtpCreationDate.Value.AddMinutes(smsValidToInMinutes) > DateTime.UtcNow)
            //{
            //    var otp = Utilities.GenerateOtp();

            //    user.OTP = otp;
            //    user.OtpCreationDate = DateTime.UtcNow;

            //    var updateResult = await _userManager.UpdateAsync(user);

            //    var number = "";
            //    if (request.Service == OtpService.SMS)
            //    {

            //        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            //        {
            //            throw new CustomException(_localizer["PhoneNumberNotFound"]);
            //        }
            //        number = user.PhoneNumber;
            //    }
            //    if (request.Service == OtpService.MyMsc)
            //    {
            //        number = user.PersonnelCode;
            //    }

            //    await _smsSender.SendSmsAsync(user,
            //        new SmsRequest
            //        {
            //            Receivers = number,
            //            SmsText = _localizer["loginOtpText"] + user.OTP,
            //            IsPassword = 1
            //        }, Domain.Enums.SmsType.ForgetPassword);


            //    return updateResult.Succeeded;
            //}

            //if (!user.OtpCreationDate.HasValue || (user.OtpCreationDate.HasValue && user.OtpCreationDate.Value.AddMinutes(smsValidToInMinutes) < DateTime.UtcNow))
            //{
            var otp = Utilities.GenerateOtp();

            user.OTP = otp;
            user.OtpCreationDate = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);

            var number = "";
            if (request.Service == OtpService.SMS)
            {

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    throw new CustomException(_localizer["PhoneNumberNotFound"]);
                }
                number = user.PhoneNumber;
            }
            if (request.Service == OtpService.MyMsc)
            {
                number = user.PersonnelCode;
            }

            await _smsSender.SendSmsAsync(user,
                new SmsRequest
                {
                    Receivers = number,
                    SmsText = _localizer["loginOtpText"] + user.OTP,
                    IsPassword = 1,
                    SenderId = "otp"
                }, SmsType.ForgetPassword);


            return updateResult.Succeeded;
            //}

            //return false;
        }
    }
}
