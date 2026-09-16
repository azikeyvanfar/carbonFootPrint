using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Menus.Commands.DeleteMenu
{
    public class DeleteMenuCommand : IRequest
    {
        public Guid MenuId { get; set; }

        public DeleteMenuCommand(Guid id)
        {
            MenuId = id;
        }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
    {
        private readonly IRepository<Menu> _repository;

        public DeleteMenuCommandHandler(IRepository<Menu> repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.MenuId);
            if (entity == null)
                throw new NullReferenceException();

            _repository.Delete(entity);

            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<DeleteMenuCommand>.Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
