using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Lookups.Queries.GetListById
{
    /// <summary>
    /// جست وجو لیست بر اساس شناسه 
    /// </summary>
    public class GetLookupByIdQuery : IRequest<LookupDto>
    {
        public Guid Id { get; set; }
    }

    public class GetListByIdQueryHandler : IRequestHandler<GetLookupByIdQuery, LookupDto>
    {
        private readonly ILookupRepository _serivces;

        public GetListByIdQueryHandler(ILookupRepository serivces)
        {
            _serivces = serivces;
        }

        public async Task<LookupDto> Handle(GetLookupByIdQuery request, CancellationToken cancellationToken)
        {
            var list = await _serivces.GetListById(request.Id);
            return list;
        }
    }
}
