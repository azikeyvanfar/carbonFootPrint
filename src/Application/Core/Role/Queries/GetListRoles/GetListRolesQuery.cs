using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Enums.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Role.Queries.GetListRoles
{
    public class GetListRolesQuery : SearchQueryRequest, IRequest<SearchQueryResponse<SelectModel>>
    {
        public RoleType? RoleType { get; set; }

    }

    public class GetAllRolesQueryHandler :
        IRequestHandler<GetListRolesQuery, SearchQueryResponse<SelectModel>>
    {

        private readonly DbSet<Domain.Entities.Identity.Role> _roles;

        public GetAllRolesQueryHandler(
            IApplicationDbContext dbContext)
        {
            _roles = dbContext.Set<Domain.Entities.Identity.Role>();
        }
        public async Task<SearchQueryResponse<SelectModel>> Handle(GetListRolesQuery request, CancellationToken cancellationToken)
        {
            var query = _roles.AsNoTracking();
            if (request.RoleType != null && request.RoleType != 0)
            {
                query = query.Where(q => q.RoleType == request.RoleType);
            }
            var res = query.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            QueryablePaging<SelectModel> qp = await res.GridifyQueryableAsync(request, null, cancellationToken);
            Paging<SelectModel> result = new(qp.Count, qp.Query.ToList());
            return new SearchQueryResponse<SelectModel>(request, result);
        }
    }
}