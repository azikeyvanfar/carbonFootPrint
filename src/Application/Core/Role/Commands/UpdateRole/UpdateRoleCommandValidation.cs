using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Role.Commands.UpdateRole
{
    public class UpdateRoleCommandValidation : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidation(IStringLocalizer<SharedResource> _localizer)
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
