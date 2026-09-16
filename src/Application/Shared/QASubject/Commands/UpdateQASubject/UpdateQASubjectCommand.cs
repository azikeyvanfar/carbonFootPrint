using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;

namespace ContractorBackend.Application.QASubjects.Commands.UpdateQASubject
{
    public class UpdateQASubjectCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string SubjectName { get; set; }
    }

    public class UpdateQASubjectCommandHandler : IRequestHandler<UpdateQASubjectCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<QASubject> _repository;
        public UpdateQASubjectCommandHandler(IMapper mapper, IRepository<QASubject> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public Task<Unit> Handle(UpdateQASubjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);

            if (entity is null)
                throw new NullReferenceException();

            _mapper.Map(request, entity);

            _repository.Update(entity);

            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<UpdateQASubjectCommand>.Handle(UpdateQASubjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
