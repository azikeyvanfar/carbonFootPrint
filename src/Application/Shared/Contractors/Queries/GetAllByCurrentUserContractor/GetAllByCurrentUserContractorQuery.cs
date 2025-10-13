using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.IsSuiteEntities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Contractors.Queries.GetAllContractor
{
    public class GetAllByCurrentUserContractorQuery : SearchQueryRequest, IRequest<SearchQueryResponse<Contractor>>
    {
    }

    public class GetAllByCurrentUserContractorQueryHandler : IRequestHandler<GetAllByCurrentUserContractorQuery, SearchQueryResponse<Contractor>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Contractor> _repository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationUserManager _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GetAllByCurrentUserContractorQueryHandler(
            IMapper mapper,
            IRepository<Contractor> repository,
            IApplicationDbContext dbContext,
            IHttpContextAccessor accessor,
            IApplicationUserManager userManager,
            IStringLocalizer<SharedResource> localizer)
        {
            _mapper = mapper;
            _repository = repository;
            _dbContext = dbContext;
            _accessor = accessor;
            _userManager = userManager;
            _localizer = localizer;
        }
        public async Task<SearchQueryResponse<Contractor>> Handle(GetAllByCurrentUserContractorQuery request, CancellationToken cancellationToken)
        {
            #region get all based on current User Contract
            var currentUser = _userManager.GetCurrentUser();
            ArgumentNullException.ThrowIfNull(currentUser);
            //var ContractorCode = currentUser.Employee.ContractorCode;

            var query1 = _repository.GetAllAsNoTracking();

            //if (!string.IsNullOrWhiteSpace(ContractorCode) && !ContractorCode.Equals("0"))
            //{ //پیمانکاران با قرارداد واقعی
            //    query1 = query1.Where(x => x.ContractorCode == ContractorCode);
            //}
            //else if (!string.IsNullOrWhiteSpace(ContractorCode) && ContractorCode.Equals("0"))
            //{ //پیمانکار با قراداد مجازی - ارور می دهد
            //    throw new CustomException(_localizer["NoAccess"]);
            //}
            //else if (string.IsNullOrWhiteSpace(ContractorCode))
            //{ // پرسنل فولاد است - همه قرارداد ها را میبیند
            //}
            #endregion

            //var query = query1.ProjectTo<ContractorDto>(_mapper.ConfigurationProvider);

            //QueryablePaging<ContractorDto> qp = await query.GridifyQueryableAsync<ContractorDto>(request, null, cancellationToken);
            //Paging<ContractorDto> result = new(qp.Count, qp.Query.ToList());

            //return new SearchQueryResponse<ContractorDto>(request, result);
            return null;
        }
    }
}
