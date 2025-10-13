using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Account.Commands.LoginAccount
{
    public class LoginDevelopCommand : IRequest<TokenInfo>
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class LoginDevelopCommandHandler : IRequestHandler<LoginDevelopCommand, TokenInfo>
    {
        private readonly IApplicationDbContext _context;
        private readonly IApplicationUserManager _userManager;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly IUserService _userService;

        public LoginDevelopCommandHandler(
            IApplicationDbContext context, ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService, IApplicationUserManager userManager,
            IUserService userService

            )
        {
            _context = context;
            _userManager = userManager;
            _tokenStoreService = tokenStoreService;
            _tokenFactoryService = tokenFactoryService;
            _userService = userService;
        }

        public async Task<TokenInfo> Handle(LoginDevelopCommand request, CancellationToken cancellationToken)
        {
            try
            {
                User user;
                user = await _userManager.Users
                   .FirstOrDefaultAsync(_ => _.UserName == request.Username && _.IsActive, cancellationToken);

                if (user is null)
                {
                    throw new CustomException("نام کاربری و یا کلمه‌ی عبور وارد شده معتبر نیستند.");
                }

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    throw new CustomException("نام کاربری و یا کلمه‌ی عبور وارد شده معتبر نیستند.");
                }

                var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
                await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
                await _context.SaveChangesAsync(cancellationToken);

                //await _userService.UpdateUserFromIsSuiteAfterThresholdTime(user.Id); 

                await _userManager.UpdateUserLastActivityDateAsync(user.Id);

                var returnObject = new TokenInfo
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,

                };

                return returnObject;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message + " inner " + e.InnerException?.Message ?? "");
            }
        }
    }
}
