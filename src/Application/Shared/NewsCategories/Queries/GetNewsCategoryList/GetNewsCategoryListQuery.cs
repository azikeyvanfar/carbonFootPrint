using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.NewsCategories.Queries.GetNewsCategoryList
{
    public class GetNewsCategoryListQuery : SearchQueryRequest, IRequest<SearchQueryResponse<SelectModel>>
    {
    }

    public class GetNewsCategoryListQueryHandler : IRequestHandler<GetNewsCategoryListQuery, SearchQueryResponse<SelectModel>>
    {
        private readonly IRepository<NewsCategory> _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;
        public GetNewsCategoryListQueryHandler(
            IRepository<NewsCategory> repository,
            IMapper mapper,
             IHttpContextAccessor accessor,
        UserManager<User> userManager,
        IApplicationDbContext dbContext
            )
        {
            _repository = repository;
            _mapper = mapper;

            _accessor = accessor;
            _userManager = userManager;
            _dbContext = dbContext;

        }

        public async Task<SearchQueryResponse<SelectModel>> Handle(GetNewsCategoryListQuery request, CancellationToken cancellationToken)
        {
            var userId = _accessor.HttpContext.GetUserId();

            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            #region load CurrentUser notifications with user's orgUnit - ( if Is Admin then load All notifications )
            var queryNotification = _repository.GetAllAsNoTracking()
                .Include(x => x.NewsCategoryOrgUnits)
                .AsQueryable()
                //.Where(x => x.IsNotifications)
                ;

            var userRoles = _dbContext.Set<UserRole>()
             .Where(x => x.UserId == user.Id).Select(x => x.Role).ToList();

            if (!userRoles.Any(x => x.IsAdministrator))
            {
                //var currentUserOrgCode = user.Employee.BusinessUnitIsSuiteId;
                //var currentUserOrgId = _dbContext.BusinessUnits.FirstOrDefault(x => x.IsSuiteId == currentUserOrgCode).Id;

                //queryNotification = queryNotification
                //    .Where(x => x.NewsCategoryOrgUnits.Any(l => l.OrgUnitId == currentUserOrgId));
            }
            else
            {
                queryNotification = _repository.GetAllAsNoTracking()
                .Include(x => x.NewsCategoryOrgUnits)
                .AsQueryable()
                //.Where(x => !x.IsNotifications)
                ;

            }
            #endregion

            var query = queryNotification
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<NewsCategoryDto>(_mapper.ConfigurationProvider);

            QueryablePaging<NewsCategoryDto> qp = await query.GridifyQueryableAsync<NewsCategoryDto>(request, null, cancellationToken);
            Paging<SelectModel> result = new(qp.Count, qp.Query.Select(_ => new SelectModel()
            {
                Text = _.Name,
                Value = _.Id.ToString()
            }));
            return new SearchQueryResponse<SelectModel>(request, result);
        }
    }
}
