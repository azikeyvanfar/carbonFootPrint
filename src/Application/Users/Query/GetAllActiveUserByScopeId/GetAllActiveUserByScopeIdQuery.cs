using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Identity;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Users.Query.GetAllActiveUserByScopeId
{
    public class GetAllActiveUserByScopeIdQuery : SearchQueryRequest, IRequest<SearchQueryResponse<UserLovDto>>
    {
        [Required]
        public Guid ScopeId { get; set; }
        public bool? HasCurrentUser { get; set; } = true;
    }
    public class GetAllActiveUsersQueryHandler : IRequestHandler<GetAllActiveUserByScopeIdQuery, SearchQueryResponse<UserLovDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _repository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationDbContext _dbContext;

        public GetAllActiveUsersQueryHandler(
            IRepository<User> repository,
            IMapper mapper,
            IHttpContextAccessor accessor,
            IApplicationDbContext dbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _accessor = accessor;
            _dbContext = dbContext;
        }

        public async Task<SearchQueryResponse<UserLovDto>> Handle(GetAllActiveUserByScopeIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _accessor.HttpContext.GetUserId();
            IQueryable<User> queryEntity;
            if (request.HasCurrentUser == false || request.HasCurrentUser == null)
            {
                queryEntity = _repository.GetAllAsNoTracking().Where(x => x.Id != currentUserId);
            }
            else
            {
                queryEntity = _repository.GetAllAsNoTracking();
            }
            var query = queryEntity
                .Where(x => x.IsActive)
                .Where(x => x.Roles.Any(r => r.Role.RoleScopeId == request.ScopeId))
                .ProjectTo<UserLovDto>(_mapper.ConfigurationProvider);

            QueryablePaging<UserLovDto> qp = await query.GridifyQueryableAsync<UserLovDto>(request, null, cancellationToken);
            Paging<UserLovDto> result = new(qp.Count, qp.Query);
            return new SearchQueryResponse<UserLovDto>(request, result);
        }
    }
}
