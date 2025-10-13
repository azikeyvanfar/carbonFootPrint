using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Documents.Queries.GetAllRootDocument
{
    public class GetAllRootDocumentQuery : IRequest<List<DocumentDto>>
    {
    }

    public class GetAllRootDocumentQueryHandler : IRequestHandler<GetAllRootDocumentQuery, List<DocumentDto>>
    {
        private readonly IRepository<Document> _repository;
        public GetAllRootDocumentQueryHandler(IRepository<Document> repository)
        {
            _repository = repository;
        }
        public async Task<List<DocumentDto>> Handle(GetAllRootDocumentQuery request, CancellationToken cancellationToken)
        {
            var lst = await _repository.GetAllAsNoTracking()
                 .Where(_ => _.ParentId == null && _.Type == DocumentType.Folder)
                 .Select(a => new DocumentDto()
                 {
                     Name = a.Name,
                     Id = a.Id
                 }).ToListAsync(cancellationToken: cancellationToken);
            return lst;
        }
    }
}
