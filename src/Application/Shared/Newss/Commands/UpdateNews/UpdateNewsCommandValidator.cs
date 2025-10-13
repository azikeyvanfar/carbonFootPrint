using System.Linq;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Core;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Shared.Newss.Commands.UpdateNews
{
    public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
    {
        private readonly IFileExtensions _fileExtensions;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UpdateNewsCommandValidator(IFileExtensions fileExtensions, IStringLocalizer<SharedResource> localizer)
        {

            _fileExtensions = fileExtensions;
            _localizer = localizer;
            RuleFor(c => c.Photos)
                .MustAsync(async (c, d, e) => await _fileExtensions.ValidTypeFileAsync(c.Photos, DocumentFolder.NewsPhotos))
                .WithMessage(_localizer["FileFormatIsNotValid"])
                .MustAsync(async (c, d, e) => await _fileExtensions.ValidMaxCountAsync(c.Photos?.ToList(), DocumentFolder.NewsPhotos))
                .WithMessage(_localizer["FileMaxCountIsNotValid"]);

            RuleFor(c => c.Attachments)
                .MustAsync(async (c, d, e) => await _fileExtensions.ValidTypeFileAsync(c.Attachments, DocumentFolder.NewsAttachments))
                .WithMessage(_localizer["FileFormatIsNotValid"])
                .MustAsync(async (c, d, e) => await _fileExtensions.ValidMaxCountAsync(c.Attachments?.ToList(), DocumentFolder.NewsAttachments))
                .WithMessage(_localizer["FileMaxCountIsNotValid"]);

            RuleFor(c => c.Title)
               .NotNull()
               .WithName("عنوان خبر")
               .WithMessage("لطفا {PropertyName} را وارد کنید")
               .NotEmpty()
               .WithMessage("لطفا {PropertyName} را وارد کنید");

        }

    }
}
