using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
namespace ContractorBackend.Application.Menus.Commands.CreateMenu
{
    public class CreateMenuCommand : IRequest
    {
        public string Name { get; set; } = null!;
        public string Title { get; set; } = null!;
        public Guid? DocumentId { get; set; }
        public string? Description { get; set; }
        // public string? Options { get; set; }
        public bool IsActive { get; set; } = true;

    }

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public CreateMenuCommandHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<Unit> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Menu>(request);
            if (string.IsNullOrEmpty(request.Title))
            {
                throw new Exception("عنوان منو را وارد نمایید");
            }

            _repository.Insert(entity);

            return Task.FromResult(Unit.Value);
        }
    }
}
