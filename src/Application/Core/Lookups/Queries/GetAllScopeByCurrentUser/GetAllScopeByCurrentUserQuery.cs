using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Resources;
using Gridify;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Lookups.Queries.GetAllScopeByCurrentUser
{
    /// <summary>
    /// لیست آیتم ها
    /// </summary>
    public class GetAllScopeByCurrentUserQuery : SearchQueryRequest, IRequest<SearchQueryResponse<LookupItemDto>>
    {
    }

    public class GetAllScopeByCurrentUserQueryHandler : IRequestHandler<GetAllScopeByCurrentUserQuery, SearchQueryResponse<LookupItemDto>>
    {
        private readonly ILookupRepository _services;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUserService _userService;

        private const string code = Utilities.ScopeCode;

        public GetAllScopeByCurrentUserQueryHandler(ILookupRepository services, IApplicationRoleManager roleManager, IStringLocalizer<SharedResource> localizer, IUserService userService)
        {
            _services = services;
            _roleManager = roleManager;
            _localizer = localizer;
            _userService = userService;
        }

        public async Task<SearchQueryResponse<LookupItemDto>> Handle(GetAllScopeByCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var query = _userService.GetCurrentUserScopes();
            //QueryablePaging<LookupItemDto> qp = await query. GridifyQueryableAsync(request, null, cancellationToken);
            Paging<LookupItemDto> pq = new(query.Count(), query.ToList());
            return new SearchQueryResponse<LookupItemDto>(request, pq);
        }
    }
}
