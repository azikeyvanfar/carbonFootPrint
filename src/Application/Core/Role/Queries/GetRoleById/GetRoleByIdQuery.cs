using System;
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
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Role.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public long Id { get; set; }

        public GetRoleByIdQuery(long id)
        {
            Id = id;
        }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationDbContext _dbContext;
        private readonly DbSet<Domain.Entities.Identity.Role> _roles;
        private readonly IRepository<RolePageRouteAccess> _rolePageRouteAccessRepo;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(
            IApplicationRoleManager roleManager,
            IApplicationDbContext dbContext,
            IMapper mapper,
            IRepository<RolePageRouteAccess> rolePageRouteAccessRepo)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
            _roles = dbContext.Set<Domain.Entities.Identity.Role>();
            _mapper = mapper;
            _rolePageRouteAccessRepo = rolePageRouteAccessRepo;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _roles.AsNoTracking()
              .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
              .Where(x => x.RoleId == request.Id)
              .FirstOrDefaultAsync(cancellationToken)
              ;

            if (entity is null)
            {
                throw new NullReferenceException();
            }

            var roleEntity = await _roleManager.FindByIdAsync(entity.RoleId.ToString());
            var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
            entity.HasAccess = _rolePageRouteAccessRepo.GetAllAsNoTracking().Any(_ => _.RoleId == entity.RoleId);
            foreach (var claim in roleClaims)
            {
                entity.ActionList.Add(new SelectModel() { Text = claim.Value, Value = claim.Value });
            }

            return entity;
        }
    }
}
