using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Enums.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.Core.GeneralClaims.Queries.GetAllGeneralClaims
{
    public class GetAllGeneralClaimsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<SelectModel>>
    {
        public RoleType RoleType { get; set; }
        public bool FilterGlobal { get; set; }
    }

    public class GetAllGeneralClaimsQueryHandler : IRequestHandler<GetAllGeneralClaimsQuery, SearchQueryResponse<SelectModel>>
    {
        private readonly IRepository<Domain.Entities.Core.GeneralClaims> _repository;
        private readonly IMapper _mapper;
        public GetAllGeneralClaimsQueryHandler(IRepository<Domain.Entities.Core.GeneralClaims> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<SearchQueryResponse<SelectModel>> Handle(GetAllGeneralClaimsQuery request,
               CancellationToken cancellationToken)
        {
            var query = _repository.GetAllAsNoTracking()
                .Where(_ => _.IsActive && _.RoleType == request.RoleType)
                ;

            if (request.FilterGlobal == true)
            {
                query = query.Where(x => !x.IsGlobal);
            }

            QueryablePaging<Domain.Entities.Core.GeneralClaims> qp = await query.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<SelectModel> result = new(qp.Count, qp.Query.Select(_ => new SelectModel()
            {
                Text = $"{_.ClaimValue}-{_.ClaimName}",
                Value = _.Id.ToString()
            }));
            return new SearchQueryResponse<SelectModel>(request, result);
        }
    }
}
