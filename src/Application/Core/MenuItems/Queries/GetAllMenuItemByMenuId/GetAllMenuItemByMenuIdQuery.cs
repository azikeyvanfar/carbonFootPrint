using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.MenuItems.Queries.GetAllMenuItemByMenuId
{

    public class GetAllMenuItemByMenuIdQuery : SearchQueryRequest, IRequest<SearchQueryResponse<MenuItemDto>>
    {
        public Guid MenuId { get; set; }
        public RoleType? RoleType { get; set; }

    }
    public class GetAllMenuItemByMenuIdQueryHandler : IRequestHandler<GetAllMenuItemByMenuIdQuery, SearchQueryResponse<MenuItemDto>>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public GetAllMenuItemByMenuIdQueryHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<MenuItemDto>> Handle(GetAllMenuItemByMenuIdQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.Children)
                .Include(x => x.ParentMenuItem)
                .Include(x => x.Menu)
                .Include(x => x.PageRoute)
                .Where(c => c.MenuId == request.MenuId);

            if (request.RoleType is not null)
            {
                query = query.Where(c => c.RoleAccessType == request.RoleType);
            }

            query = query.OrderBy(x => x.Priority);

            var queryDto = query
                .ProjectTo<MenuItemDto>(_mapper.ConfigurationProvider);

            QueryablePaging<MenuItemDto> qp1 = await queryDto.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<MenuItemDto> result = new(qp1.Count, qp1.Query);
            return new SearchQueryResponse<MenuItemDto>(request, result);

        }

    }
}
