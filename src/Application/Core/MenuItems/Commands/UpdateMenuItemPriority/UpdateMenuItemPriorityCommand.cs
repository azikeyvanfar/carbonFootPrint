using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;


namespace ContractorBackend.Application.Core.MenuItems.Commands.UpdateMenuItemPriority
{
    public class UpdateMenuItemPriorityCommand : IRequest
    {
        public Guid Id { get; set; }
        public int? PrevPriority { get; set; }
        public int? NextPriority { get; set; }
        public Guid NextId { get; set; }
    }

    public class UpdateMenuItemPriorityCommandHandler : IRequestHandler<UpdateMenuItemPriorityCommand>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;
        public UpdateMenuItemPriorityCommandHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<Unit> Handle(UpdateMenuItemPriorityCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            if (entity == null)
                throw new NullReferenceException();

            var nextEntity = _repository.GetById(request.NextId);
            if (nextEntity == null)
                throw new NullReferenceException();

            entity.ParentId = nextEntity.ParentId;
            if (request.PrevPriority == null || request.PrevPriority == 0)
            {
                //آیتم اول

                var Items = _repository.GetAll().Where(f => f.MenuId == entity.MenuId && f.ParentId == entity.ParentId).ToList();
                Items.ForEach(a =>
                {
                    a.Priority = a.Priority + 1;
                    _repository.Update(a);
                });
                entity.Priority = 1;


            }
            else if (request.NextPriority == null)
            {
                //آیتم آخر
                entity.Priority = request.PrevPriority + 1;

            }
            else
            {
                // در میان آیتم ها
                var Items = _repository.GetAll().Where(f => f.MenuId == entity.MenuId &&
                       f.Priority > request.PrevPriority && f.ParentId == entity.ParentId).ToList();
                Items.ForEach(a =>
                {
                    a.Priority++;
                    _repository.Update(a);
                });
                entity.Priority = request.PrevPriority + 1;

            }
            _repository.Update(entity);
            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<UpdateMenuItemPriorityCommand>.Handle(UpdateMenuItemPriorityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
