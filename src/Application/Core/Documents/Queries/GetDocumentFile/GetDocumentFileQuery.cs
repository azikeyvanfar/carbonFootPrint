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

namespace ContractorBackend.Application.Core.Documents.Queries.GetDocumentFile
{
    public class GetDocumentFileQuery : IRequest<FileBase64Dto>
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; }
        public GetDocumentFileQuery(Guid id, bool isPrivate)
        {
            Id = id;
            IsPrivate = isPrivate;
        }

    }

    public class GetDocumentFileQueryHandler : IRequestHandler<GetDocumentFileQuery, FileBase64Dto>
    {
        private readonly IRepository<Document> _repository;
        private readonly IConfiguration _configuration;

        public GetDocumentFileQueryHandler(IRepository<Document> repository, IConfiguration config)
        {
            _repository = repository;
            _configuration = config;
        }

        public async Task<FileBase64Dto> Handle(GetDocumentFileQuery request, CancellationToken cancellationToken)
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

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    var base64Str = Convert.ToBase64String(fileBytes);

                    file.Close();
                    file.Dispose();
                    return new FileBase64Dto
                    {
                        Id = request.Id,
                        Content = base64Str,
                        ContentType = mimeType,
                        //FileName = file.Name
                        FileName = entity.RealName
                    };
                }
            }
            else
            {
                throw new CustomException("خطا در انجام عملیات");
            }

        }
    }
}
