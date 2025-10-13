using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.ApplicationSettingPage.Commands.CreateApplicationSettingsClient
{
    public class CreateApplicationSettingsClientCommandValidator : AbstractValidator<CreateApplicationSettingsClientCommand>
    {
        public CreateApplicationSettingsClientCommandValidator(
            IStringLocalizer<SharedResource> _localizer)
        {
            RuleFor(c => c.RateLimitMaxRequests)
                .NotEmpty()
                .WithName(_localizer["RateLimitMaxRequests"])
                .WithMessage(_localizer["ValidationNotEmpty"])
                .Must((x, y) => y >= 10 && y <= 1_000_000)
                .WithMessage(_localizer["RateLimitRangeError"])
                ;

            RuleFor(c => c.RateLimitTimeWindow)
                .NotEmpty()
                .WithName(_localizer["RateLimitTimeWindow"])
                .WithMessage(_localizer["ValidationNotEmpty"])
                .Must((x, y) => y >= 1 && y <= 1000)
                .WithMessage(_localizer["RateLimitRangeError"])
                ;

            RuleForEach(c => c.RateLimitCustomRules)
                .Must((x, y) =>
                   (
                        !string.IsNullOrWhiteSpace(y.Name)
                    //&& y.RateLimitTimeWindow.Value >= 10 && y.RateLimitTimeWindow.Value <= 1_000_000      has Error
                    //&& y.RateLimitTimeWindow.Value >= 1 && y.RateLimitTimeWindow.Value <= 1000            has Error
                    )
                )
                .WithName(_localizer["RateLimitCustomRules"])
                .WithMessage(_localizer["RateLimitCustomRulesValidationError"])
                ;

        }
    }

}
