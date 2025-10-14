using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Entities.Log;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.ErrorHistorys.Queries.GetAll
{
    public class GetAllErrorHistoryQuery : SearchQueryRequest, IRequest<SearchQueryResponse<ErrorHistory>>
    {
    }
    public class GetAllErrorHistoryQueryHandler : IRequestHandler<GetAllErrorHistoryQuery, SearchQueryResponse<ErrorHistory>>
    {
        private readonly ILogDbContext _errorHistoryRepository;
        public GetAllErrorHistoryQueryHandler(ILogDbContext errorHistoryRepository)
        {
            _errorHistoryRepository = errorHistoryRepository;
        }

        public async Task<SearchQueryResponse<ErrorHistory>> Handle(GetAllErrorHistoryQuery request, CancellationToken cancellationToken)
        {
            var query = _errorHistoryRepository.ErrorHistories.AsQueryable().OrderByDescending(_ => _.Id);
            QueryablePaging<ErrorHistory> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<ErrorHistory> pq = new(qp.Count, qp.Query);
            return new SearchQueryResponse<ErrorHistory>(request, pq);

        }
    }
}
