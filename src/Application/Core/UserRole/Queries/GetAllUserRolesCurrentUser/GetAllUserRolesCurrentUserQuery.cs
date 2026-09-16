using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Enums.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace ContractorBackend.Application.Core.UserRole.Queries.GetAllUserRolesCurrentUser
{
    public class GetAllUserRolesCurrentUserQuery : SearchQueryRequest, IRequest<SearchQueryResponse<UserWithRoleDto>>
    {
        public RoleType RoleType { get; set; }
    }
    public class GetAllUserRolesCurrentUserQueryHandler : IRequestHandler<GetAllUserRolesCurrentUserQuery, SearchQueryResponse<UserWithRoleDto>>
    {
        private readonly IRepository<Domain.Entities.Identity.UserRole> _repository;
        private readonly IApplicationUserManager _userManager;
        private readonly RoleManager<Domain.Entities.Identity.Role> _roleManager;

        public GetAllUserRolesCurrentUserQueryHandler(IRepository<Domain.Entities.Identity.UserRole> repository, IApplicationUserManager userManager, RoleManager<Domain.Entities.Identity.Role> roleManager)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<SearchQueryResponse<UserWithRoleDto>> Handle(GetAllUserRolesCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetCurrentUserAsync();
            var userRoles = await _userManager.GetRolesAsync(user, request.RoleType);
            var roleNames = userRoles != null && userRoles.Any() ?
                _roleManager.Roles
                    .Where(x => userRoles.Any(l => l == x.Id.ToString()) && x.RoleType == request.RoleType)
                    .Select(x => x.NormalizedName)
                    .ToList()
                : null;

            var query = _repository.GetAllAsNoTracking()
                 .Include(c => c.User)
                 .Include(c => c.Role)
                 .Where(c => c.User.Id == user.Id);


            var lst = query
                .GroupBy(_ => _.UserId)
                .Select(UserRole => new UserWithRoleDto
                {
                    UserId = UserRole.Key,
                    PersonnelCode = UserRole.FirstOrDefault(_ => _.UserId == UserRole.Key).User.PersonnelCode,
                    FirstName = UserRole.FirstOrDefault(_ => _.UserId == UserRole.Key).User.FirstName,
                    LastName = UserRole.FirstOrDefault(_ => _.UserId == UserRole.Key).User.LastName,
                    //DesPost = UserRole.FirstOrDefault(_ => _.UserId == UserRole.Key).User.UserDetails.DesPost,
                    Roles = query
                        .Where(_ => _.UserId == UserRole.Key)
                        .Select(role => new RoleUserDto
                        {
                            RoleName = role.Role.Name,
                            PersonnelCode = role.User.PersonnelCode,
                            UserId = role.User.Id,
                            RoleType = role.Role.RoleType,
                            RoleTypeId = ((int)role.Role.RoleType).ToString(),
                            RoleId = role.RoleId
                        })
                });

            var list = lst;//.BuildMock();

            QueryablePaging<UserWithRoleDto> qp1 = await list.AsQueryable().GridifyQueryableAsync(request, null, cancellationToken);
            Paging<UserWithRoleDto> result = new(qp1.Count, qp1.Query);

            return new SearchQueryResponse<UserWithRoleDto>(request, result);
        }
    }
}