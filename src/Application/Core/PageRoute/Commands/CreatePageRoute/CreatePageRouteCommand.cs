using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.Core.PageRoute.Commands.CreatePageRoute
{
    public class CreatePageRouteCommand : IRequest<bool>
    {
        public RoleType RoleType { get; set; }
        public string Route { get; set; }
        public string RouteName { get; set; }
        public string Icon { get; set; }
    }

    public class CreatePageRouteCommandHandler : IRequestHandler<CreatePageRouteCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Domain.Entities.Core.PageRoute> _repository;
        public CreatePageRouteCommandHandler(IMapper mapper, IRepository<Domain.Entities.Core.PageRoute> pageRouteRepo)
        {
            _mapper = mapper;
            _repository = pageRouteRepo;
        }
        public Task<bool> Handle(CreatePageRouteCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.Core.PageRoute>(request);
            entity.IsActive = true;
            bool flag = false;
            flag = _repository.InsertEntity(entity);

            return Task.FromResult(flag);

        }
    }
}
