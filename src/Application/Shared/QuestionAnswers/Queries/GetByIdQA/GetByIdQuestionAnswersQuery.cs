
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.QuestionAnswers.Queries.GetByIdQA
{
    public class GetByIdQuestionAnswersQuery : IRequest<QuestionAnswersDto>
    {
        public Guid Id { get; set; }
        public GetByIdQuestionAnswersQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetByIdQuestionAnswersQueryHandler : IRequestHandler<GetByIdQuestionAnswersQuery, QuestionAnswersDto>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;
        public GetByIdQuestionAnswersQueryHandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<QuestionAnswersDto> Handle(GetByIdQuestionAnswersQuery request, CancellationToken cancellationToken)
        {
            var dto = await _dbContext.QuestionAnswers
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .ProjectTo<QuestionAnswersDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (dto is null)
            {
                throw new NullReferenceException();
            }

            return dto;
        }
    }
}
