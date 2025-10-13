using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.Newss.Queries.GetAllNotificationNews
{
    public class GetAllNotificationNewsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<NewsDto>>
    {
        public bool IsManager { get; set; }
    }
    public class GetAllNotificationNewsQueryHandler : IRequestHandler<GetAllNotificationNewsQuery, SearchQueryResponse<NewsDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<News> _repository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository<Document> _docRepository;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;

        public GetAllNotificationNewsQueryHandler(
            IMapper mapper,
            IRepository<News> repository,
            IHttpContextAccessor accessor,
            IRepository<Document> docRepo,
            UserManager<User> userManager,
            IApplicationDbContext dbContext
            )
        {
            _mapper = mapper;
            _repository = repository;
            _accessor = accessor;
            _docRepository = docRepo;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<SearchQueryResponse<NewsDto>> Handle(GetAllNotificationNewsQuery request, CancellationToken cancellationToken)
        {

            request.OrderBy = "StartShownDate desc";

            var query = _repository.GetAllAsNoTracking()
                .Include(s => s.Category).ThenInclude(x => x.NewsCategoryOrgUnits)
                .Include(s => s.PublisherUser)
                .Where(x => x.Category.IsNotifications)
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .AsQueryable();



            if (!request.IsManager)
            {
                query = query.Where(x => x.IsActive);
            }

            //query = query.Concat(queryNotif);


            QueryablePaging<News> qp1 = await query.GridifyQueryableAsync<News>(request, null, cancellationToken);
            var qp = _mapper.Map<List<NewsDto>>(qp1.Query);
            Paging<NewsDto> result = new(qp1.Count, qp);

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
            }
            return new SearchQueryResponse<NewsDto>(request, result);

        }
    }
}
