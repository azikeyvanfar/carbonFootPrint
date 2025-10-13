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

namespace ContractorBackend.Application.Shared.NewsCategories.Queries.GetAllNewsCategories
{
    public class GetAllNewsCategoriesQuery : SearchQueryRequest, IRequest<SearchQueryResponse<NewsCategoryDto>>
    {
    }

    public class GetAllNewsCategoriesQueryHandler :
        IRequestHandler<GetAllNewsCategoriesQuery, SearchQueryResponse<NewsCategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<NewsCategory> _repository;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;
        public GetAllNewsCategoriesQueryHandler(
            IMapper mapper,
            IRepository<NewsCategory> repository,
            IHttpContextAccessor accessor,
        UserManager<User> userManager,
        IApplicationDbContext dbContext
            )
        {
            _mapper = mapper;
            _repository = repository;

            _accessor = accessor;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<SearchQueryResponse<NewsCategoryDto>> Handle(GetAllNewsCategoriesQuery request, CancellationToken cancellationToken)
        {
            var userId = _accessor.HttpContext.GetUserId();

            var user = await _userManager.Users

            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);


            #region load CurrentUser Categories with user's orgUnit - ( if Is Admin then load All notifications )
            var query1 = _repository.GetAllAsNoTracking()
                .Include(x => x.NewsCategoryOrgUnits)
                .AsQueryable();

            var userRoles = _dbContext.Set<UserRole>()
             .Where(x => x.UserId == user.Id).Select(x => x.Role).ToList();

            if (!userRoles.Any(x => x.IsAdministrator))
            {
                //var currentUserOrgCode = user.Employee.BusinessUnitIsSuiteId;
                //var currentUserOrgId = _dbContext.BusinessUnits.FirstOrDefault(x => x.IsSuiteId == currentUserOrgCode).Id;

                //query1 = query1
                //    .Where(x => x.NewsCategoryOrgUnits.Any(l => l.OrgUnitId == currentUserOrgId));
            }
            #endregion


            var query = query1
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<NewsCategoryDto>(_mapper.ConfigurationProvider);

            QueryablePaging<NewsCategoryDto> qp = await query.GridifyQueryableAsync<NewsCategoryDto>(request, null, cancellationToken);
            Paging<NewsCategoryDto> result = new(qp.Count, qp.Query.ToList());

            foreach (var item in result.Data)
            {
                //item.OrgUnits = _dbContext.NewsCategoryOrgUnits
                //    .Include(x => x.OrgUnit)
                //    .Where(x => x.NewsCategoryId == item.Id)
                //    .Select(x => new SelectModel
                //    {
                //        Value = x.OrgUnit.Id.ToString(),
                //        Text = x.OrgUnit.des_busun,
                //    })
                //    .ToList();

                var lastModifiedByUser = _dbContext.Set<User>().FirstOrDefault(x => x.Id == item.LastModifiedByUserId);
                item.LastModifiedByFullName = (lastModifiedByUser != null) ? lastModifiedByUser.FirstName + " " + lastModifiedByUser.LastName : "";

            }

            return new SearchQueryResponse<NewsCategoryDto>(request, result);
        }
    }
}
