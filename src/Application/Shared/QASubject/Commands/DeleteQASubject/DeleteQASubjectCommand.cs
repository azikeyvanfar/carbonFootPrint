using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.QASubjects.Commands.DeleteQASubject
{
    public class DeleteQASubjectCommand : IRequest
    {
        public Guid SubId { get; set; }
        public DeleteQASubjectCommand(Guid id)
        {
            SubId = id;
        }
    }

    public class DeleteQASubjectCommandHandler : IRequestHandler<DeleteQASubjectCommand>
    {
        private readonly IRepository<QASubject> _repository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DeleteQASubjectCommandHandler(IRepository<QASubject> repository, IStringLocalizer<SharedResource> localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }
        public Task<Unit> Handle(DeleteQASubjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetAll()
                .Include(x => x.QuestionAnswers)
                .FirstOrDefault(x => x.Id == request.SubId);

            if (entity is null)
            {
                throw new NullReferenceException();
            }

            try
            {
                _repository.Delete(entity);

            }
            catch (Exception e)
            {
                throw new CustomException(_localizer["DeleteSubjectError"], e);
            }

            return Task.FromResult(Unit.Value);
        }
    }
}
