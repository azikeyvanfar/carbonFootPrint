using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.Menus.Commands.UpdateMenu
{
    public class UpdateMenuCommand : IRequest
    {
        public Guid Id { get; set; }
        //public string Name { get; set; } = null!;
        public string Title { get; set; } = null!;
        public Guid? DocumentId { get; set; }
        public string Description { get; set; }
        //  public string? Options { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;
        public UpdateMenuCommandHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public Task<Unit> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            if (entity == null)
                throw new NullReferenceException();
            _mapper.Map(request, entity);
            _repository.Update(entity);
            return Task.FromResult(Unit.Value);
        }
    }
}
