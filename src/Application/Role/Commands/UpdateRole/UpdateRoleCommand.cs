using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using MediatR;

namespace ContractorBackend.Application.Role.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest
    {

        public long RoleId { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; } = true;

        public string Description { get; set; }

        public bool IsAdministrator { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
    {
        private readonly IApplicationRoleManager _roleManager;
        public UpdateRoleCommandHandler(IApplicationRoleManager roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<MediatR.Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleEntity = await _roleManager.FindRoleIncludeRoleClaimsAsync(request.RoleId);
            if (roleEntity is null)
            {
                throw new NullReferenceException();
            }
            roleEntity.Name = request.Title;
            roleEntity.IsActive = request.IsActive;
            roleEntity.Description = request.Description;
            roleEntity.IsAdministrator = request.IsAdministrator;
            await _roleManager.UpdateAsync(roleEntity);
            return await Task.FromResult(MediatR.Unit.Value);
        }
    }
}
