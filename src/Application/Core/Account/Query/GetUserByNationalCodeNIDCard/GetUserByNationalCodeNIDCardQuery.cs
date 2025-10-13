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

namespace ContractorBackend.Application.Core.Account.Query.GetUserByNationalCodeNIDCard
{
    public class GetUserByNationalCodeNIDCardQuery : IRequest<bool>
    {
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Captcha { get; set; } = null!;
        public OtpService Service { get; set; }
        public bool WithCaptcha { get; set; } = true;

    }

    public class GetUserByNationalCodeNIDCardQueryHandler :
        IRequestHandler<GetUserByNationalCodeNIDCardQuery, bool>
    {
        private readonly IApplicationUserManager _userManager;
        private readonly ISmsSender _smsSender;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GetUserByNationalCodeNIDCardQueryHandler(
            IApplicationUserManager userManager,
            ISmsSender smsSender,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _localizer = localizer;
        }
        public async Task<bool> Handle(GetUserByNationalCodeNIDCardQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(_ =>
                    _.PhoneNumber == request.PhoneNumber &&
                    _.NationalCode == request.NationalCode &&
                    _.IsActive,
                cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }
            var otp = Utilities.GenerateOtp();

            user.OTP = otp;
            user.OtpCreationDate = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                throw new CustomException(_localizer["PhoneNumberNotFound"]);
            }

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
                    Receivers = user.PhoneNumber,
                    SmsText = _localizer["ForgotPasswordSMSText"] + user.OTP,
                    IsPassword = 1,
                    SenderId = "otp"
                }, SmsType.ForgetPassword);

            return updateResult.Succeeded;

        }
    }
}
