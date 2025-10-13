using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Login;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Persistence.Services.Login
{
    public class UserloginService : IUserloginService
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IConfiguration _configuration;
        private readonly ILogDbContext logDbContext;
        private readonly IHttpContextAccessor _contextAccessor;


        public UserloginService(
            IApplicationUserManager userManager,
            IStringLocalizer<SharedResource> localizer,
            IConfiguration configuration,
            ILogDbContext logDbContext,
            IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _localizer = localizer;
            _configuration = configuration;
            this.logDbContext = logDbContext;
            _contextAccessor = contextAccessor;
        }
        public async Task<User> CheckUserExistsAndHasRoleAndReturnUser(string username, string password, RoleType roleType, CancellationToken cancellationToken)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(_ => _.UserName == username && _.IsActive, cancellationToken);

            if (user is null)
            {
                throw new CustomException(_localizer["DeActiveUser"]);
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if ((await _userManager.GetLockoutEnabledAsync(user)) && (await _userManager.GetLockoutEndDateAsync(user) > DateTimeOffset.UtcNow))
                {
                    throw new CustomException(_localizer["UserlockedOutError"]);
                }
                await _userManager.AccessFailedAsync(user);
                var failedDeactive = await _userManager.AccessFailedDeActiveAsync(user, false);
                if (failedDeactive >= Convert.ToInt32(_configuration.GetSection("MaxAccessFailedDeActive").Value))
                {
                    user.IsActive = false;
                }
                await _userManager.UpdateAsync(user);

                #region Log SignIn to LogContext
                logDbContext.UserSignIns.Add(new UserSignIn
                {
                    IsLogin = false,
                    TimeStamp = DateTimeOffset.Now,
                    UserId = user.Id,
                    PersonnelCode = user.PersonnelCode,
                    CreatedByIP = _contextAccessor.HttpContext.Connection.RemoteIpAddress + ":" + _contextAccessor.HttpContext.Connection.LocalPort,
                    CreatedByBrowserName = _contextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    CreatedByBrowserNameSpecific = _contextAccessor.HttpContext?.Request?.Headers["sec-ch-ua"].ToString(),
                    ErrorReason = _localizer["WrongUsernamePassword"],
                    SystemName = _configuration.GetSection("SystemName").Value
                });
                logDbContext.SaveChanges();
                #endregion

                throw new CustomException(_localizer["WrongUsernamePassword"]);
            }

            var user_Roles = await _userManager.GetRolesAsync(user, roleType);
            if (user_Roles.Count == 0)
            {

                #region Log SignIn to LogContext
                logDbContext.UserSignIns.Add(new UserSignIn
                {
                    IsLogin = false,
                    TimeStamp = DateTimeOffset.Now,
                    UserId = user.Id,
                    PersonnelCode = user.PersonnelCode,
                    CreatedByIP = _contextAccessor.HttpContext.Connection.RemoteIpAddress + ":" + _contextAccessor.HttpContext.Connection.LocalPort,
                    CreatedByBrowserName = _contextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    CreatedByBrowserNameSpecific = _contextAccessor.HttpContext?.Request?.Headers["sec-ch-ua"].ToString(),
                    ErrorReason = _localizer["UserHasNoRoleError"],
                    SystemName = _configuration.GetSection("SystemName").Value

                });
                logDbContext.SaveChanges();
                #endregion

                throw new CustomException(_localizer["UserHasNoRoleError"]);
            }
            return user;

        }
    }
}
