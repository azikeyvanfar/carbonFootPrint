using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.PageRoute.Commands.CreatePageRoute
{
    public class CreatePageRouteCommandValidator : AbstractValidator<CreatePageRouteCommand>
    {
        public CreatePageRouteCommandValidator(IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(x => x.Route)
           .NotNull()
           .WithName("مسیر")
           .WithMessage(_localizer["EnterProperty"])
           .NotEmpty()
           .WithMessage(_localizer["CantEmpty"]);

            RuleFor(x => x.RouteName)
           .NotNull()
           .WithName("نام مسیر")
           .WithMessage(_localizer["EnterProperty"])
           .NotEmpty()
           .WithMessage(_localizer["CantEmpty"]);
        }
    }
}
