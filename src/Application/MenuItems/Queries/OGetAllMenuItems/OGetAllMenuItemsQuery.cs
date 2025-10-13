using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ContractorBackend.Application.MenuItems.Queries.OGetAllMenuItems
{
    public class OGetAllMenuItemsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<MenuItemDto>>
    {
        public Guid MenuId { get; set; }
        public OGetAllMenuItemsQuery(Guid menuId)
        {
            MenuId = menuId;
        }
        public OGetAllMenuItemsQuery()
        {

        }
    }

    public class OGetAllMenuItemsQueryHandler : IRequestHandler<OGetAllMenuItemsQuery, SearchQueryResponse<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public OGetAllMenuItemsQueryHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<MenuItemDto>> Handle(OGetAllMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                  .Include(x => x.Owner)
                    .Include(x => x.Children)
                    .Include(x => x.ParentMenuItem)
                    .Include(x => x.Menu)
                    .Where(x => x.MenuId == request.MenuId && x.ParentId == null)
                    .OrderBy(x => x.Priority)
                    .ProjectTo<MenuItemDto>(_mapper.ConfigurationProvider);

            QueryablePaging<MenuItemDto> qp = await query.GridifyQueryableAsync<MenuItemDto>(request, null, cancellationToken);
            Paging<MenuItemDto> result = new(qp.Count, qp.Query);
            return new SearchQueryResponse<MenuItemDto>(request, result);
        }

    }
}
