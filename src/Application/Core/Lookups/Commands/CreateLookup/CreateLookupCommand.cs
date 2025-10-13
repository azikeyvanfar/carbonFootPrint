using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.Create
{
    /// <summary>
    /// افزودن لیست جدید
    /// </summary>
    public class CreateLookupCommand : AddLookupDto, IRequest<LookupDto>
    {
    }

    public class CreateLookupCommandCommandHandler : IRequestHandler<CreateLookupCommand, LookupDto>
    {
        private readonly ILookupRepository _services;

        public CreateLookupCommandCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<LookupDto> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
        {
            var list = await _services.CreateList(request);
            return list;
        }
    }
}
