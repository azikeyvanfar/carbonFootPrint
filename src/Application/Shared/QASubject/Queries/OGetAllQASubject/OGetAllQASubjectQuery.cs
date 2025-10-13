using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Shared;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.QASubjects.Queries.OGetAllQASubject
{
    public class OGetAllQASubjectQuery : SearchQueryRequest, IRequest<SearchQueryResponse<SelectModel>>
    {
        public bool IsAdmin { get; set; } = false;
    }

    public class OGetAllQASubjectQueryHandler : IRequestHandler<OGetAllQASubjectQuery, SearchQueryResponse<SelectModel>>
    {
        private readonly IRepository<QASubject> _repository;
        private readonly IMapper _mapper;
        public OGetAllQASubjectQueryHandler(IRepository<QASubject> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SearchQueryResponse<SelectModel>> Handle(OGetAllQASubjectQuery request, CancellationToken cancellationToken)
        {
            var queryRes = _repository.GetAllAsNoTracking();
            if (!request.IsAdmin)
            {
                queryRes = queryRes.Where(_ => _.IsActive);
            }
            var query = queryRes
                .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
                .ProjectTo<QASubjectDto>(_mapper.ConfigurationProvider);

            QueryablePaging<QASubjectDto> qp = await query.GridifyQueryableAsync<QASubjectDto>(request, null, cancellationToken);
            Paging<SelectModel> result = new(qp.Count, qp.Query.Select(_ => new SelectModel()
            {
                Text = _.SubjectName,
                Value = _.Id.ToString()
            }));
            return new SearchQueryResponse<SelectModel>(request, result);
        }
    }
}
