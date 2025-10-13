using ContractorBackend.Application.Core.Account.Commands.LoginAccount;
using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.LoginSecondStep
{
    public class LoginSecondStepCommandValidartor : AbstractValidator<LoginAccountCommand>
    {
        public LoginSecondStepCommandValidartor(IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithName("نام کاربری")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

            RuleFor(x => x.Password)
                .NotNull()
                .WithName("کلمه عبور")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

            RuleFor(x => x.Captcha)
             .NotNull()
             .WithName("OTP")
             .WithMessage(_localizer["EnterProperty"])
             .NotEmpty()
             .WithMessage(_localizer["CantEmpty"]);

        }
    }
}
