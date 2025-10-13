using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Documents.Commands.UpdateDocument
{
    public class UpdateDocumentCommand : IRequest
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid Id { get; set; }
        public Guid? RootId { get; set; }
        public string AliasName { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public string Alt { get; set; }


        // DO NOT REMOVE THIS COMMENT:NG02
    }

    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Document> _repository;

        public UpdateDocumentCommandHandler(IMapper mapper, IRepository<Document> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<Unit> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);

            if (entity is null)
                throw new CustomException("فایل یا پوشه مورد نظر موجود نمی باشد");

            _mapper.Map(request, entity);

            _repository.Update(entity);

            return Task.FromResult(Unit.Value);
        }
    }
}