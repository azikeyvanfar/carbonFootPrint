using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Commands.Update
{
    /// <summary>
    /// بروز رسانی اطلاعات لیست
    /// </summary>
    public class UpdateLookupCommand : UpdateLookupDto, IRequest<LookupDto>
    {
    }

    public class UpdateLookupCommandHandler : IRequestHandler<UpdateLookupCommand, LookupDto>
    {
        private readonly ILookupRepository _services;

        public UpdateLookupCommandHandler(ILookupRepository services)
        {
            _services = services;
        }

        public async Task<LookupDto> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
        {
            var list = await _services.UpdateList(request);
            return list;
        }
    }
}
