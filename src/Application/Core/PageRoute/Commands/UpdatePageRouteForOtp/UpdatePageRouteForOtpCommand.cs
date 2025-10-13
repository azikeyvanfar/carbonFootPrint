using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.PageRoute.Commands.UpdatePageRouteForOtp
{
    public class UpdatePageRouteForOtpCommand : IRequest<bool>
    {
        public Guid PageRouteId { get; set; }
        public bool IsOtp { get; set; }
    }

    public class UpdatePageRouteForOtpCommandHandler : IRequestHandler<UpdatePageRouteForOtpCommand, bool>
    {
        private readonly IRepository<Domain.Entities.Core.PageRoute> _repository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UpdatePageRouteForOtpCommandHandler(IRepository<Domain.Entities.Core.PageRoute> pageRouteRep, IStringLocalizer<SharedResource> localizer)
        {
            _repository = pageRouteRep;
            _localizer = localizer;
        }

        public Task<bool> Handle(UpdatePageRouteForOtpCommand request, CancellationToken cancellationToken)
        {
            var pageRoute = _repository.GetAllAsNoTracking()
                .Include(x => x.PageRouteClaims).ThenInclude(x => x.GeneralClaim)
                .FirstOrDefault(x => x.IsActive && x.Id == request.PageRouteId);

            var hasClaimWithIsSuiteCall = pageRoute.PageRouteClaims
                .Any(x => IsSuiteUrlClass.sensitiveUrls2.Values.Any(l => l.ClaimValue.ToLowerInvariant() == x.GeneralClaim.ClaimValue.ToLowerInvariant()));

            if (hasClaimWithIsSuiteCall && request.IsOtp == true)
            {
                throw new CustomException(_localizer["PageCallsIsSuteError"]);
            }

            bool flag = false;
            var entity = _repository.GetById(request.PageRouteId);
            if (entity is null)
                throw new NullReferenceException();

            entity.IsOtp = request.IsOtp;
            flag = _repository.UpdateEntity(entity);
            return Task.FromResult(flag);
        }

    }
}