using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Shared.Newss.Commands.UpdateNews
{
    public class UpdateNewsCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsSpecial { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public long RelatedOrgUnitId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime? StartShownDate { get; set; }
        public DateTime? EndShownDate { get; set; }

        /// <summary>
        /// content of news
        /// </summary>
        public string BodyContent { get; set; }



        /// <summary>
        /// عکس های ضمیمه که قبلا ذخیره شده اند
        /// </summary>
        public List<Guid> PhotoIds { get; set; } = new();
        /// <summary>
        /// فایل های ضمیمه که قبلا ذخیره شده اند
        /// </summary>
        public List<Guid> AttachmentIds { get; set; } = new();

        /// <summary>
        /// عکس های پیوست
        /// </summary>
        public List<IFormFile> Photos { get; set; } = new();
        /// <summary>
        /// فایل های پیوست
        /// </summary>
        public List<IFormFile> Attachments { get; set; } = new();
    }

    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IDocumentService _documentService;
        private readonly IRepository<Document> _docRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public UpdateNewsCommandHandler
            (
            IMapper mapper,
            IDocumentService documentService,
            IRepository<Document> docRepository,
            IConfiguration configuration,
            IApplicationDbContext context,
            IHttpContextAccessor accessor,
            UserManager<User> userManager,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _mapper = mapper;
            _docRepository = docRepository;
            _accessor = accessor;
            _configuration = configuration;
            _docRepository = docRepository;
            _dbContext = context;
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer;
            _documentService = documentService;
        }

        public async Task<bool> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.News.FirstOrDefault(x => x.Id == request.Id);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {

                        #region Delete Attachments
                        if (!string.IsNullOrWhiteSpace(entity.PhotoIds))
                        {
                            var entityIds = entity.PhotoIds.Split(",").Select(x => Guid.Parse(x)).ToList();
                            var deleteList = entityIds.Where(x => request.PhotoIds.All(l => l != x));
                            _documentService.DeleteDocuments(deleteList, DocumentFolder.NewsPhotos);

                            var remainingDocIds = entityIds.Except(deleteList);
                            entity.PhotoIds = string.Join(",", remainingDocIds);
                        }
                        if (!string.IsNullOrWhiteSpace(entity.AttachmentIds))
                        {
                            var entityIds = entity.AttachmentIds.Split(",").Select(x => Guid.Parse(x)).ToList();
                            var deleteList = entityIds.Where(x => request.AttachmentIds.All(l => l != x));
                            _documentService.DeleteDocuments(deleteList, DocumentFolder.NewsAttachments);

                            var remainingDocIds = entityIds.Except(deleteList);
                            entity.AttachmentIds = string.Join(",", remainingDocIds);
                        }
                        #endregion

                        _mapper.Map(request, entity);

                        _dbContext.News.Update(entity);

                        #region Add Attachments
                        if (request.Photos.Count != 0)
                        {
                            var resDocs = await _documentService.InsertDocuments(request.Photos, true, DocumentFolder.NewsPhotos, cancellationToken);
                            var addedDocumentIds = string.Join(",", resDocs.Select(x => x.Id).ToList());
                            entity.PhotoIds = string.IsNullOrWhiteSpace(entity.PhotoIds) ? addedDocumentIds : entity.PhotoIds + "," + addedDocumentIds;
                        }
                        if (request.Attachments.Count != 0)
                        {
                            var resDocs = await _documentService.InsertDocuments(request.Attachments, true, DocumentFolder.NewsAttachments, cancellationToken);
                            var addedDocumentIds = string.Join(",", resDocs.Select(x => x.Id).ToList());
                            entity.AttachmentIds = string.IsNullOrWhiteSpace(entity.AttachmentIds) ? addedDocumentIds : entity.AttachmentIds + "," + addedDocumentIds;
                        }
                        #endregion

                        await _dbContext.SaveChangesAsync(cancellationToken);

                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                    await transaction.DisposeAsync();
                }
            });

            return true;

        }

    }
}
