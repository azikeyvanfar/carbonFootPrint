using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.Newss.Commands.CreateNews
{
    public class CreateNewsCommand : IRequest<bool>
    {
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
        /// first AttachmentId is mainPhoto
        /// comma seperated documentIds from document entity
        /// </summary>
        public List<IFormFile> Photos { get; set; } = new();


        /// <summary>
        /// فایل های پیوست
        /// </summary>
        public List<IFormFile> Attachments { get; set; } = new();
    }

    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IDocumentService _documentService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;

        public CreateNewsCommandHandler(
            IMapper mapper,
            IDocumentService documentService,
            IApplicationDbContext dbContext,
            IHttpContextAccessor accessor)
        {
            _mapper = mapper;
            _documentService = documentService;
            _dbContext = dbContext;
            _accessor = accessor;
        }
        public async Task<bool> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            var res = 0;

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
                    {
                        try
                        {
                            var entity = _mapper.Map<News>(request);
                            var currentUserId = _accessor.HttpContext.GetUserId();

                            entity.PublisherUserId = currentUserId;
                            _dbContext.News.Add(entity);
                            await _dbContext.SaveChangesAsync(cancellationToken);

                            if (request.Photos.Count != 0)
                            {
                                var resDocs = await _documentService.InsertDocuments(request.Photos, true, DocumentFolder.NewsPhotos, cancellationToken);
                                entity.PhotoIds = string.Join(",", resDocs.Select(x => x.Id).ToList());
                            }
                            if (request.Attachments.Count != 0)
                            {
                                var resDocs = await _documentService.InsertDocuments(request.Attachments, true, DocumentFolder.NewsAttachments, cancellationToken);
                                entity.AttachmentIds = string.Join(",", resDocs.Select(x => x.Id).ToList());
                            }

                            res = await _dbContext.SaveChangesAsync(cancellationToken);

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

            return res > 0;

        }




    }
}
