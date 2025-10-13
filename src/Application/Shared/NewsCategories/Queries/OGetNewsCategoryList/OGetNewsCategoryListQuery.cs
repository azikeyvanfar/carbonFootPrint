using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Application.Shared.NewsCategories.Queries.OGetNewsCategoryList
{
    public class OGetNewsCategoryListQuery : SearchQueryRequest, IRequest<SearchQueryResponse<SelectModel>>
    {
    }

    public class OGetNewsCategoryListQueryHandler : IRequestHandler<OGetNewsCategoryListQuery, SearchQueryResponse<SelectModel>>
    {
        private readonly IRepository<NewsCategory> _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;
        public OGetNewsCategoryListQueryHandler(
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

        public async Task<SearchQueryResponse<SelectModel>> Handle(OGetNewsCategoryListQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
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
