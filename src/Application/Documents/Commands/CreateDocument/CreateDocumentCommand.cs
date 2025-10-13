using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Documents.Commands.CreateDocument
{
    public class CreateDocumentCommand : IRequest
    {
        // DO NOT REMOVE THIS COMMENT:NG01

        public Guid ParentId { get; set; }

        public Guid? RootId { get; set; }
        public string? AliasName { get; set; }
        public String Name { get; set; }
        public DocumentType Type { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
        public string? Alt { get; set; }

        // DO NOT REMOVE THIS COMMENT:NG02
    }

    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Document> _repository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomUploadFileService _customUploadFileService;

        public CreateDocumentCommandHandler(IMapper mapper,
            ICustomUploadFileService customUploadFileService,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Document> repository,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _customUploadFileService = customUploadFileService;
            _configuration = configuration;
        }

        public async Task<MediatR.Unit> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Document>(request);

            entity.OwnerId = _httpContextAccessor.HttpContext.GetUserId();
            entity.ParentId = entity.ParentId != Guid.Empty ? entity.ParentId : null;
            if (request.File != null && request.Type == DocumentType.File)
            {
                if (FileExtensions.IsSaveFile(request.File))
                {
                    if (entity.RootId is not null)
                    {
                        var rootEntity = _repository.GetById(entity.RootId.Value);
                        //var saveFileResult = await _customUploadFileService.UploadFileAsync(request.File, "Document");
                        var saveFileResult = await _customUploadFileService.UploadFileInCustomStorageAsync(request.File, _configuration["localStaticStoragePath"], rootEntity.Name);
                        if (saveFileResult.IsSaved)
                        {
                            if (FileExtensions.IsImage(request.File))
                            {
                                //var saveThumbnail = _customUploadFileService.UploadThumbnailAsync(request.File, 
                                //    "Document\\Thumbnail", "tmb_" + saveFileResult.FileName);
                                var saveThumbnail = _customUploadFileService.UploadThumbnailInCustomStorageAsync(request.File, _configuration["localStaticStoragePath"],
                                    $"{rootEntity.Name}\\Thumbnail", "tmb_" + saveFileResult.FileName);
                            }
                            entity.RealName = request.File.FileName;
                            entity.GeneratedName = saveFileResult.FileName;
                            entity.Extension = Path.GetExtension(saveFileResult.FileName);
                            entity.MimeType = FileExtensions.GetMimeType(saveFileResult.FileName);

                            _repository.Insert(entity);
                            return MediatR.Unit.Value;
                        }
                        else
                            throw new CustomException("لطفا ادرس روت را مشخص کنید.");

                    }
                }
                else
                {
                    throw new CustomException("نوع فایل صحیح نمی باشد");
                }
            }
            else if (request.File == null && request.Type == DocumentType.Folder)
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    throw new CustomException("نام را وارد کنید");
                }
                else
                if (string.IsNullOrWhiteSpace(entity.AliasName))
                {
                    throw new CustomException("نام مستعار را وارد کنید");
                }
                else
                {

                    _repository.Insert(entity);
                    return MediatR.Unit.Value;
                }
            }
            else { throw new CustomException("درخواست شما برای ثبت فایل یا پوشه صحیح نمی باشد"); }

            return MediatR.Unit.Value;
        }


    }
}