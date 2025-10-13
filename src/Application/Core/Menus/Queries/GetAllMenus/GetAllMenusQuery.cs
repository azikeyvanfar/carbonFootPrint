using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.Menus.Queries.GetAllMenus
{
    public class GetAllMenusQuery : SearchQueryRequest, IRequest<SearchQueryResponse<MenuDto>>
    {

    }

    public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, SearchQueryResponse<MenuDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Menu> _repository;

        public GetAllMenusQueryHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking().ProjectTo<MenuDto>(_mapper.ConfigurationProvider);

            QueryablePaging<MenuDto> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<MenuDto> result = new(qp.Count, qp.Query.ToList());

            return new SearchQueryResponse<MenuDto>(request, result);
        }
    }
}
