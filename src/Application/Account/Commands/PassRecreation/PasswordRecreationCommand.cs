using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Account.Commands.PassRecreation
{
    public class PasswordRecreationCommand : IRequest<bool>
    {

        public string? NationalCode { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string? PhoneNumber { get; set; }
        [MaxLength(64)]
        public string? Pass { get; set; }
        [MaxLength(64)]
        public string? RePass { get; set; }

        public string? OTP { get; set; }
    }

    public class PasswordRecreationCommandHandler :
        IRequestHandler<PasswordRecreationCommand, bool>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IConfiguration _config;
        private readonly ISmsSender _smsSender;
        private readonly IApplicationUserManager _userManager;
        private readonly IUsedPasswordsService _usedPasswordsService;
        public PasswordRecreationCommandHandler(IApplicationUserManager userManager, IStringLocalizer<SharedResource> localizer, ISmsSender smsSender, IUsedPasswordsService usedPasswordsService, IConfiguration config)
        {
            _userManager = userManager;
            _localizer = localizer;
            _smsSender = smsSender;
            _usedPasswordsService = usedPasswordsService;
            _config = config;
        }
        public async Task<bool> Handle(PasswordRecreationCommand request, CancellationToken cancellationToken)
        {
            var res = false;
            var user = await _userManager.Users
                .FirstOrDefaultAsync(_ =>
                        _.IsActive &&
                        _.NationalCode == request.NationalCode &&
                        _.PhoneNumber == request.PhoneNumber,
                        cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }
            else
            {
                if (!request.Pass.Equals(request.RePass))
                {
                    throw new CustomException(_localizer["PasswordNotConfirmRepeated"]);
                }
                if (!user.OTP.Equals(request.OTP))
                {
                    throw new CustomException(_localizer["OtpWrong"]);
                }
                if (user.OTP.Equals(request.OTP) && user.OtpCreationDate.Value.AddMinutes(10) < DateTime.UtcNow)
                {
                    throw new CustomException(_localizer["OtpTimeOut"]);
                }
                else
                {
                    //تعداد مشخص از پسوردهای قبلی را نتواند انتخاب کند
                    if (await _usedPasswordsService.IsPreviouslyUsedPasswordAsync(user, request.Pass))
                    {
                        throw new CustomException(_localizer["NewPassAndCurrentCantEqual"]);
                    }
                    //در یک روز یکبار
                    var MaxDate = await _usedPasswordsService.GetLastUserPasswordChangeDateAsync(user.Id);
                    if (MaxDate.HasValue)
                    {
                        var CountHourChangePass = _config.GetSection("NotAllowedCountHourChangePass").Value == null ? 0 : Convert.ToDouble(_config.GetSection("NotAllowedCountHourChangePass").Value);
                        //if (DateOnly.FromDateTime((DateTime)MaxDate.Value.AddHours(CountHourChangePass)) == DateOnly.FromDateTime(DateTime.Now))
                        if ((DateTime)MaxDate.Value.AddHours(CountHourChangePass) >= DateTime.Now)
                        {
                            throw new CustomException(_localizer["CantChangePasswordMoreThanOneInDay", CountHourChangePass]);
                        }
                    }
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var changePass = await _userManager.ResetPasswordAsync(user, token, request.Pass);
                    if (changePass.Succeeded)
                    {
                        res = true;

                        user.IsPasswordChangeForce = false;
                        await _userManager.UpdateAsync(user);

                        await _smsSender.SendSmsAsync(user,
                        new SmsRequest
                        {
                            Receivers = user.PhoneNumber,
                            SmsText = _localizer["ChangePasswordConfirm"],
                            IsPassword = 1,
                            SenderId = "otp"
                        }, SmsType.ChangePassword);
                        return res;
                    }
                    else
                    {
                        var msgError = "";
                        foreach (var err in changePass.Errors)
                        {
                            msgError += "* " + err.Description + "  \n";
                        }
                        throw new CustomException(msgError);
                    }
                }
            }
            return res;
        }
    }
}