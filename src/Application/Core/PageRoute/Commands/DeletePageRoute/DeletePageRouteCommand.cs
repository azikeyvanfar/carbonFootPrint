using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using MediatR;

namespace ContractorBackend.Application.Core.PageRoute.Commands.DeletePageRoute
{
    public class DeletePageRouteCommand : IRequest
    {
        public Guid PageRouteId { get; set; }
        public DeletePageRouteCommand(Guid id)
        {
            PageRouteId = id;
        }
    }

    public class DeletePageRouteCommandHandler : IRequestHandler<DeletePageRouteCommand>
    {
        private readonly IRepository<Domain.Entities.Core.PageRoute> _pgRepository;
        public DeletePageRouteCommandHandler(IRepository<Domain.Entities.Core.PageRoute> pgRouteRepository)
        {
            _pgRepository = pgRouteRepository;
        }

        public Task<Unit> Handle(DeletePageRouteCommand request, CancellationToken cancellationToken)
        {
            var entity = _pgRepository.GetById(request.PageRouteId);
            if (entity is null)
                throw new NullReferenceException();

            _pgRepository.Delete(entity);
            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<DeletePageRouteCommand>.Handle(DeletePageRouteCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
