using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using Gridify;
using Gridify.EntityFramework;
using MediatR;

namespace ContractorBackend.Application.OtpSettings.Queries.GetAll
{
    public class GetAllOtpSettingsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<OtpSettingDto>>
    {
    }

    public class GetAllOtpSettingsQueryHandler : IRequestHandler<GetAllOtpSettingsQuery, SearchQueryResponse<OtpSettingDto>>
    {
        private readonly IRepository<OtpSetting> _repository;
        private readonly IMapper _mapper;

        public GetAllOtpSettingsQueryHandler(IRepository<OtpSetting> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchQueryResponse<OtpSettingDto>> Handle(GetAllOtpSettingsQuery request, CancellationToken cancellationToken)
        {

            var query = _repository.GetAllAsNoTracking().Where(t => t.IsActive == true);

            QueryablePaging<OtpSetting> qp1 = await
              query.GridifyQueryableAsync<OtpSetting>(request, null, cancellationToken);
            var qp = _mapper.Map<List<OtpSettingDto>>(qp1.Query);

            Paging<OtpSettingDto> result = new(qp1.Count, qp);
            return new SearchQueryResponse<OtpSettingDto>(request, result);
        }
    }
}
