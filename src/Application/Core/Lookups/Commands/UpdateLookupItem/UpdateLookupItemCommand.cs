using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.UpdateItem
{
    public class UpdateLookupItemCommand : UpdateLookupItemDto, IRequest<LookupItemDto>
    {
    }

    public class UpdateLookupItemCommandHandler : IRequestHandler<UpdateLookupItemCommand, LookupItemDto>
    {
        private readonly ILookupRepository _services;

        public UpdateLookupItemCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<LookupItemDto> Handle(UpdateLookupItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _services.UpdateItem(request);
            return item;
        }
    }
}
