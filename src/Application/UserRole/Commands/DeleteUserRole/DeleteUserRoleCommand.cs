using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;

namespace ContractorBackend.Application.UserRoles.Commands.DeleteUserRole
{
    public class DeleteUserRoleCommand : IRequest
    {
        public long RoleId { get; set; }
        public long UserId { get; set; }
        public DeleteUserRoleCommand(long RId, long UId)
        {
            RoleId = RId;
            UserId = UId;
        }
    }
    public class DeleteClaimMenuItemCommandHandler : IRequestHandler<DeleteUserRoleCommand>
    {
        private IRepository<UserRole> _repository;
        public DeleteClaimMenuItemCommandHandler(IRepository<UserRole> repository)
        {
            _repository = repository;
        }
        public Task<Unit> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
        {
            var entities = _repository.GetAllAsNoTracking().Where(_ => _.UserId == request.UserId && _.RoleId == request.RoleId);

            if (!entities.Any())
                throw new NullReferenceException();

            _repository.DeleteRange(entities);
            return Task.FromResult(Unit.Value);
        }
    }
}