using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Services;
using ContractorBackend.Persistence.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContractorBackend.Application.ApplicationSettingPage.Queries.GetApplicationActionsClient
{
    /// <summary>
    /// لیست اکشن های سیستم
    /// </summary>
    public class GetApplicationActionsClientQuery : IRequest<List<ActionPathDto>>
    {

    }


    public class CreateApplicationSettingCommandHandler : IRequestHandler<GetApplicationActionsClientQuery, List<ActionPathDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HttpClientMethods _httpClient;
        private readonly IHttpContextAccessor _accessor;


        public CreateApplicationSettingCommandHandler(
            IApplicationDbContext dbContext,
            IMapper mapper,
            IConfiguration configuration,
            HttpClientMethods httpClient,
            IHttpContextAccessor accessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            _httpClient = httpClient;
            _accessor = accessor;
        }
        public async Task<List<ActionPathDto>> Handle(GetApplicationActionsClientQuery request, CancellationToken cancellationToken)
        {
            string token = "";
            string authHeader = _accessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            var url = _configuration["ProjectUrl"].DecryptC() + Utilities.clientSettingGetActionsApiRoute;
            var apiResponse = await _httpClient.Get(url, token);

            JObject masterObject = JObject.Parse(apiResponse);
            var masterObjectData = masterObject["data"];

            var resJson = JsonConvert.SerializeObject(masterObjectData);

            var result = JsonConvert.DeserializeObject<List<ActionPathDto>>(resJson);

            return result;
        }
    }
}