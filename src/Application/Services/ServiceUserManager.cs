using System;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Services
{
    public class ServiceUserManager : IServiceUserManager
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IsSuiteClientService _isSuitHttp;
        private readonly IApplicationUserManager _userManager;
        private readonly IConfiguration _configuration;


        public ServiceUserManager(
            IsSuiteClientService isSuitHttp,
            IStringLocalizer<SharedResource> localizer,
            IApplicationUserManager userManager,
            IConfiguration configuration)
        {
            _isSuitHttp = isSuitHttp ?? throw new ArgumentNullException(nameof(isSuitHttp));
            _localizer = localizer;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task UserDeActive()
        {
            var currentDate = DateTime.Now;
            var allUser = _userManager.Users.Where(x => x.IsActive && x.LastLoggedIn != null && x.LastLoggedIn.Value.AddDays(Convert.ToDouble(_configuration.GetSection("DeActiveUserAfterDays").Value)) <= currentDate);
            if (allUser.Any())
            {
                foreach (var user in allUser)
                {
                    user.IsActive = false;
                    await _userManager.ResetAccessFailedCountAsync(user);
                    var massage = _localizer["InActiveUserInSystem"];
                    var identityResult = await _userManager.UpdateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        //await _smsSender.SendSmsAsync(user,
                        //        new SmsRequest
                        //        {
                        //            Receivers = user.PhoneNumber,
                        //            SmsText = massage,
                        //            IsPassword = 1
                        //        }, SmsType.ChangePassword);                        
                    }
                }
            }
        }
    }
}