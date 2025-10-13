using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces.Login;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Account.Commands.LoginFirstStep
{
    public class LoginFirstStepCommand : IRequest<Token>
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        /// <summary>
        /// result of sum of image
        /// </summary>
        public string Captcha { get; set; } = null!;

        public RoleType RoleType { get; set; }


    }

    public class LoginFirstStepCommandHandler : IRequestHandler<LoginFirstStepCommand, Token>
    {
        private readonly IApplicationSignInManager _signInManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUserloginService _userloginService;
        private readonly IApplicationUserManager _userManager;


        public LoginFirstStepCommandHandler(IApplicationSignInManager signInManager,
            IStringLocalizer<SharedResource> localizer,
            IUserloginService userloginService,
            IApplicationUserManager userManager)
        {
            _signInManager = signInManager;
            _localizer = localizer;
            _userloginService = userloginService;
            _userManager = userManager;
        }

        public async Task<Token> Handle(LoginFirstStepCommand request, CancellationToken cancellationToken)
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
            else
            {
                await _userManager.AccessFailedDeActiveAsync(user, true);
            }


            return new Token
            {
                AccessToken = "",
                RefreshToken = ""
            };


        }
    }

}

