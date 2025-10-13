using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using MediatR;

namespace ContractorBackend.Application.Core.UserRole.Commands.DeleteByUserId
{

    public class DeleteByUserIdCommand : IRequest
    {
        public long UserId { get; set; }
        public DeleteByUserIdCommand(long Id)
        {
            UserId = Id;

        }
    }
    public class DeleteByUserIdCommandHandler : IRequestHandler<DeleteByUserIdCommand>
    {
        private IRepository<Domain.Entities.Identity.UserRole> _repository;
        public DeleteByUserIdCommandHandler(IRepository<Domain.Entities.Identity.UserRole> repository)
        {
            _repository = repository;

        }
        public Task<Unit> Handle(DeleteByUserIdCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetAllAsNoTracking().Where(c => c.UserId == request.UserId);

            if (entity is null)
                throw new NullReferenceException();

            _repository.DeleteRange(entity);
            return Task.FromResult(Unit.Value);
        }
    }
}
