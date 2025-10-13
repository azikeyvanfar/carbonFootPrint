using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Menus.Queries.GetPageById
{
    public class GetMenuByIdQuery : IRequest<MenuDto>
    {
        public Guid Id { get; set; }

        public GetMenuByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public GetMenuByIdQueryHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            if (entity == null)
                throw new NullReferenceException();

            return Task.FromResult(_mapper.Map<MenuDto>(entity));
        }
    }
}
