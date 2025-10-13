using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ContractorBackend.Application.Core.Role.Queries.GetAllRoles
{
    /// <summary>
    /// لیست صفحه بندی شده نقش ها به همراه صفحاتی که هر نقش دسترسی دارد 
    /// </summary>
    public class GetAllRolesQuery : SearchQueryRequest, IRequest<SearchQueryResponse<RoleDto>>
    {
        public string Title { get; set; }
    }

    public class GetAllRolesQueryHandler :
        IRequestHandler<GetAllRolesQuery, SearchQueryResponse<RoleDto>>
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IMapper _mapper;
        private readonly IRepository<RolePageRouteAccess> _rolePageRouteAccessRepo;
        private readonly DbSet<Domain.Entities.Identity.Role> _roles;

        public GetAllRolesQueryHandler(
            IMapper mapper,
            IApplicationRoleManager roleManager,
            IRepository<RolePageRouteAccess> rolePageRouteAccessRepo,
            IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _rolePageRouteAccessRepo = rolePageRouteAccessRepo;
            _roles = dbContext.Set<Domain.Entities.Identity.Role>();
        }
        public async Task<SearchQueryResponse<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var query = _roles.AsNoTracking()
                 .ProjectTo<RoleDto>(_mapper.ConfigurationProvider);

            QueryablePaging<RoleDto> qp = await query.GridifyQueryableAsync(request, null);
            Paging<RoleDto> result = new(qp.Count, qp.Query.ToList());


            // todo: : remove these lines if not used
            foreach (var role in result.Data)
            {
                var roleEntity = await _roleManager.FindByIdAsync(role.RoleId.ToString());
                var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                role.HasAccess = _rolePageRouteAccessRepo.GetAllAsNoTracking().Any(_ => _.RoleId == role.RoleId);
                foreach (var claim in roleClaims)
                {
                    role.ActionList.Add(new SelectModel() { Text = claim.Value, Value = claim.Value });
                }

            }
            return new SearchQueryResponse<RoleDto>(request, result);
        }
    }
}
