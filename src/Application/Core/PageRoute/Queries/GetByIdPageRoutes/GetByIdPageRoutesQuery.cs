using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.PageRoute.Queries.GetByIdPageRoutes
{
    public class GetByIdPageRoutesQuery : IRequest<PageRouteDto>
    {
        public Guid Id { get; set; }
    }

    public class GetByIdPageRoutesQueryHandler : IRequestHandler<GetByIdPageRoutesQuery, PageRouteDto>
    {
        private readonly IRepository<Domain.Entities.Core.PageRoute> _repository;
        private readonly IRepository<PageRouteClaim> _pgRouteClaimRepository;
        private readonly IMapper _mapper;

        public GetByIdPageRoutesQueryHandler(IRepository<Domain.Entities.Core.PageRoute> repository, IMapper mapper, IRepository<PageRouteClaim> pgRouteClaimRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _pgRouteClaimRepository = pgRouteClaimRepository;
        }

        public async Task<PageRouteDto> Handle(GetByIdPageRoutesQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAllAsNoTracking()
                .Where(x => x.Id == request.Id)
                .ProjectTo<PageRouteDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
                ;

            if (entity is null)
            {
                throw new NullReferenceException();
            }

            var claims = _pgRouteClaimRepository.GetAllAsNoTracking().Where(_ => _.PageRouteId == entity.Id).Include(_ => _.GeneralClaim);
            foreach (var claim in claims)
            {
                entity.Claims.Add(new SelectModel() { Value = claim.GeneralClaim.Id.ToString(), Text = claim.GeneralClaim.ClaimValue });
            }

            return entity;
        }
    }
}
