using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator(IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(x => x.RefreshToken)
          .NotNull()
          .WithName("RefreshToken")
          .WithMessage(_localizer["EnterProperty"])
          .NotEmpty()
          .WithMessage(_localizer["CantEmpty"]);
        }
    }
}
