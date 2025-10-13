using System.Linq;
using ContractorBackend.Application.Common.Dtos;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Persistence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContractorBackend.Application
{
    public static class ApplicationSettingsService
    {

        public static void AddRateLimitOptionsToConfiguration(this IApplicationBuilder app, IConfiguration _configuration, bool isAdmin)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var _context = serviceScope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var setting = _context.ApplicationSettings
                    .Include(x => x.RateLimitCustomRules).FirstOrDefault(x => x.IsAdmin == isAdmin);

                if (setting == null)
                {
                    return;
                }

                _configuration["thresholdDays"] = setting.thresholdDays.ToString();
                _configuration["ActiveOtp"] = setting.ActiveOtp ? "true" : "false";   // only in client
                _configuration["ActiveTimeOtp"] = setting.ActiveTimeOtp.ToString();  // only in client
                _configuration["WaitConfirmTimeOtp"] = setting.WaitConfirmTimeOtp.ToString();  // only in client
                _configuration.GetSection("ApplicationSettings")["TwoStepLogin"] = setting.TwoStepLogin ? "true" : "false";
                _configuration["DeActiveUserAfterDays"] = setting.DeActiveUserAfterDays.ToString();
                _configuration["MaxAccessFailedDeActive"] = setting.MaxAccessFailedDeActive.ToString();
                _configuration["NotAllowedPreviouslyUsedPasswords"] = setting.NotAllowedPreviouslyUsedPasswords.ToString();
                _configuration["NotAllowedCountHourChangePass"] = setting.NotAllowedCountHourChangePass.ToString();
                _configuration["ChangePasswordReminderDays"] = setting.ChangePasswordReminderDays.ToString();

                _configuration.SetDefaultRateLimit(new RateLimitDecorator { MaxRequests = setting.RateLimitMaxRequests, TimeWindow = setting.RateLimitTimeWindow });
                if (setting.RateLimitCustomRules != null && setting.RateLimitCustomRules.Any())
                {
                    _configuration.SetRateLimitCustomRules(setting.RateLimitCustomRules.Select(x => new RateLimitCustomRuleDto
                    {
                        IsHeader = x.IsHeader,
                        Name = x.Name,
                        RateLimitTimeWindow = x.RateLimitTimeWindow,
                        RateLimitMaxRequests = x.RateLimitMaxRequests
                    }).ToList());
                }
            }

        }
    }
}
