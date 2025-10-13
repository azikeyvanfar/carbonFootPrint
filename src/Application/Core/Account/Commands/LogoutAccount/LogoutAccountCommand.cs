using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Core.OtpSettings.Commands.UpdateOtpSetting;
using ContractorBackend.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ContractorBackend.Application.Core.Account.Commands.LogoutAccount
{
    public class LogoutAccountCommand : IRequest
    {
        public LogoutAccountCommand(string refreshToken)
        {
            //RefreshToken = refreshToken;
        }

        public string RefreshToken { get; set; }
    }

    public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand>
    {
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IApplicationDbContext _context;
        //private readonly IAntiForgeryCookieService _antiForgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityService _securityService;

        public LogoutAccountCommandHandler(ITokenStoreService tokenStoreService, IApplicationDbContext context,
            //IAntiForgeryCookieService antiForgery,
            IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
        {
            _tokenStoreService = tokenStoreService;
            _context = context;
            //_antiForgery = antiForgery;
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
        }

        public async Task<Unit> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _httpContextAccessor.HttpContext.GetUserId();

            await DisableOtp();

            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
                // Delete the user's tokens from the database (revoke its bearer token)
                await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue.ToString(), token);
                await _context.SaveChangesAsync(cancellationToken);
            }

            //_antiForgery.DeleteAntiForgeryCookies();

            return Unit.Value;
        }
        private async Task DisableOtp()
        {
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                ISender Mediator = _httpContextAccessor.HttpContext.RequestServices.GetService<ISender>()!;
                var otpDispable = _context.OtpSettings.Where(o => o.IsActive && o.HashToken == _securityService.GetSha256Hash(token)).FirstOrDefault();
                if (otpDispable != null)
                {
                    var updateOtp = new UpdateOtpSettingCommand();
                    updateOtp.IsActive = false;
                    updateOtp.Id = otpDispable.Id;
                    updateOtp.ActiveTime = otpDispable.ActiveTime;
                    updateOtp.WaitConfirmTime = otpDispable.WaitConfirmTime;
                    updateOtp.HashToken = otpDispable.HashToken;
                    updateOtp.Otp = otpDispable.Otp;
                    await Mediator.Send(updateOtp);
                }
            }
        }
    }
}
