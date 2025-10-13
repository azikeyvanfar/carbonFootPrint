using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.Newss.Queries.GetNewsById
{
    public class GetNewsByIdQuery : IRequest<NewsDto>
    {
        public Guid Id { get; set; }
        public GetNewsByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<News> _repository;
        private readonly IRepository<NewsCategory> _catRepository;
        private readonly IRepository<Document> _docRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;


        public GetNewsByIdQueryHandler(IMapper mapper,
            IRepository<News> repository,
            IRepository<Document> docRepository,
            IRepository<NewsCategory> catRepository,
            IHttpContextAccessor accessor,
            UserManager<User> userManager,
            IApplicationDbContext dbContext
            )
        {
            _mapper = mapper;
            _repository = repository;
            _docRepository = docRepository;
            _catRepository = catRepository;
            _accessor = accessor;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<NewsDto> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _accessor.HttpContext.GetUserId();

            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            //its not use in code and publish log has error
            //var currentUserOrgCode = user.UserDetails.BusinessUnitId;
            //var currentUserOrgId = _dbContext.BusinessUnits.FirstOrDefault(x => x.IsSuiteId == currentUserOrgCode).Id;

            var entity = await _repository.GetAllAsNoTracking()
                        .Include(s => s.Category).ThenInclude(x => x.NewsCategoryOrgUnits)
                        .Include(s => s.PublisherUser)
                        .Where(x => x.IsActive)
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (entity is null)
            {
                throw new NullReferenceException();
            }

            var dto = _mapper.Map<NewsDto>(entity);

            // var businessUnit = await _dbContext.BusinessUnits.FirstOrDefaultAsync(x => x.IsSuiteId == entity.RelatedOrgUnitId);

            // dto.RelatedOrgUnitName = businessUnit !=null ? businessUnit.des_busun : string.Empty;

            if (!string.IsNullOrWhiteSpace(dto.PhotosId) && dto.PhotosId.Length > 0)
            {
                var arr = dto.PhotosId.Split(",");
                foreach (var doc in arr)
                {
                    if (doc.Trim() != string.Empty)
                    {
                        var docEntity = _docRepository.GetById(new Guid(doc));
                        if (docEntity is not null)
                        {
                            dto.PhotosVM.Add(new FileVM()
                            {
                                DocId = docEntity.Id,
                                Extension = docEntity.Extension,
                                Name = docEntity.RealName,
                                MimeType = docEntity.MimeType
                            });
                        }

                    }

                }
            }

            if (!string.IsNullOrWhiteSpace(dto.AttachmentIds) && dto.AttachmentIds.Length > 0)
            {
                var arr = dto.AttachmentIds.Split(",");
                foreach (var doc in arr)
                {
                    if (doc.Trim() != string.Empty)
                    {
                        var docEntity = _docRepository.GetById(new Guid(doc));
                        if (docEntity is not null)
                        {
                            dto.AttachmentsVM.Add(new FileVM()
                            {
                                DocId = docEntity.Id,
                                Extension = docEntity.Extension,
                                Name = docEntity.RealName,
                                MimeType = docEntity.MimeType
                            });
                        }

                    }

                }
            }

            var lastModifiedByUser = _dbContext.Set<User>().FirstOrDefault(x => x.Id == dto.LastModifiedByUserId);
            dto.LastModifiedByFullName = (lastModifiedByUser != null) ? lastModifiedByUser.FirstName + " " + lastModifiedByUser.LastName : "";

            return dto;
        }
    }
}
