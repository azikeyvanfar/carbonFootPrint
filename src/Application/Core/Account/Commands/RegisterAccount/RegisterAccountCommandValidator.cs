using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.RegisterAccount
{
    public class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
    {
        private readonly IAccountService _accountService;
        public RegisterAccountCommandValidator(IAccountService accountService, IStringLocalizer<SharedResource> _localizer)
        {
            _accountService = accountService;


            When(x => x.EmployeeId != null, () =>
            {
                RuleFor(x => x.PhoneNumber)
                .NotNull()
                .WithName("شماره موبایل")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"])
                .MaximumLength(10)
                .WithMessage("{PropertyName} را به صورت کامل وارد نمایید")
                .Must((command, s) => _accountService.IsValidPhoneNumber(command.PhoneNumber))
                .WithMessage("{PropertyName} وارد شده نامعتبر است")
                .Must((command, s) => _accountService.ExistsEmployeePhoneNumber(command.EmployeeId.Value) == false ? !string.IsNullOrWhiteSpace(command.PhoneNumber) : true)
                .WithMessage("{PropertyName} الزامی است");

            });

            When(x => x.EmployeeId == null, () =>
            {
                RuleFor(x => x.FirstName)
                .NotNull()
                .WithName("نام")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

                RuleFor(x => x.LastName)
                .NotNull()
                .WithName("نام خانوادگی")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

                RuleFor(x => x.PhoneNumber)
                .NotNull()
                .WithName("شماره موبایل")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"])
                .MaximumLength(10)
                .WithMessage("{PropertyName} را به صورت کامل وارد نمایید")
                .Must((command, s) => _accountService.IsValidPhoneNumber(command.PhoneNumber))
                .WithMessage("{PropertyName} وارد شده نامعتبر است")
                ;

                RuleFor(x => x.NationalCode)
                .NotNull()
                .WithName("کد ملی")
                .WithMessage(_localizer["EnterProperty"])
                .NotEmpty()
                .WithMessage(_localizer["CantEmpty"]);

            });

            RuleFor(x => x.Password)
            .NotNull()
            .WithName("رمز عبور")
            .WithMessage(_localizer["EnterProperty"])
            .NotEmpty()
            .WithMessage(_localizer["CantEmpty"]);



        }
    }
}
