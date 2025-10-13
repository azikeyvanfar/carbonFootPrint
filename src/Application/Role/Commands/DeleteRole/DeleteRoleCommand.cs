using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using MediatR;

namespace ContractorBackend.Application.Role.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteRoleCommand(long id)
        {
            Id = id;
        }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand>
    {
        private readonly IApplicationRoleManager _roleManager;
        public DeleteRoleCommandHandler(IApplicationRoleManager roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleEntity = await _roleManager.FindRoleIncludeRoleClaimsAsync(request.Id);
            if (roleEntity is null || roleEntity.IsAdministrator)
            {
                throw new NullReferenceException();
            }
            await _roleManager.DeleteAsync(roleEntity);
            return await Task.FromResult(Unit.Value);
        }
    }
}
