using System;
using System.Linq;
using ContractorBackend.Application.Common.Interfaces;
using FluentValidation;

namespace ContractorBackend.Application.Shared.NewsCategories.Commands.CreateNewsCategory
{
    public class CreateNewsCategoryCommandValidator : AbstractValidator<CreateNewsCategoryCommand>
    {
        private readonly INewsCategoryService _service;
        public CreateNewsCategoryCommandValidator(INewsCategoryService service)
        {
            _service = service;

            RuleFor(c => c.Name)
              .NotNull()
              .WithName("نام گروه خبر")
              .WithMessage("لطفا {PropertyName} را وارد کنید")
              .NotEmpty()
              .WithMessage("لطفا {PropertyName} را وارد کنید")
               .MustAsync(async (command, s, cancellationToken) => !await _service.CheckExistNameAsync(null, command.Name, cancellationToken))
               .WithMessage("این {PropertyName} قبلا استفاده شده است");

            RuleFor(c => c.BusinessUnitIds)
             .Must((x, y) => x.BusinessUnitIds != null && y.Any(x => Guid.TryParse(x.ToString(), out Guid isGu)))
             .WithName("لیست واحد ها")
             .WithMessage("{PropertyName} معتبر نیست");
        }

    }
}
