using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.PageRoutes.Commands.DeletePageRoute
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
        private readonly IRepository<PageRoute> _pgRepository;
        public DeletePageRouteCommandHandler(IRepository<PageRoute> pgRouteRepository)
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
    }
}
