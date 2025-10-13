using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.PageRouteClaims.Commands.DeleteClaimMenuItemSByMenuItemId
{

    public class DeleteByPageRouteIdCommand : IRequest
    {
        public Guid PageRouteId { get; set; }
        public DeleteByPageRouteIdCommand(Guid Id)
        {
            PageRouteId = Id;

        }
    }
    public class DeleteByPageRouteIdCommandHandler : IRequestHandler<DeleteByPageRouteIdCommand>
    {
        private IRepository<PageRouteClaim> _repository;
        public DeleteByPageRouteIdCommandHandler(IRepository<PageRouteClaim> repository)
        {
            _repository = repository;

        }
        public Task<Unit> Handle(DeleteByPageRouteIdCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetAllAsNoTracking().Where(c => c.PageRouteId == request.PageRouteId);

            if (entity is null)
                throw new NullReferenceException();

            _repository.DeleteRange(entity);
            return Task.FromResult(Unit.Value);
        }
    }
}
