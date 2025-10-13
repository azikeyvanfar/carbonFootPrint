using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.QuestionAnswers.Commands.UpdateQA
{
    public class UpdateQuestionAnswersCommand : IRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Guid OrgUnitId { get; set; }
        public Guid QaSubjectId { get; set; }
        public long QuestionerId { get; set; }
        public long ResponderId { get; set; }
        // public DateTime? ResponseDate { get; set; }
        public bool IsPrivate { get; set; }

        public bool IsFromEmployee { get; set; }

    }

    public class UpdateQuestionAnswersCommandHandler : IRequestHandler<UpdateQuestionAnswersCommand>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IMapper _mapper;
        public UpdateQuestionAnswersCommandHandler(IHttpContextAccessor accessor,
            IRepository<QuestionAnswer> repository, IMapper mapper)
        {
            _httpContextAccessor = accessor;
            _mapper = mapper;
            _repository = repository;
        }
        public Task<Unit> Handle(UpdateQuestionAnswersCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            entity.ResponseDate = DateTime.Now;
            if (entity is null)
                throw new NullReferenceException();

            if (entity.IsPopular && request.IsFromEmployee)
            {
                throw new BadRequestException();
            }

            _mapper.Map(request, entity);






            // todo:uncomment
            //entity.QuestionerId = _httpContextAccessor.HttpContext.GetUserId();







            _repository.Update(entity);
            return Task.FromResult(Unit.Value);
        }
    }
}
