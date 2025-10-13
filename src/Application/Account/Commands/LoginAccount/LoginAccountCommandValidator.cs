using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Account.Commands.LoginAccount
{
    public class LoginAccountCommandValidator : AbstractValidator<LoginAccountCommand>
    {
        public LoginAccountCommandValidator(IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithName("کد پرسنلی سا شماره ملی")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

            RuleFor(x => x.Password)
                .NotNull()
                .WithName("کلمه عبور")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);
        }
    }
}
