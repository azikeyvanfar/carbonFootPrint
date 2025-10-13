using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.ChangeActiveUser
{
    public class ChangeActiveUserCommand : IRequest<bool>
    {

        [Required]
        public long UserId { get; set; }
        public bool? IsActive { get; set; } = true;
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }

    public class ChangeActiveUserCommandHandler : IRequestHandler<ChangeActiveUserCommand, bool>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ISmsSender _smsSender;
        private readonly IApplicationUserManager _userManager;
        public ChangeActiveUserCommandHandler(IApplicationUserManager userManager, IStringLocalizer<SharedResource> localizer,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _localizer = localizer;
            _smsSender = smsSender;
        }
        public async Task<bool> Handle(ChangeActiveUserCommand request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }

            if (request.IsActive != null)
            {
                user.IsActive = request.IsActive.Value;
                user.LastLoggedIn = null;
                if (user.IsActive)
                {
                    user.AccessFailedDeActive = 0;
                }
                await _userManager.UpdateAsync(user);
                await _userManager.ResetAccessFailedCountAsync(user);

            }

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                await ChangePasswordForTargetUser(user, request, cancellationToken);
            }

            return true;
        }




        public async Task<bool> ChangePasswordForTargetUser(User user, ChangeActiveUserCommand request, CancellationToken cancellationToken)
        {
            if (user is null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }

            if (!request.NewPassword.Equals(request.NewPasswordConfirm))
            {
                throw new CustomException(_localizer["PasswordNotConfirmRepeated"]);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var changePass = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (changePass.Succeeded)
            {
                user.IsPasswordChangeForce = false;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                var msgError = string.Join(" \n", changePass.Errors.Select(x => x.Description).ToArray());
                throw new CustomException(msgError);
            }

            return true;
        }

    }
}
