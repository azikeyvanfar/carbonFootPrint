using ContractorBackend.Application.Account.Commands.LoginAccount;
using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Account.Commands.LoginFirstStep
{
    public class LoginFirstStepCommandValidator : AbstractValidator<LoginAccountCommand>
    {
        public LoginFirstStepCommandValidator(IStringLocalizer<SharedResource> _localizer)
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
             .WithName("کد کپچا")
             .WithMessage(_localizer["EnterProperty"])
             .NotEmpty()
             .WithMessage(_localizer["CantEmpty"]);

        }
    }
}
