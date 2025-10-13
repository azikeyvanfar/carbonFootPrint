using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.DeleteItem
{
    /// <summary>
    /// حذف آیتم لیست کشویی
    /// </summary>
    public class DeleteLookupItemCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteLookupItemCommandHandler : IRequestHandler<DeleteLookupItemCommand, Unit>
    {
        private readonly ILookupRepository _services;

        public DeleteLookupItemCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<Unit> Handle(DeleteLookupItemCommand request, CancellationToken cancellationToken)
        {
            await _services.DeleteItem(request.Id);
            return Unit.Value;
        }
    }
}
