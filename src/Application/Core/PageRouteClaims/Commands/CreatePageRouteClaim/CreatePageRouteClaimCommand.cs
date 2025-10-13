using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;      
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.PageRouteClaims.Commands.CreatePageRouteClaim
{
    public class CreatePageRouteClaimCommand : IRequest
    {
        public Guid PageRouteId { get; set; }
        public List<Guid> GeneralClaimIdList { get; set; }

    }
    public class CreatePageRouteClaimCommandHandler : IRequestHandler<CreatePageRouteClaimCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<PageRouteClaim> _repository;
        private readonly IRepository<Domain.Entities.Core.GeneralClaims> _claimsRepository;
        private readonly IRepository<Domain.Entities.Core.PageRoute> _pgRepository;
        public CreatePageRouteClaimCommandHandler(IRepository<PageRouteClaim> repository,
            IRepository<Domain.Entities.Core.GeneralClaims> claimRepository,
            IRepository<Domain.Entities.Core.PageRoute> pgRepository,
            IMapper mapper)
        {
            _repository = repository;
            _claimsRepository = claimRepository;
            _mapper = mapper;
            _pgRepository = pgRepository;
        }

        public async Task<Unit> Handle(CreatePageRouteClaimCommand request, CancellationToken cancellationToken)
        {
            var pageRoutClaimList = _repository.GetAll().Include(x => x.RoleClaims).Where(c => c.PageRouteId == request.PageRouteId).ToList();
            var deleteList = new List<PageRouteClaim>();
            var pgRoute = _pgRepository.GetById(request.PageRouteId);

            //var globalLinkes = _claimsRepository.GetAllAsNoTracking().Where(_ => _.RoleType == pgRoute.RoleType && _.IsGlobal == true).Select(_ => _.Id);
            //request.GeneralClaimIdList.AddRange(globalLinkes);

            if (pageRoutClaimList.Any(_ => !request.GeneralClaimIdList.Any(x => x == _.GeneralClaimsId)))
            {
                deleteList = pageRoutClaimList.Where(x => !request.GeneralClaimIdList.Any(c => c == x.GeneralClaimsId)).ToList();

                _repository.DeleteRange(deleteList);
            }
            var remainedList = pageRoutClaimList.Except(deleteList).ToList();
            var insertList = request.GeneralClaimIdList.Where(x => !remainedList.Any(c => c.GeneralClaimsId == x)).ToList();
            foreach (var item in insertList)
            {
                _repository.Insert(new PageRouteClaim { GeneralClaimsId = item, PageRouteId = request.PageRouteId, IsActive = true });
            }


            //todo: delete Claims Cache for current user

            return await Task.FromResult(Unit.Value);

        }
    }
}
