using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Shared.QuestionAnswers.Commands.ChangeQAStatus
{
    public class ChangeQaStatusCommand : IRequest<bool>
    {
        public Guid QuetionAnswerId { get; set; }

        public bool IsPopular { get; set; }
        public ChangeQaStatusCommand(Guid id, bool isPopular)
        {
            QuetionAnswerId = id;
            IsPopular = isPopular;
        }
    }

    public class ChangeQaStatusCommandHandler : IRequestHandler<ChangeQaStatusCommand, bool>
    {
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IHttpContextAccessor _accessor;
        public ChangeQaStatusCommandHandler(IRepository<QuestionAnswer> repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _accessor = accessor;
        }

        public async Task<bool> Handle(ChangeQaStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.QuetionAnswerId);
            if (entity == null)
                throw new NullReferenceException();

            if ((entity.IsPopular && request.IsPopular) || (!entity.IsPopular && !request.IsPopular))
            {
                throw new BadRequestException();
            }
            entity.IsPopular = request.IsPopular;
            var res = _repository.UpdateEntity(entity);
            return res;
        }
    }
}
