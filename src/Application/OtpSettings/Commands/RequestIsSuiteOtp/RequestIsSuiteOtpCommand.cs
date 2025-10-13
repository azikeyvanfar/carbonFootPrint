using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using MediatR;

namespace ContractorBackend.Application.OtpSettings.Commands.RequestIsSuiteOtp
{
    /// <summary>
    /// request Is-Suite Otp code 
    /// </summary>
    public class RequestIsSuiteOtpCommand : IRequest<bool>
    {
    }

    public class RequestIsSuiteOtpCommandValidator : IRequestHandler<RequestIsSuiteOtpCommand, bool>
    {
        private readonly IIsSuiteOtpService _isSuiteOtp;

        public RequestIsSuiteOtpCommandValidator(IIsSuiteOtpService isSuiteOtp)
        {
            _isSuiteOtp = isSuiteOtp ?? throw new ArgumentNullException(nameof(isSuiteOtp));
        }

        public async Task<bool> Handle(RequestIsSuiteOtpCommand request, CancellationToken cancellationToken)
        {
            var res = await _isSuiteOtp.SendIsSuiteOtpRequestCodeAsync();
            return res;
        }

    }

}
