using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.PageRouteClaims.Commands.UpdatePageRouteClaim
{
    public class UpdatePageRouteClaimCommand : IRequest
    {
        public Guid Id { get; set; }
        //public string ClaimValue { get; set; }

        public Guid PageRouteId { get; set; }
        //public string ClaimName { get; set; }

        public Guid ClaimId { get; set; }
    }
    public class UpdateClaimMenuItemCommandHandler : IRequestHandler<UpdatePageRouteClaimCommand>
    {
        private readonly IRepository<PageRouteClaim> _repository;
        private readonly IMapper _mapper;
        public UpdateClaimMenuItemCommandHandler(IRepository<PageRouteClaim> repository, IMapper mapper)
        {

            _repository = repository;
            _mapper = mapper;
        }
        public Task<Unit> Handle(UpdatePageRouteClaimCommand request, CancellationToken cancellationToken)
        {

            var entity = _repository.GetById(request.Id);
            if (entity is null)
                throw new NullReferenceException();
            _mapper.Map(request, entity);
            _repository.Update(entity);

            return Task.FromResult(Unit.Value);

        }
    }
}
