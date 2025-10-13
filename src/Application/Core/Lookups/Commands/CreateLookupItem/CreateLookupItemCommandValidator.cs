using System.Linq;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Enums.Core;
using FluentValidation;

namespace ContractorBackend.Application.Core.Lookups.Commands.CreateItem
{
    public class CreateLookupItemCommandValidator : AbstractValidator<CreateLookupItemCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateLookupItemCommandValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            RuleFor(x => x).Must(CheckInputData).WithMessage("داده های ورودی نامعتبر است");
        }

        public bool CheckInputData(CreateLookupItemCommand input)
        {
            var result = true;
            var checkExistCode = _dbContext.Lookups.Any(x => x.EnName.ToLower() == input.Code.ToLower() && x.Type == LookupType.Lookup);
            if (!checkExistCode) result = false;
            if (input.ParentId.HasValue)
            {
                var checkExistParent = _dbContext.Lookups.Any(x => x.Id == input.ParentId && x.Type == LookupType.Lookup);
                if (!checkExistCode) result = false;
            }
            if (input.CategoryId.HasValue)
            {
                var checkExistParent = _dbContext.Lookups.Any(x => x.Id == input.CategoryId && x.Type == LookupType.Lookup);
                if (!checkExistCode) result = false;
            }
            return result;
        }
    }
}
