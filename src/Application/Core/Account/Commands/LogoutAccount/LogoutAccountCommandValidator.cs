using FluentValidation;

namespace ContractorBackend.Application.Core.Account.Commands.LogoutAccount
{
    public class LogoutAccountCommandValidator : AbstractValidator<LogoutAccountCommand>
    {
        //public LogoutAccountCommandValidator(IStringLocalizer<SharedResource> _localizer)
        //{
        //    RuleFor(x => x.RefreshToken)
        //        .NotNull()
        //        .WithName("RefreshToken")
        //        .WithMessage(_localizer["EnterProperty"])
        //        .NotEmpty()
        //        .WithMessage(_localizer["CantEmpty"]);
        //}
    }
}
