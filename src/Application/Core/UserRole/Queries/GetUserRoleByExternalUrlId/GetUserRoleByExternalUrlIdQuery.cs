using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.UserRole.Queries.GetUserRoleByExternalUrlId
{
    public class GetUserRoleByUserIdQuery : SearchQueryRequest, IRequest<SearchQueryResponse<UserRoleDto>>
    {
        public long UserId { get; set; }
    }
    public class GetUserRoleByIdQueryHandler : IRequestHandler<GetUserRoleByUserIdQuery, SearchQueryResponse<UserRoleDto>>
    {
        private readonly IRepository<Domain.Entities.Identity.UserRole> _repository;
        public GetUserRoleByIdQueryHandler(IRepository<Domain.Entities.Identity.UserRole> repository)
        {
            _repository = repository;
        }
        public async Task<SearchQueryResponse<UserRoleDto>> Handle(GetUserRoleByUserIdQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                .Include(c => c.User)
                .Include(c => c.Role)
                .Select(c => new UserRoleDto()
                {
                    UserId = c.UserId,
                    PersonnelCode = c.User.PersonnelCode,
                    UserRoleType = c.Role.RoleType,
                    UserRoleTypeId = ((int)c.Role.RoleType).ToString(),
                    RoleId = c.RoleId,
                    RoleTitle = c.Role.Name
                })
                .Where(c => c.UserId == request.UserId);

            QueryablePaging<UserRoleDto> qp1 = await query.GridifyQueryableAsync(request, null, cancellationToken);

            Paging<UserRoleDto> result = new(qp1.Count, qp1.Query);
            return new SearchQueryResponse<UserRoleDto>(request, result);
        }
    }
}
