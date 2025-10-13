using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Documents.Queries.GetDocumentById
{
    public class GetDocumentByIdQuery : IRequest<DocumentDto>
    {
        public Guid Id { get; set; }
        public GetDocumentByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Document> _repository;

        public GetDocumentByIdQueryHandler(IMapper mapper, IRepository<Document> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAll()
                      .Include(x => x.Owner)
                      .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NullReferenceException();

            return _mapper.Map<DocumentDto>(entity);

        }
    }
}
