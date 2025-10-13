using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Core.Account.Commands.LogoutAccount;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.CurrentUserPassChange
{
    public class CurrentUserPassChangeCommand : IRequest<bool>
    {
        public string OldPass { get; set; }
        [MaxLength(64)]
        public string NewPass { get; set; }
        [MaxLength(64)]
        public string ReNewPass { get; set; }
        public RoleType RoleType { get; set; }
    }

    public class CurrentUserPassChangeCommandHandler : IRequestHandler<CurrentUserPassChangeCommand, bool>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ISmsSender _smsSender;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationUserManager _userManager;
        private readonly IUsedPasswordsService _usedPasswordsService;
        public CurrentUserPassChangeCommandHandler(IApplicationUserManager userManager, IStringLocalizer<SharedResource> localizer,
            ISmsSender smsSender, IHttpContextAccessor accessor, IUsedPasswordsService usedPasswordsService)
        {
            _userManager = userManager;
            _localizer = localizer;
            _smsSender = smsSender;
            _accessor = accessor;
            _usedPasswordsService = usedPasswordsService;
        }
        public async Task<bool> Handle(CurrentUserPassChangeCommand request, CancellationToken cancellationToken)
        {
            var res = false;
            var P_NUM_PRSN = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            var user = _userManager.FindByPersonnelCode(P_NUM_PRSN);

            //var listPass = _userUsedPass.GetAll().Where(_ => _.UserId == user.Id).Max(x => EF.Property<DateTime>(x, "CreatedDateTime"));

            if (user is null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }
            else
            {
                //تکرار پسورد اشتباه
                if (!request.NewPass.Equals(request.ReNewPass))
                {
                    throw new CustomException(_localizer["PasswordNotConfirmRepeated"]);
                }
                //پسورد جدید و فعلی
                if (request.OldPass.Equals(request.NewPass))
                {
                    throw new CustomException(_localizer["NewPassAndCurrentCantEqual"]);
                }
                //پسورد جاری اشتباه
                var oldresVerify = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPass);
                if (oldresVerify != PasswordVerificationResult.Success)
                {
                    throw new CustomException(_localizer["OldPassNotCorrect"]);
                }
                else
                {
                    //تعداد مشخص از پسوردهای قبلی را نتواند انتخاب کند
                    if (await _usedPasswordsService.IsPreviouslyUsedPasswordAsync(user, request.NewPass))
                    {
                        throw new CustomException(_localizer["NewPassAndCurrentCantEqual"]);
                    }
                    //در یک روز یکبار
                    var MaxDate = await _usedPasswordsService.GetLastUserPasswordChangeDateAsync(user.Id);
                    if (MaxDate.HasValue)
                    {
                        if (DateOnly.FromDateTime((DateTime)MaxDate) == DateOnly.FromDateTime(DateTime.Now))
                        {
                            throw new CustomException(_localizer["CantChangePasswordMoreThanOneInDay"]);
                        }
                    }

                    var changePass = await _userManager.ChangePasswordAsync(user, request.OldPass, request.NewPass);
                    if (changePass.Succeeded)
                    {
                        res = true;

                        if (user.IsPasswordChangeForce)
                        {
                            user.IsPasswordChangeForce = false;
                            await _userManager.UpdateAsync(user);
                        }

                        await _smsSender.SendSmsAsync(user,
                        new SmsRequest
                        {
                            Receivers = user.PhoneNumber,
                            SmsText = _localizer["ChangePasswordConfirm"],
                            IsPassword = 1,
                            SenderId = "otp"
                        }, SmsType.ChangePassword);


                        ISender Mediator = _accessor.HttpContext.RequestServices.GetService<ISender>()!;
                        await Mediator.Send(new LogoutAccountCommand(""));

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