using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.QuestionAnswers.Queries.GetAllQA
{
    public class GetAllQuestionAnswersQuery :
         SearchQueryRequest, IRequest<SearchQueryResponse<QuestionAnswersDto>>
    {
    }

    public class GetAllQuestionAnswersQueryHandler : IRequestHandler<GetAllQuestionAnswersQuery,
                  SearchQueryResponse<QuestionAnswersDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<QuestionAnswer> _repository;
        private readonly IApplicationDbContext _dbContext;
        public GetAllQuestionAnswersQueryHandler(IMapper mapper, IRepository<QuestionAnswer> repository, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<SearchQueryResponse<QuestionAnswersDto>> Handle(GetAllQuestionAnswersQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<QuestionAnswersDto>(_mapper.ConfigurationProvider);

            QueryablePaging<QuestionAnswersDto> qp = await query.GridifyQueryableAsync<QuestionAnswersDto>(request, null, cancellationToken);
            Paging<QuestionAnswersDto> result = new(qp.Count, qp.Query.ToList());
            foreach (var item in result.Data)
            {
                var lastModifiedByUser = _dbContext.Set<User>().FirstOrDefault(x => x.Id == item.LastModifiedByUserId);
                item.LastModifiedByFullName = (lastModifiedByUser != null) ? lastModifiedByUser.FirstName + " " + lastModifiedByUser.LastName : "";
            }
            return new SearchQueryResponse<QuestionAnswersDto>(request, result);
        }
    }
}
