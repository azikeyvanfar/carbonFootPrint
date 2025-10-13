using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.MenuItems.Queries.GetMenuItemById
{
    public class GetMenuItemByIdQuery : IRequest<MenuItemDto>
    {
        public Guid Id { get; set; }

        public GetMenuItemByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public GetMenuItemByIdQueryHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MenuItemDto> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAllAsNoTracking()
                .Where(x => x.Id == request.Id)
                .ProjectTo<MenuItemDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
                ;
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            return entity;
        }
    }
}
