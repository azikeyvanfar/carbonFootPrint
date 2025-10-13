using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Role.Commands.CreateRole
{
    public class CreateRoleCommandValidation : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidation(IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(c => c.Title)
              .NotNull()
              .WithName("عنوان")
              .WithMessage(_localizer["EnterProperty"])
              .NotEmpty()
              .WithMessage(_localizer["EnterProperty"]);
        }
    }
}
