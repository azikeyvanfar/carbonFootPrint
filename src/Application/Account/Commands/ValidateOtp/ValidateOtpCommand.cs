using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using MediatR;

namespace ContractorBackend.Application.Account.Commands.ValidateOtp
{
    public class ValidateOtpCommand : IRequest<bool>
    {
        public string Username { get; set; }
        public string Otp { get; set; }
    }

    public class ValidateOtpCommandHandler : IRequestHandler<ValidateOtpCommand, bool>
    {
        private readonly IApplicationUserManager _userManager;
        private readonly int smsValidToInMinutes = 4;

        public ValidateOtpCommandHandler(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ValidateOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) { return false; }
            if (user.OTP == request.Otp && user.OtpCreationDate?.AddMinutes(smsValidToInMinutes) > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }
    }
}
