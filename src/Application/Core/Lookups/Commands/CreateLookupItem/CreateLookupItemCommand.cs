using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.CreateItem
{
    /// <summary>
    /// افزودن ایتم به لیست های کشویی
    /// </summary>
    public class CreateLookupItemCommand : AddLookupItemDto, IRequest<LookupItemDto>
    {
    }

    public class CreateLookupItemCommandHandler : IRequestHandler<CreateLookupItemCommand, LookupItemDto>
    {
        private readonly ILookupRepository _services;

        public CreateLookupItemCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<LookupItemDto> Handle(CreateLookupItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _services.CreateItem(request);
            return item;
        }
    }
}
