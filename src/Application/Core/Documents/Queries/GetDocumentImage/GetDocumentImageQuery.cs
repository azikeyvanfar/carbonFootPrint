using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Core.Documents.Queries.GetDocumentImage
{
    public class GetDocumentImageQuery : IRequest<FileDto>
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; }
        public GetDocumentImageQuery(Guid id, bool isPrivate)
        {
            Id = id;
            IsPrivate = isPrivate;
        }

    }

    public class GetDocumentImageQueryHandler : IRequestHandler<GetDocumentImageQuery, FileDto>
    {
        private readonly IRepository<Document> _repository;
        private readonly IConfiguration _configuration;

        public GetDocumentImageQueryHandler(IRepository<Document> repository, IConfiguration config)
        {
            _repository = repository;
            _configuration = config;
        }

        public async Task<FileDto> Handle(GetDocumentImageQuery request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);

            if (entity == null)
            {
                throw new CustomException("فایل مورد نظر پیدا نشد");

            }

            if (entity.IsPublic == request.IsPrivate)
            {
                throw new CustomException("دسترسی به این فایل ندارید");
            }
            Document rootFolder = null;
            if (entity.Type == DocumentType.File && entity.RootId is not null)
            {
                rootFolder = _repository.GetById(entity.RootId.Value);
            }



            if (entity.Type == DocumentType.File && !string.IsNullOrEmpty(entity.GeneratedName))
            {
                var path = Path.Combine(
                    _configuration["localStaticStoragePath"],
                    rootFolder.Name,
                    entity.GeneratedName).Replace("\\", "/");
                var file = File.OpenRead(path);

                var mimeType = MimeKit.MimeTypes.GetMimeType(file.Name);

                return new FileDto
                {
                    ContentType = mimeType,
                    FileContents = file,
                    FileDownloadName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name)
                };
            }
            else
                throw new NullReferenceException();
        }
    }
}
