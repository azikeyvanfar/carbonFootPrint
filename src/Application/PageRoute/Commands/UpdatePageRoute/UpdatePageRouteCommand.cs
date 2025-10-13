using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.PageRoutes.Commands.UpdatePageRoute
{
    public class UpdatePageRouteCommand : IRequest<bool>
    {
        public Guid PageRouteId { get; set; }
        public RoleType RoleType { get; set; }
        public string Route { get; set; }
        public string RouteName { get; set; }
        public string Icon { get; set; }
    }

    public class UpdatePageRouteCommandHandler : IRequestHandler<UpdatePageRouteCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<PageRoute> _repository;
        private readonly IRepository<MenuItem> _mnuItemrepository;
        public UpdatePageRouteCommandHandler(IMapper mapper, IRepository<PageRoute> pageRouteRep, IRepository<MenuItem> mnuItemrepository)
        {
            _mapper = mapper;
            _repository = pageRouteRep;
            _mnuItemrepository = mnuItemrepository;
        }

        public async Task<bool> Handle(UpdatePageRouteCommand request, CancellationToken cancellationToken)
        {
            bool flag = false;
            var entity = _repository.GetById(request.PageRouteId);

            if (entity is null)
                throw new NullReferenceException();

            _mapper.Map(request, entity);

            flag = _repository.UpdateEntity(entity);
            await UpdateIconMenu(request, flag);
            if (!flag)
            {
                throw new DbUpdateException();
            }
            return flag;
        }
        private async Task UpdateIconMenu(UpdatePageRouteCommand request, bool flag)
        {
            var menuItem = _mnuItemrepository.GetAll().Where(m => m.PageRouteId == request.PageRouteId && m.RoleAccessType == request.RoleType).ToList();
            if (menuItem.Count > 0)
            {
                foreach (var item in menuItem)
                {
                    item.Icon = request.Icon;
                }
                var a = _mnuItemrepository.UpdateRange(menuItem);
            }
            await Task.FromResult(flag);
        }
    }
}