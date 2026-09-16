using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.QASubjects.Commands.CreateQASubject
{
    public class CreateQASubjectCommand : IRequest
    {
        public bool IsActive { get; set; }
        public string SubjectName { get; set; }
    }

    public class CreateQASubjectCommandHandler : IRequestHandler<CreateQASubjectCommand>
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<QASubject> _repository;
        private readonly IMapper _mapper;
        public CreateQASubjectCommandHandler(IMapper mapper, IRepository<QASubject> repo)
        {
            _repository = repo;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateQASubjectCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<QASubject>(request);

            //entity.OwnerId = _httpContextAccessor.HttpContext.GetUserId();
            var result = await _repository.GetAll().ToListAsync(cancellationToken);
            foreach (var item in result)
            {
                if (item.SubjectName == request.SubjectName)
                    throw new Exception("موضوع سوال تکراری می باشد");
            }

            _repository.Insert(entity);

            return Unit.Value;
        }

        Task IRequestHandler<CreateQASubjectCommand>.Handle(CreateQASubjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
