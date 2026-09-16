using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.PageRouteClaims.Commands.DeletePageRouteClaim
{
    public class DeletePageRouteClaimCommand : IRequest
    {
        public Guid PageRouteId { get; set; }
        public DeletePageRouteClaimCommand(Guid Id)
        {
            PageRouteId = Id;

        }
    }
    public class DeleteClaimMenuItemCommandHandler : IRequestHandler<DeletePageRouteClaimCommand>
    {
        private IRepository<PageRouteClaim> _repository;
        public DeleteClaimMenuItemCommandHandler(IRepository<PageRouteClaim> repository)
        {
            _repository = repository;

        }
        public Task<Unit> Handle(DeletePageRouteClaimCommand request, CancellationToken cancellationToken)
        {
            var entities = _repository.GetAllAsNoTracking().Where(_ => _.PageRouteId == request.PageRouteId);

            if (entities.Count() == 0)
                throw new NullReferenceException();

            _repository.DeleteRange(entities);
            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<DeletePageRouteClaimCommand>.Handle(DeletePageRouteClaimCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
