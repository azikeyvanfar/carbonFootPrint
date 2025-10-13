using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Resources;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Serilog;

namespace ContractorBackend.Infrastructure.Services
{
    /// <summary>
    /// سرویس ذخیره فایل در درایو و ذخیره داکیومنت مربوط به ان در دیتابیس
    /// </summary>
    public class DocumentService : IDocumentService
    {
        //public Task<bool> CheckExistNameAsync(Guid? id, string name, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CreateDocument(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveFolder(string folderName)
        //{
        //    throw new NotImplementedException();
        //}

        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IRepository<Document> _docRepository;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IFileExtensions _fileExtensions;
        private readonly IWebHostEnvironment _env;

        public DocumentService(
            IMapper mapper,
            IHttpContextAccessor accessor,
            ICustomUploadFileService customUploadFileService,
            IApplicationDbContext context,
            IRepository<Document> docRepository,
            IConfiguration config,
            IStringLocalizer<SharedResource> localizer,
            IFileExtensions fileExtensions
,
            IWebHostEnvironment env)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
            _customUploadFileService = customUploadFileService ?? throw new ArgumentNullException(nameof(customUploadFileService));
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
            _docRepository = docRepository ?? throw new ArgumentNullException(nameof(docRepository));
            _localizer = localizer;
            _fileExtensions = fileExtensions;
            _env = env;
        }

        /// <summary>
        /// ذخیره در دیتابیس لیست فایل و بازگرداندن لیست دیتابیسی  
        /// </summary>
        /// <param name="docs">لیست فایل هایی که کلاینت برای ذخیره فرستاده</param>
        /// <param name="isPublicFiles">file is Public = true OR private = false</param>
        /// <param name="folderToSave">محل ذخیره</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<List<Document>> InsertDocuments(List<IFormFile> docs, bool isPublicFiles, DocumentFolder folderToSaveEnum, CancellationToken cancellationToken)
        {
            var folderToSave = folderToSaveEnum.ToString();
            List<Guid> insertedDocumentIds = new List<Guid>();
            List<Document> savedDocuments = new List<Document>();

            if (docs is not null)
            {
                foreach (var doc in docs)
                {
                    if (!await _fileExtensions.ValidTypeFileAsync(doc, folderToSaveEnum))
                    {
                        throw new CustomException("فرمت فایل غیرمجاز است.");
                    }

                    try
                    {
                        var rootFolder = _dbContext.Documents.FirstOrDefault(_ => _.Name == folderToSave);
                        if (rootFolder == null)
                        {
                            _dbContext.Documents.Add(new Document
                            {
                                AliasName = folderToSave,
                                Name = folderToSave,
                                IsActive = true,
                                Type = DocumentType.Folder,
                                IsPublic = false,
                                OwnerId = _accessor.HttpContext.GetUserId()
                            });

                            _dbContext.SaveChanges();
                            rootFolder = _dbContext.Documents.FirstOrDefault(_ => _.Name == folderToSave);
                        }

                        var item = new AttachmentDto() { File = doc };
                        if (rootFolder is not null)
                        {
                            item.RootId = rootFolder.Id;
                            var entity = _mapper.Map<Document>(item);
                            entity.OwnerId = _accessor.HttpContext.GetUserId();
                            entity.ParentId = entity.ParentId != Guid.Empty ? entity.ParentId : null;
                            entity.Type = DocumentType.File;
                            entity.IsPublic = isPublicFiles;
                            (bool IsSaved, string FilePath, string FileName) = await _customUploadFileService.UploadFileInCustomStorageAsync(item.File,
                              _configuration["localStaticStoragePath"], folderToSave);

                            if (IsSaved)
                            {
                                if (FileExtensions.IsImage(item.File))
                                {
                                    var saveThumbnail = _customUploadFileService
                                        .UploadThumbnailInCustomStorageAsync(item.File, _configuration["localStaticStoragePath"],
                                        folderToSave + "\\Thumbnail", "tmb_" + FileName);
                                }

                                entity.RealName = item.File.FileName;
                                entity.GeneratedName = FileName;
                                entity.Extension = Path.GetExtension(FileName);
                                entity.MimeType = FileExtensions.GetMimeType(FileName);

                                var resInsert = _docRepository.InsertEntity(entity);
                                if (resInsert)
                                {
                                    insertedDocumentIds.Add(entity.Id);
                                    savedDocuments.Add(entity);
                                }
                                else
                                {
                                    throw new Exception("فایل ذخیره نشد.");
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal($"exception: {ex.ToString()}");
                        throw;
                    }

                    var savedDocumentIds = string.Join(",", insertedDocumentIds);
                }

            }

            return savedDocuments;

        }

        /// <summary>
        /// ذخیره در دیتابیس فایل و بازگرداندن آن
        /// </summary>
        /// <param name="docs">فایلی که کلاینت برای ذخیره فرستاده</param>
        /// <param name="isPublicFiles">file is Public = true OR private = false</param>
        /// <param name="folderToSave">محل ذخیره</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<Document> InsertDocument(IFormFile doc, bool isPublicFiles, DocumentFolder folderToSaveEnum, CancellationToken cancellationToken)
        {
            var folderToSave = folderToSaveEnum.ToString();

            var savedDocument = new Document();

            if (doc is not null)
            {
                if (!await _fileExtensions.ValidTypeFileAsync(doc, folderToSaveEnum))
                    throw new CustomException(_localizer["FileFormatIsNotValid"].Value);

                try
                {
                    var rootFolder = _dbContext.Documents.FirstOrDefault(_ => _.Name == folderToSave);
                    var item = new AttachmentDto() { File = doc };

                    if (rootFolder is null)
                    {
                        item.Name = folderToSave;
                        var entity = _mapper.Map<Document>(item);
                        entity.OwnerId = _accessor.HttpContext.GetUserId();
                        entity.IsPublic = true;
                        entity.IsActive = true;
                        entity.RootId = null;
                        entity.ParentId = null;
                        entity.Type = DocumentType.Folder;
                        var resInsert = _docRepository.InsertEntity(entity);
                        rootFolder = _dbContext.Documents.FirstOrDefault(_ => _.Name == folderToSave);
                    }
                    if (rootFolder is not null)
                    {
                        item.RootId = rootFolder.Id;
                        var entity = _mapper.Map<Document>(item);
                        entity.OwnerId = _accessor.HttpContext.GetUserId();
                        entity.ParentId = entity.ParentId != Guid.Empty ? entity.ParentId : null;
                        entity.Type = DocumentType.File;
                        entity.IsPublic = isPublicFiles;
                        var (IsSaved, FilePath, FileName) = await _customUploadFileService.UploadFileInCustomStorageAsync(item.File,
                            _configuration["localStaticStoragePath"], folderToSave);

                        if (IsSaved)
                        {
                            if (FileExtensions.IsImage(item.File))
                            {
                                var saveThumbnail = _customUploadFileService
                                    .UploadThumbnailInCustomStorageAsync(item.File, _configuration["localStaticStoragePath"],
                                    folderToSave + "\\Thumbnail", "tmb_" + FileName);
                            }

                            entity.RealName = item.File.FileName;
                            entity.GeneratedName = FileName;
                            entity.Extension = Path.GetExtension(FileName);
                            entity.MimeType = FileExtensions.GetMimeType(FileName);

                            var resInsert = _docRepository.InsertEntity(entity);
                            if (resInsert)
                            {
                                savedDocument = entity;
                            }
                            else
                            {
                                throw new Exception("فایل ذخیره نشد.");
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal($"exception: {ex.ToString()}");
                    throw;
                }
            }
            return savedDocument;
        }

        /// <summary>
        /// حذف از دیتابیس فایل 
        /// </summary>
        /// <param name="documentId">فایلی که کلاینت برای حذف فرستادهID </param>
        public void DeleteDocument(Guid documentId, DocumentFolder folderToSaveEnum)
        {
            try
            {
                var folderToSave = folderToSaveEnum.ToString();

                var file = _docRepository.GetById(documentId);
                if (file != null)
                {
                    _docRepository.Delete(file);
                    _customUploadFileService.DeleteFile(file.GeneratedName, _configuration["localStaticStoragePath"], folderToSave);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"exception: {ex.ToString()}");
                throw;
            }
        }
        /// <summary>
        /// حذف از دیتابیس فایل ها 
        /// </summary>
        /// <param name="documentIds"> </param>
        /// <exception cref="BadRequestException"></exception>
        public void DeleteDocuments(IEnumerable<Guid> documentIds, DocumentFolder folderToSaveEnum)
        {
            if (documentIds == null || !documentIds.Any())
            {
                return;
            }

            foreach (var documentId in documentIds)
            {
                DeleteDocument(documentId, folderToSaveEnum);
            }
        }

        public async Task<FileRecordDto> GetFileRecord(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new CustomException(_localizer["FileNotFound"]);
            }

            Document rootFolder = null;
            if (entity.Type == DocumentType.File && entity.RootId is not null)
            {
                rootFolder = await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == entity.RootId.Value);
            }

            if (entity.Type == DocumentType.File && !string.IsNullOrEmpty(entity.GeneratedName))
            {
                try
                {
                    var path = Path.Combine(
                        _configuration["localStaticStoragePath"],
                        rootFolder.Name,
                        entity.GeneratedName).Replace("\\", "/");
                    var file = File.OpenRead(path);

                    var mimeType = MimeKit.MimeTypes.GetMimeType(file.Name);

                    return new FileRecordDto
                    {
                        Id = id,
                        ContentType = mimeType,
                        FileName = entity.RealName
                    };
                }
                catch (Exception ex)
                {
                    if (_env.IsDevelopment())
                    {
                        return new FileRecordDto
                        {
                            Id = id,
                            ContentType = "",
                            FileName = entity.RealName
                        };
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new CustomException(_localizer["FileCorrupt"]);
            }

        }


    }
}
