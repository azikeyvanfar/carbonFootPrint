using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.ApplicationSettingPage.Queries.GetApplicationSettingsClient
{

    public class GetApplicationSettingsClientQuery : IRequest<ApplicationSettingDto>
    {

    }


    public class GetApplicationSettingsClientQueryHandler : IRequestHandler<GetApplicationSettingsClientQuery, ApplicationSettingDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HttpClientMethods _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public GetApplicationSettingsClientQueryHandler(
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
        public async Task<ApplicationSettingDto> Handle(GetApplicationSettingsClientQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ApplicationSettings
                    .Where(x => x.IsAdmin == false)
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

                    IsAdmin = false
                };
            }
            var resultMapped = _mapper.Map<ApplicationSetting, ApplicationSettingDto>(result);
            return resultMapped;
        }

    }
}