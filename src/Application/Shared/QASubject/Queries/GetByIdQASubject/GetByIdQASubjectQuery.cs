using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.QASubjects.Queries.GetByIdQASubject
{
    public class GetByIdQASubjectQuery : IRequest<QASubjectDto>
    {
        public Guid Id { get; set; }
        public GetByIdQASubjectQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetByIdQASubjectQueryHandler : IRequestHandler<GetByIdQASubjectQuery, QASubjectDto>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;
        public GetByIdQASubjectQueryHandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<QASubjectDto> Handle(GetByIdQASubjectQuery request, CancellationToken cancellationToken)
        {
            var dto = await _dbContext.QASubjects
                .Where(x => x.Id == request.Id)
                .ProjectTo<QASubjectDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
            {
                throw new NullReferenceException();
            }

            return dto;
        }
    }


}
