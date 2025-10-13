using System;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContractorBackend.Persistence.Services.Identity
{
    public class ApplicationSignInManager :
        SignInManager<User>,
        IApplicationSignInManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ApplicationSignInManager> _logger;
        private readonly ILogDbContext logDbContext;
        private readonly IConfiguration _configuration;

        public ApplicationSignInManager(
            IApplicationUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<ApplicationSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> confirmation,
            ILogDbContext logDbContext,
            IConfiguration configuration)
            : base((UserManager<User>)userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.logDbContext = logDbContext;
            _configuration = configuration;
        }

        #region BaseClass

        Task<bool> IApplicationSignInManager.IsLockedOut(User user)
        {
            return base.IsLockedOut(user);
        }

        Task<SignInResult> IApplicationSignInManager.LockedOut(User user)
        {
            return base.LockedOut(user);
        }

        Task<SignInResult> IApplicationSignInManager.PreSignInCheck(User user)
        {
            return base.PreSignInCheck(user);
        }

        Task IApplicationSignInManager.ResetLockout(User user)
        {
            return base.ResetLockout(user);
        }

        Task<SignInResult> IApplicationSignInManager.SignInOrTwoFactorAsync(User user, bool isPersistent, string loginProvider, bool bypassTwoFactor)
        {
            return base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

        #endregion

        #region CustomMethods

        public override Task SignInAsync(User user, bool isPersistent, string authenticationMethod = null)
        {
            #region Log SignIn to LogContext
            logDbContext.UserSignIns.Add(new UserSignIn
            {
                IsLogin = true,
                TimeStamp = DateTimeOffset.Now,
                UserId = user.Id,
                PersonnelCode = user.PersonnelCode,
                CreatedByIP = _contextAccessor.HttpContext.Connection.RemoteIpAddress + ":" + _contextAccessor.HttpContext.Connection.LocalPort,
                CreatedByBrowserName = _contextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                CreatedByBrowserNameSpecific = _contextAccessor.HttpContext?.Request?.Headers["sec-ch-ua"].ToString(),
                SystemName = _configuration.GetSection("SystemName").Value
            });
            logDbContext.SaveChanges();
            #endregion

            _logger.LogWarning($"User {user.UserName} Signed In with isPersistent:{isPersistent} and authenticationMethod:{authenticationMethod}");
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            var resTask = base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

            if (!resTask.Result.Succeeded)
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
                    SystemName = _configuration.GetSection("SystemName").Value
                });
                logDbContext.SaveChanges();
                #endregion

                _logger.LogWarning($"User {user.UserName} Signed In Failed. ");
            }

            return resTask;
        }

        public bool IsCurrentUserSignedIn()
        {
            return IsSignedIn(_contextAccessor.HttpContext.User);
        }

        public Task<User> ValidateCurrentUserSecurityStampAsync()
        {
            return ValidateSecurityStampAsync(_contextAccessor.HttpContext.User);
        }

        #endregion
    }
}
