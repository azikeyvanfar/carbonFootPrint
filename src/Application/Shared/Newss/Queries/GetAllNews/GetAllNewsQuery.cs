using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.Newss.Queries.GetAllNews
{
    public class GetAllNewsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<NewsDto>>
    {
        public bool IsManager { get; set; }
    }
    public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, SearchQueryResponse<NewsDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<News> _repository;
        private readonly IRepository<Document> _docRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public GetAllNewsQueryHandler(
            IMapper mapper,
            IRepository<News> repository,
            IRepository<Document> docRepo,
            IApplicationDbContext dbContext,
            IApplicationUserManager userManager)
        {
            _mapper = mapper;
            _repository = repository;
            _docRepository = docRepo;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<SearchQueryResponse<NewsDto>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
        {

            request.OrderBy = "StartShownDate desc";

            var query = _repository.GetAllAsNoTracking()
                .Include(s => s.Category).ThenInclude(x => x.NewsCategoryOrgUnits)
                .Include(s => s.PublisherUser)
                .Include(s => s.PublisherUser)
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .AsQueryable();

            #region Show List of Current User OrgUnit if is MANAGER
            if (request.IsManager)
            {
                var currentUser = await _userManager.GetCurrentUserAsync();
                //var busId = currentUser.Employee.BusinessUnitIsSuiteId;

                //var isUserAdministrator = _dbContext.Set<UserRole>().Where(x => x.UserId == currentUser.Id).Include(x => x.Role).Any(x => x.Role.IsAdministrator);
                //if (isUserAdministrator == false)
                //{
                //    var currentUserOrgUnit = _dbContext.BusinessUnits.Where(x => x.IsActive && x.IsSuiteId == busId).FirstOrDefault();
                //    if (currentUserOrgUnit != null)
                //    {
                //        query = query.Where(x => x.Category.NewsCategoryOrgUnits.Any(l => l.OrgUnitId == currentUserOrgUnit.Id));
                //    }
                //}
            }
            #endregion

            if (!request.IsManager)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsActive).Where(x =>
                    (x.StartShownDate == null || x.StartShownDate <= now)
                    && (x.EndShownDate == null || x.EndShownDate >= now)
                );
            }

            var queryDto = query
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<NewsDto>(_mapper.ConfigurationProvider);

            QueryablePaging<NewsDto> qp1 = await queryDto.GridifyQueryableAsync<NewsDto>(request, null, cancellationToken);
            Paging<NewsDto> result = new(qp1.Count, qp1.Query.ToList());

            List<FileVM> files = new List<FileVM>();
            foreach (var item in result.Data)
            {
                if (!string.IsNullOrWhiteSpace(item.PhotosId) && item.PhotosId.Length > 0)
                {
                    var arr = item.PhotosId.Split(",");
                    foreach (var doc in arr)
                    {
                        if (doc.Trim() != string.Empty)
                        {
                            var docEntity = _docRepository.GetById(new Guid(doc));
                            if (docEntity is not null)
                            {
                                item.PhotosVM.Add(new FileVM()
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

                if (!string.IsNullOrWhiteSpace(item.AttachmentIds) && item.AttachmentIds.Length > 0)
                {
                    var arr = item.AttachmentIds.Split(",");
                    foreach (var doc in arr)
                    {
                        if (doc.Trim() != string.Empty)
                        {
                            var docEntity = _docRepository.GetById(new Guid(doc));
                            if (docEntity is not null)
                            {
                                item.AttachmentsVM.Add(new FileVM()
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


                var lastModifiedByUser = _dbContext.Set<User>().FirstOrDefault(x => x.Id == item.LastModifiedByUserId);
                item.LastModifiedByFullName = (lastModifiedByUser != null) ? lastModifiedByUser.FirstName + " " + lastModifiedByUser.LastName : "";
            }

            return new SearchQueryResponse<NewsDto>(request, result);

        }
    }
}
