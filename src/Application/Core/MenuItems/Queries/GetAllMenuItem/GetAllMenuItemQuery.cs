using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.MenuItems.Queries.GetAllMenuItem
{
    public class GetAllMenuItemQuery : SearchQueryRequest, IRequest<SearchQueryResponse<MenuItemDto>>
    {
    }
    public class GetAllMenuItemQueryHandler : IRequestHandler<GetAllMenuItemQuery, SearchQueryResponse<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public GetAllMenuItemQueryHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<MenuItemDto>> Handle(GetAllMenuItemQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                  .Include(x => x.Owner)
                    .Include(x => x.Children)
                    .Include(x => x.ParentMenuItem)
                    .Include(x => x.Menu)
                    .OrderBy(x => x.Priority);

            QueryablePaging<MenuItem> qp1 = await query.GridifyQueryableAsync(request, null, cancellationToken);
            var qp = _mapper.Map<List<MenuItemDto>>(qp1.Query);
            Paging<MenuItemDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<MenuItemDto>(request, result);
        }

    }
}
