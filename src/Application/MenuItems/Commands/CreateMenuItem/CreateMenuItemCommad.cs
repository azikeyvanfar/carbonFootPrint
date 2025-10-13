using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.MenuItems.Commands.CreateMenuItem
{
    public class CreateMenuItemCommand : IRequest
    {
        public string Title { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid? DocumentId { get; set; }
        public string? Description { get; set; }
        public MenuType MenuType { get; set; }
        public RoleType RoleAccessType { get; set; }
        public Guid? FormID { get; set; }
        public Guid? PageRouteId { get; set; }
        public string? BaseUrl { get; set; }
        //public string? ExternalUrl { get; set; }
        public Guid? ExternalUrlId { get; set; }
        public int? PrevPriority { get; set; }
        public int? NextPriority { get; set; }
        public string Icon { get; set; }
        public Guid? PageId { get; set; }
        public Guid MenuId { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand>
    {
        private readonly IRepository<MenuItem> _repository;
        private readonly IMapper _mapper;

        public CreateMenuItemCommandHandler(IRepository<MenuItem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<Unit> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            if (request.MenuType == MenuType.Page && request.FormID.HasValue)
            {
                request.FormID = null;
            }
            var entity = _mapper.Map<MenuItem>(request);

            var siblingsCount = _repository.GetAll().Where(_ => _.ParentId == request.ParentId).Count();

            if (request.NextPriority == siblingsCount + 1)
            {
                request.NextPriority = null;
            }
            if (request.PrevPriority == null || request.PrevPriority == 0)
            {
                //آیتم اول
                entity.Priority = 1;
                var Items = _repository.GetAll().Where(f => f.MenuId == request.MenuId && f.ParentId == request.ParentId).ToList();
                Items.ForEach(a =>
                {
                    a.Priority++;
                    _repository.Update(a);
                });
            }
            else if (request.NextPriority == null)
            {
                //آیتم آخر
                entity.Priority = request.PrevPriority + 1;
            }
            else
            {
                // در میان آیتم ها
                entity.Priority = request.PrevPriority + 1;
                var Items = _repository.GetAll().Where(f => f.MenuId == request.MenuId
                   && f.Priority > request.PrevPriority && f.ParentId == request.ParentId).ToList();
                Items.ForEach(a =>
                {
                    a.Priority++;
                    _repository.Update(a);
                });
            }

            entity.IsOpen = false;
            _repository.Insert(entity);

            return Task.FromResult(Unit.Value);
        }
    }
}
