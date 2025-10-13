using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.MenuItems.Commands.CreateMenuItem
{
    internal class CreateMenuItemCommadValidator : AbstractValidator<CreateMenuItemCommand>
    {
        public CreateMenuItemCommadValidator(IStringLocalizer<SharedResource> _localizer)
        {

            RuleFor(c => c.Title)
              .NotNull()
              .WithName("عنوان منو")
              .WithMessage(_localizer["EnterProperty"])
              .NotEmpty()
              .WithMessage(_localizer["EnterProperty"]);

            RuleFor(c => c.Name)
              .NotNull()
              .WithName("نام انگلیسی منو")
              .WithMessage(_localizer["EnterProperty"])
              .NotEmpty()
              .WithMessage(_localizer["EnterProperty"]);

        }

    }
}