using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Identity;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.UserRoles.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQuery : SearchQueryRequest, IRequest<SearchQueryResponse<UserWithRoleDto>>
    {
    }
    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, SearchQueryResponse<UserWithRoleDto>>
    {
        private readonly IRepository<User> _repositoryUser;
        public GetAllUserRolesQueryHandler(IRepository<User> repositoryUser)
        {
            _repositoryUser = repositoryUser;
        }
        public async Task<SearchQueryResponse<UserWithRoleDto>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            var lst = _repositoryUser.GetAllAsNoTracking()
                .Select(u => new UserWithRoleDto
                {
                    UserId = u.Id,
                    IsActive = u.IsActive,
                    PersonnelCode = u.PersonnelCode,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    // DesPost = u.UserDetails.DesPost,
                    Roles = u.Roles.Where(_ => _.UserId == u.Id)
                                   .Select(role => new RoleUserDto
                                   {
                                       RoleName = role.Role.Name,
                                       PersonnelCode = role.User.PersonnelCode,
                                       UserId = role.User.Id,
                                       RoleType = role.Role.RoleType,
                                       RoleTypeId = ((int)role.Role.RoleType).ToString(),
                                       RoleId = role.RoleId
                                   })
                }).OrderBy(_ => _.UserId);


            QueryablePaging<UserWithRoleDto> qp1 = await lst.AsQueryable().GridifyQueryableAsync<UserWithRoleDto>(request, null, cancellationToken);
            Paging<UserWithRoleDto> result = new(qp1.Count, qp1.Query);

            return new SearchQueryResponse<UserWithRoleDto>(request, result);
        }
    }
}