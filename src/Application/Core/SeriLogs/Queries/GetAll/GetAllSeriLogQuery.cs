using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Entities.Identity;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.SeriLogs.Queries.GetAll
{

    public class GetAllSeriLogQuery : SearchQueryRequest, IRequest<SearchQueryResponse<AppLogEvent>>
    {
    }
    public class GetAllSeriLogQueryHandler : IRequestHandler<GetAllSeriLogQuery, SearchQueryResponse<AppLogEvent>>
    {
        private readonly ILogDbContext _logRepository;
        public GetAllSeriLogQueryHandler(ILogDbContext logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<SearchQueryResponse<AppLogEvent>> Handle(GetAllSeriLogQuery request, CancellationToken cancellationToken)
        {
            var query = _logRepository.AppLogEvents.AsQueryable().OrderByDescending(_ => _.TimeStamp);
            QueryablePaging<AppLogEvent> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<AppLogEvent> pq = new(qp.Count, qp.Query);
            return new SearchQueryResponse<AppLogEvent>(request, pq);

        }
    }
}
