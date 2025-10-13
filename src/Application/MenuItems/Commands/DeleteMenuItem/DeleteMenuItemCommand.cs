using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.MenuItems.Commands.DeleteMenuItem
{
    public class DeleteMenuItemCommand : IRequest
    {
        public Guid MenuItemId { get; set; }

        public DeleteMenuItemCommand(Guid id)
        {
            MenuItemId = id;
        }
    }

    public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand>
    {
        private readonly IRepository<MenuItem> _repository;

        public DeleteMenuItemCommandHandler(IRepository<MenuItem> repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.MenuItemId);
            if (entity == null)
                throw new NullReferenceException();


            var menuItems = _repository.GetAll().Where(_ => _.MenuId == entity.MenuId);

            if (menuItems.Any())
            {
                DelSubChildren(menuItems, request.MenuItemId);
            }
            // _repository.Delete(entity);

            return Task.FromResult(Unit.Value);
        }

        private void DelSubChildren(IQueryable<MenuItem> items, Guid parentMenuItemId)
        {
            if (items.Any(_ => _.ParentId == parentMenuItemId))
            {
                foreach (var item in items.Where(_ => _.ParentId == parentMenuItemId))
                {
                    DelSubChildren(items, item.Id);
                }
            }
            else
            {
                var entity = _repository.GetById(parentMenuItemId);
                _repository.Delete(entity);
            }
        }
    }
}
