using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Shared.NewsCategories.Commands.DeleteNewsCategory
{
    public class DeleteNewsCategoryCommand : IRequest
    {
        public Guid NewsCategoryId { get; set; }
        public DeleteNewsCategoryCommand(Guid Id)
        {
            NewsCategoryId = Id;
        }
    }

    public class DeleteNewsCategoryCommandHandler : IRequestHandler<DeleteNewsCategoryCommand>
    {
        private readonly IRepository<NewsCategory> _repository;
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public DeleteNewsCategoryCommandHandler(
            IRepository<NewsCategory> repository,
            IApplicationDbContext context,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _repository = repository;
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        public Task<Unit> Handle(DeleteNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.NewsCategoryId);

            if (entity is null)
                throw new NullReferenceException();

            if (_context.News.Any(_ => _.CategoryId == entity.Id))
            {
                throw new CustomException(_sharedLocalizer["DeleteNewsCategoryCommandHasNewsError"]);
            }
            _repository.Delete(entity);

            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<DeleteNewsCategoryCommand>.Handle(DeleteNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
