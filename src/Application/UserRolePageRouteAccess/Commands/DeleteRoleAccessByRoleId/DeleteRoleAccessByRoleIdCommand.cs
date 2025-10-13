using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;

namespace ContractorBackend.Application.UserRolePageRouteAccesses.Commands.DeleteRoleAccessByRoleId
{
    public class DeleteRoleAccessByRoleIdCommand : IRequest
    {
        public long RoleId { get; set; }

        public DeleteRoleAccessByRoleIdCommand(long id)
        {
            RoleId = id;
        }
    }

    public class DeleteRoleAccessByRoleIdQueryHandler : IRequestHandler<DeleteRoleAccessByRoleIdCommand>
    {
        private readonly IRepository<RoleClaim> _claimRepository;
        private readonly IRepository<RolePageRouteAccess> _rolePageRouteRepo;
        private readonly IRepository<PageRouteClaim> _pageRouteClaimRepository;
        public DeleteRoleAccessByRoleIdQueryHandler(IRepository<RoleClaim> claimRepo,
            IRepository<RolePageRouteAccess> rolePageRouteRepo,
            IRepository<PageRouteClaim> pageRouteRepo)
        {
            _claimRepository = claimRepo;
            _rolePageRouteRepo = rolePageRouteRepo;
            _pageRouteClaimRepository = pageRouteRepo;
        }

        public async Task<Unit> Handle(DeleteRoleAccessByRoleIdCommand request, CancellationToken cancellationToken)
        {
            //TODO implement transaction

            List<Guid> pageRouteClaimIds = new List<Guid>();
            List<Guid> pageRouteIds = new List<Guid>();

            var pageRouteAccesses = _rolePageRouteRepo.GetAll().Where(_ => _.RoleId == request.RoleId).ToList();
            pageRouteIds = pageRouteAccesses.Select(_ => _.PageRouteId).ToList();
            _rolePageRouteRepo.DeleteRange(pageRouteAccesses);


            pageRouteClaimIds = _pageRouteClaimRepository.GetAll().Where(_ => pageRouteIds.Contains(_.PageRouteId)).Select(a => a.Id).ToList();


            var roleClaims = _claimRepository.GetAll().Where(_ => _.RoleId == request.RoleId && pageRouteClaimIds.Contains(_.PageRouteClaimId.Value)).ToList();
            _claimRepository.DeleteRange(roleClaims);

            return await Task.FromResult(Unit.Value);
        }
    }
}
