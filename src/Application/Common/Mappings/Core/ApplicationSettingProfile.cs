using AutoMapper;
using ContractorBackend.Application.Core.ApplicationSettingPage.Commands.CreateApplicationSettings;
using ContractorBackend.Application.Core.ApplicationSettingPage.Commands.CreateApplicationSettingsClient;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Domain.Entities.Core;

namespace ContractorBackend.Application.Common.Mappings.Core
{
    public class ApplicationSettingProfile : Profile
    {
        public ApplicationSettingProfile()
        {

            CreateMap<CreateApplicationSettingsCommand, ApplicationSetting>()
                .ForMember(s => s.RateLimitCustomRules, m => m.MapFrom(_ => _.RateLimitCustomRules))
                ;

            CreateMap<CreateApplicationSettingsClientCommand, ApplicationSetting>()
                .ForMember(s => s.RateLimitCustomRules, m => m.MapFrom(_ => _.RateLimitCustomRules))
                ;

            CreateMap<ApplicationSetting, ApplicationSettingDto>()
                .ForMember(s => s.RateLimitCustomRules, m => m.MapFrom(_ => _.RateLimitCustomRules))
                ;

            CreateMap<ApplicationSettingDto, ApplicationSetting>()
                .ForMember(s => s.RateLimitCustomRules, m => m.MapFrom(_ => _.RateLimitCustomRules))
                ;

            CreateMap<RateLimitCustomRule, RateLimitCustomRuleDto>();

            CreateMap<RateLimitCustomRuleDto, RateLimitCustomRule>();
        }
    }
}