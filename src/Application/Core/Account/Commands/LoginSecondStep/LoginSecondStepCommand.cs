using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Login;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Core.Account.Commands.LoginAccount;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ContractorBackend.Application.Core.Account.Commands.LoginSecondStep
{
    public class LoginSecondStepCommand : IRequest<Token>
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        /// <summary>
        /// otp code
        /// </summary>
        public string Otp { get; set; } = null!;

        public RoleType RoleType { get; set; }
    }
    public class LoginSecondStepCommandHandler : IRequestHandler<LoginSecondStepCommand, Token>

    {
        private readonly IApplicationDbContext _context;
        private readonly IApplicationUserManager _userManager;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUserService _userService;
        private readonly IUserloginService _userloginService;
        private readonly ILogger<LoginAcountCommandHandler> _logger;

        public LoginSecondStepCommandHandler(
            IApplicationDbContext context, ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService, IApplicationUserManager userManager,
            IApplicationSignInManager signInManager,
            IUserService userService,
            IStringLocalizer<SharedResource> localizer,
            IUserloginService userloginService,
            ILogger<LoginAcountCommandHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _tokenStoreService = tokenStoreService;
            _tokenFactoryService = tokenFactoryService;
            _signInManager = signInManager;
            _localizer = localizer;
            _userService = userService;
            _userloginService = userloginService;
            _logger = logger;
        }

        public async Task<Token> Handle(LoginSecondStepCommand request, CancellationToken cancellationToken)
        {
            //captcha has been already checked in caller action

            User user = await _userloginService.CheckUserExistsAndHasRoleAndReturnUser(request.Username, request.Password, request.RoleType, cancellationToken);

            var signInResponse = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (!signInResponse.Succeeded)
            {
                if (signInResponse.IsLockedOut)
                {
                    throw new CustomException(_localizer["UserlockedOutError"]);
                }
                throw new CustomException(_localizer["SignInFailedError"]);
            }

            _logger.LogWarning($"user : {request.Username} with Role {request.RoleType} is login");

            await _signInManager.SignInAsync(user, false);

            var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
            await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
            await _context.SaveChangesAsync(cancellationToken);

            //await _userService.UpdateUserFromIsSuiteAfterThresholdTime(user.Id); 

            await _userManager.UpdateUserLastActivityDateAsync(user.Id);

            var returnObject = new Token
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,

            };

            return returnObject;


        }
    }
}
