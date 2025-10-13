using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Persistence.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContractorBackend.Application.ApplicationSettingPage.Queries.GetApplicationSettings
{

    public class GetApplicationSettingsQuery : IRequest<ApplicationSettingDto>
    {
        public bool IsAdmin { get; set; }

    }


    public class GetApplicationSettingsQueryHandler : IRequestHandler<GetApplicationSettingsQuery, ApplicationSettingDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HttpClientMethods _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public GetApplicationSettingsQueryHandler(
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
        public async Task<ApplicationSettingDto> Handle(GetApplicationSettingsQuery request, CancellationToken cancellationToken)
        {

            if (request.IsAdmin == true)
            {

                var result = await _dbContext.ApplicationSettings
                    .Where(x => x.IsAdmin == request.IsAdmin)
                    .Include(x => x.RateLimitCustomRules)
                    .FirstOrDefaultAsync(cancellationToken);
                if (result == null)
                {
                    return new ApplicationSettingDto
                    {
                        thresholdDays = _configuration.GetValue<int>("thresholdDays"),
                        ActiveOtp = _configuration.GetValue<bool>("ActiveOtp"),
                        ActiveTimeOtp = _configuration.GetValue<int>("ActiveTimeOtp"),
                        WaitConfirmTimeOtp = _configuration.GetValue<int>("WaitConfirmTimeOtp"),
                        TwoStepLogin = _configuration.GetValue<bool>("TwoStepLogin"),
                        DeActiveUserAfterDays = _configuration.GetValue<int>("DeActiveUserAfterDays"),
                        MaxAccessFailedDeActive = _configuration.GetValue<int>("MaxAccessFailedDeActive"),
                        NotAllowedPreviouslyUsedPasswords = _configuration.GetValue<int>("NotAllowedPreviouslyUsedPasswords"),
                        NotAllowedCountHourChangePass = _configuration.GetValue<int>("NotAllowedCountHourChangePass"),
                        ChangePasswordReminderDays = _configuration.GetValue<int>("ChangePasswordReminderDays"),

                        RateLimitTimeWindow = _configuration.GetSection("ApplicationSettings").GetValue<int>("RateLimitTimeWindow"),
                        RateLimitMaxRequests = _configuration.GetSection("ApplicationSettings").GetValue<int>("RateLimitMaxRequests"),
                        RateLimitCustomRules = _configuration.GetSection("ApplicationSettings")?.GetSection("RateLimitCustomRules")?.Get<List<RateLimitCustomRuleDto>>(),

                        IsAdmin = true
                    };
                }
                var resultMapped = _mapper.Map<ApplicationSetting, ApplicationSettingDto>(result);
                return resultMapped;

            }
            else
            {// call client api 
                string token = "";
                string authHeader = _accessor.HttpContext.Request.Headers["Authorization"];
                if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
                var url = _configuration["ProjectUrl"].DecryptC() + Utilities.clientSettingGetApiRoute;
                var res = await _httpClient.Get(url, token);

                JObject masterObject = JObject.Parse(res);
                var masterObjectData = masterObject["data"];

                var resJson = JsonConvert.SerializeObject(masterObjectData);

                var apiResponse = JsonConvert.DeserializeObject<ApplicationSettingDto>(resJson);
                return apiResponse;
            }
        }

    }
}