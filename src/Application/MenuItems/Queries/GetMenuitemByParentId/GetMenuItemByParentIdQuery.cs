using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.MenuItems.Queries.GetMenuItemByParentId
{
    public class GetMenuItemByParentIdQuery : IRequest<MenuItemDto>
    {
        public Guid ParentId { get; set; }

        public GetMenuItemByParentIdQuery(Guid parentId)
        {
            ParentId = parentId;
        }
    }

    public class GetMenuItemByParentIdQueryHandler : IRequestHandler<GetMenuItemByParentIdQuery, MenuItemDto>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public GetMenuItemByParentIdQueryHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MenuItemDto> Handle(GetMenuItemByParentIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAll()
                 .Include(x => x.Owner)
                // .ThenInclude(x => x.UserDetails)

                .FirstOrDefaultAsync(x => x.Id == request.ParentId, cancellationToken);
            ;
            if (entity == null)
                throw new NullReferenceException();

            return _mapper.Map<MenuItemDto>(entity);
        }
    }
}
