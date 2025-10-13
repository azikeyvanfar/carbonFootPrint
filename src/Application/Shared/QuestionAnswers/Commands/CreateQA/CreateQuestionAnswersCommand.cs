using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;

namespace ContractorBackend.Application.QuestionAnswers.Commands.CreateQA
{
    public class CreateQuestionAnswersCommand : IRequest
    {
        public CreateQuestionAnswersCommand()
        {

        }
        //public CreateQuestionAnswersCommand(bool isAdminType)
        //{
        //    IsAdminType = isAdminType; 
        //}
        public bool IsActive { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreateDate { get; set; }
        //public Guid OrgUnitId { get; set; }
        public Guid QaSubjectId { get; set; }
        public long QuestionerId { get; set; }
        public long ResponderId { get; set; }
        public DateTime? ResponseDate { get; set; }
        public bool IsPrivate { get; set; }

        /// <summary>
        /// اگر ادمین باشد خودش هم جواب میدهد اگر نباشد باید کسی جز خود سئوال کننده جواب دهد
        /// </summary>
        public bool? IsAdminType { get; set; }
    }

    public class CreateQuestionAnswersCommandHandler : IRequestHandler<CreateQuestionAnswersCommand>
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IMapper _mapper;
        public CreateQuestionAnswersCommandHandler(IRepository<QuestionAnswer> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateQuestionAnswersCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<QuestionAnswer>(request);
            entity.CreateDate = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(request.Answer))
            {
                entity.ResponseDate = DateTime.Now;
            }

            _repository.Insert(entity);
            return Unit.Value;
        }
    }

}
