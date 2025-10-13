using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.Core.MenuItems.Commands.UpdateMenuItem
{
    public class UpdateMenuItemCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid? DocumentId { get; set; }
        public string Description { get; set; }
        public MenuType MenuType { get; set; }
        public RoleType RoleAccessType { get; set; }
        public Guid? FormID { get; set; }
        public Guid? PageRouteId { get; set; }
        public string BaseUrl { get; set; }
        // public string ExternalUrl { get; set; }
        public Guid? ExternalUrlId { get; set; }
        public Guid? PageId { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; } = true;

        //public int? PrevPriority { get; set; }
        //public int? NextPriority { get; set; }
        public string Icon { get; set; }
        public int? Priority { get; set; }
        public Guid NextId { get; set; }
    }

    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;
        public UpdateMenuItemCommandHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<Unit> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            int? prevPriority = null;
            int? nextPriority = null;
            if (entity == null)
                throw new NullReferenceException();
            int? lastPriority = null;
            //
            //var previousPriority = entity.Priority;
            if (request.Priority != entity.Priority) //priority should be updated
            {
                prevPriority = request.Priority != null ? request.Priority - 1 : null;
                var siblingsCount = _repository.GetAll().Where(_ => _.ParentId == request.ParentId).Count();
                if (request.Priority < siblingsCount)
                {
                    nextPriority = request.Priority + 1;
                }
                //var nextEntity = _repository.GetById(request.NextId);

                //if (nextEntity == null)
                //    throw new NullReferenceException();

                // entity.ParentId = nextEntity.ParentId;
                if (prevPriority == null || prevPriority == 0)
                {
                    //آیتم اول
                    var Items = _repository.GetAll().Where(f => f.MenuId == entity.MenuId && f.ParentId == entity.ParentId).ToList();
                    Items.ForEach(a =>
                    {
                        a.Priority++;
                        _repository.Update(a);
                    });
                    entity.Priority = 1;
                }
                else if (nextPriority == null)
                {
                    //آیتم آخر
                    entity.Priority = prevPriority + 1;

                }
                else
                {
                    // در میان آیتم ها
                    var Items = _repository.GetAll().Where(f => f.MenuId == entity.MenuId &&
                           f.Priority > prevPriority && f.ParentId == entity.ParentId).ToList();
                    Items.ForEach(a =>
                    {
                        a.Priority++;
                        _repository.Update(a);
                    });
                    entity.Priority = prevPriority + 1;
                }
                _repository.Update(entity);

                lastPriority = entity.Priority;
            }

            _mapper.Map(request, entity);
            if (lastPriority != null)
            {
                entity.Priority = lastPriority;
            }
            _repository.Update(entity);
            return Task.FromResult(Unit.Value);
        }
    }
}
