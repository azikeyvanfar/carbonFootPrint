using FluentValidation;

namespace ContractorBackend.Application.Core.PageRouteClaims.Commands.UpdatePageRouteClaim
{
    public class UpdatePageRouteClaimCommandValidator : AbstractValidator<UpdatePageRouteClaimCommand>
    {

        public UpdatePageRouteClaimCommandValidator()
        {

            RuleFor(c => c.ClaimId).NotNull().NotEmpty().WithMessage("مقدار CalimId نمی تواند خالی باشد");
            RuleFor(c => c.PageRouteId).NotNull().NotEmpty().WithMessage("مقدار PageRouteId نمی تواند خالی باشد");
            RuleFor(c => c.Id).NotEmpty().NotNull().WithMessage("مقدار Id نمی تواند خالی باشد");
        }
    }
}
