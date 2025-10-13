using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Queries.GetItemById
{
    /// <summary>
    /// جست و جو  براساس شناسه
    /// </summary>
    public class GetLookupItemByIdQuery : IRequest<LookupItemDto>
    {
        public Guid Id { get; set; }
    }

    public class GetItemByIdQueryHandler : IRequestHandler<GetLookupItemByIdQuery, LookupItemDto>
    {

        private readonly ILookupRepository _services;

        public GetItemByIdQueryHandler(ILookupRepository services)
        {
            _services = services;
        }
        public async Task<LookupItemDto> Handle(GetLookupItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _services.GetItemById(request.Id);
            return item;
        }
    }
}
