using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Identity;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Account.Query.GetAllList
{
    public class GetAllUserQuery : SearchQueryRequest, IRequest<SearchQueryResponse<UserLovDto>>
    {
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, SearchQueryResponse<UserLovDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetAllUserQueryHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<SearchQueryResponse<UserLovDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users
                .Where(_ => _.IsActive)
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<UserLovDto>(_mapper.ConfigurationProvider)
                ;

            QueryablePaging<UserLovDto> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<UserLovDto> result = new(qp.Count, qp.Query);
            return new SearchQueryResponse<UserLovDto>(request, result);
        }
    }

}
