using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.Delete
{
    /// <summary>
    /// حذف لیست
    /// </summary>
    public class DeleteLookupCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteLookupCommandHandler : IRequestHandler<DeleteLookupCommand, Unit>
    {
        private readonly ILookupRepository _services;

        public DeleteLookupCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<Unit> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
        {
            await _services.DeleteList(request.Id);
            return Unit.Value;
        }
    }
}
