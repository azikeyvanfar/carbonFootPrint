using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.QuestionAnswers.Queries.OGetAllQA
{
    /// <summary>
    /// پرسش های من
    /// </summary>
    public class OGetAllQuestionAnswersQuery : SearchQueryRequest, IRequest<SearchQueryResponse<QuestionAnswersDto>>
    {
        public long? Id { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class OGetAllQuestionAnswersQueryHandler :
        IRequestHandler<OGetAllQuestionAnswersQuery, SearchQueryResponse<QuestionAnswersDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IHttpContextAccessor _accessor;
        public OGetAllQuestionAnswersQueryHandler(IMapper mapper, IRepository<QuestionAnswer> repository,
            IHttpContextAccessor accessor)
        {
            _mapper = mapper;
            _repository = repository;
            _accessor = accessor;
        }
        public async Task<SearchQueryResponse<QuestionAnswersDto>> Handle(OGetAllQuestionAnswersQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking();

            if (!request.IsAdmin)
            {
                query = query.Where(x => x.IsActive);
            }

            if (request.Id.HasValue)
            {
                query = query.Where(_ => _.QuestionerId == request.Id.Value);
            }

            var queryDto = query.ProjectTo<QuestionAnswersDto>(_mapper.ConfigurationProvider);

            QueryablePaging<QuestionAnswersDto> qp = await queryDto.GridifyQueryableAsync<QuestionAnswersDto>(request, null, cancellationToken);
            Paging<QuestionAnswersDto> result = new(qp.Count, qp.Query);

            return new SearchQueryResponse<QuestionAnswersDto>(request, result);
        }
    }
}
