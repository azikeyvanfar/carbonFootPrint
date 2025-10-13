using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Role.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest
    {

        public string Title { get; set; }

        public bool IsActive { get; set; } = true;

        public RoleType RoleType { get; set; }
        /// <summary>
        /// حوزه سیستم
        /// </summary>
        public Guid RoleScopeId { get; set; }

        public string Description { get; set; }

        public bool IsAdministrator { get; set; }

    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand>
    {
        private readonly IApplicationRoleManager _roleManager;
        public CreateRoleCommandHandler(IApplicationRoleManager roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleEntity = new Domain.Entities.Identity.Role(request.Title, request.RoleType, request.RoleScopeId)
            {
                Description = request.Description,
                Name = request.Title,
                IsActive = request.IsActive,
                RoleType = request.RoleType,
                IsAdministrator = request.IsAdministrator,
            };

            await _roleManager.CreateAsync(roleEntity);
            //foreach (var path in request.Pathes)
            //{
            //    //var obj = new RoleClaim()
            //    //{
            //    //    ClaimType = ConstantPolicies.DynamicPermissionClaimType,
            //    //    ClaimValue = path
            //    //};
            //    await _roleManager.AddClaimAsync(roleEntity,
            //        new System.Security.Claims.Claim(ConstantPolicies.DynamicPermissionClaimType, path));
            //}
            return await Task.FromResult(Unit.Value);
        }
    }
}
