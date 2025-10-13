using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Documents.Queries.GetAllDocuments
{
    public class GetAllDocumentsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<DocumentDto>>
    {

    }

    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, SearchQueryResponse<DocumentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Document> _repository;

        public GetAllDocumentsQueryHandler(IMapper mapper, IRepository<Document> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<SearchQueryResponse<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAll()
            .Include(x => x.Owner)
            .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDateTime"));

            QueryablePaging<Document> qp1 = await query.GridifyQueryableAsync(request, null, cancellationToken);
            var qp = _mapper.Map<List<DocumentDto>>(qp1.Query);
            Paging<DocumentDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<DocumentDto>(request, result);

        }
    }
}