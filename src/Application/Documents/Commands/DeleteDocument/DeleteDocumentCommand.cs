using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Documents.Commands.DeleteDocument
{
    public class DeleteDocumentCommand : IRequest
    {
        public Guid DocumentId { get; set; }
        public DeleteDocumentCommand(Guid Id)
        {
            DocumentId = Id;
        }
    }

    public class DeleteDocumentByIdCommandHandler : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IRepository<Document> _repository;
        public DeleteDocumentByIdCommandHandler(IRepository<Document> repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.DocumentId);

            if (entity is null)
                throw new NullReferenceException();

            _repository.Delete(entity);

            return Task.FromResult(Unit.Value);
        }
    }
}