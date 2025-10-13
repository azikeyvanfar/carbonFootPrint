using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.QASubjects.Queries.GetAllQASubject
{
    public class GetAllQASubjectQuery : SearchQueryRequest, IRequest<SearchQueryResponse<QASubjectDto>>
    {

    }
    public class GetAllQASubjectQueryHandler : IRequestHandler<GetAllQASubjectQuery, SearchQueryResponse<QASubjectDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<QASubject> _repository;
        private readonly IApplicationDbContext _dbContext;

        public GetAllQASubjectQueryHandler(IRepository<QASubject> repository, IMapper mapper, IApplicationDbContext dbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<SearchQueryResponse<QASubjectDto>> Handle(GetAllQASubjectQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAll()
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<QASubjectDto>(_mapper.ConfigurationProvider);

            QueryablePaging<QASubjectDto> qp = await query.GridifyQueryableAsync<QASubjectDto>(request, null, cancellationToken);
            Paging<QASubjectDto> result = new(qp.Count, qp.Query.ToList());
            foreach (var item in result.Data)
            {
                var lastModifiedByUser = _dbContext.Set<User>().FirstOrDefault(x => x.Id == item.LastModifiedByUserId);
                item.LastModifiedByFullName = (lastModifiedByUser != null) ? lastModifiedByUser.FirstName + lastModifiedByUser.LastName : "";
            }
            return new SearchQueryResponse<QASubjectDto>(request, result);
        }
    }
}
