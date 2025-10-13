using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Hosting;
namespace ContractorBackend.Application.Core.Documents.Queries.GetDocumentThumbnail
{

    public class GetDocumentThumbnailQuery : IRequest<FileDto>
    {
        public Guid Id { get; set; }
        public GetDocumentThumbnailQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetDocumentThumbnailQueryHandler : IRequestHandler<GetDocumentThumbnailQuery, FileDto>
    {
        private readonly IRepository<Document> _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GetDocumentThumbnailQueryHandler(IWebHostEnvironment webHostEnvironment, IRepository<Document> repository)
        {
            _webHostEnvironment = webHostEnvironment;
            _repository = repository;
        }

        public async Task<FileDto> Handle(GetDocumentThumbnailQuery request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);

            if (entity == null)
                throw new NullReferenceException();

            if (!string.IsNullOrEmpty(entity.GeneratedName))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Document\\Thumbnail", "tmb_" + entity.GeneratedName);
                var file = File.OpenRead(path);

                var mimeType = MimeKit.MimeTypes.GetMimeType(file.Name);
                if (FileExtensions.IsImage(mimeType))
                {
                    return new FileDto
                    {
                        ContentType = mimeType,
                        FileContents = file,
                        FileDownloadName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name)
                    };
                }
                else { throw new CustomException("نوع فایل تصویر نمی باشد"); }
            }
            else
                throw new NullReferenceException();
        }
    }
}
