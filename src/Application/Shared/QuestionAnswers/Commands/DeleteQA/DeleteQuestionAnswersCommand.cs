using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.QuestionAnswers.Commands.DeleteQA
{
    public class DeleteQuestionAnswersCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public DeleteQuestionAnswersCommand(Guid id, bool isAdmin)
        {
            Id = id;
            IsAdmin = isAdmin;
        }
    }

    public class DeleteQuestionAnswersCommandHandler : IRequestHandler<DeleteQuestionAnswersCommand>
    {
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IHttpContextAccessor _accessor;
        public DeleteQuestionAnswersCommandHandler(IRepository<QuestionAnswer> repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _accessor = accessor;
        }
        public Task<Unit> Handle(DeleteQuestionAnswersCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            if (entity == null)
                throw new NullReferenceException();

            if (entity.IsPopular && !request.IsAdmin)
            {
                throw new CustomException("سوالات متداول قابل حذف شدن نیست.");
            }


            if (request.IsAdmin || entity.QuestionerId == _accessor.HttpContext.GetUserId())
            {
                _repository.Delete(entity);
            }
            else
                throw new ForbiddenAccessException();

            return Task.FromResult(Unit.Value);
        }
    }
}
