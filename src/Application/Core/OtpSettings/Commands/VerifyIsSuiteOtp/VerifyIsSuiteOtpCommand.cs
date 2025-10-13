using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.VerifyIsSuiteOtp
{
    /// <summary>
    /// Verify Is-Suite Otp code 
    /// </summary>
    public class VerifyIsSuiteOtpCommand : IRequest<bool>
    {
        public string OtpValue { get; set; }
        public VerifyIsSuiteOtpCommand(string otp)
        {
            OtpValue = otp;
        }
    }

    public class VerifyIsSuiteOtpCommandValidator : IRequestHandler<VerifyIsSuiteOtpCommand, bool>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IsSuiteClientService _isSuitHttp;
        private readonly IIsSuiteOtpService _isSuiteOtp;

        public VerifyIsSuiteOtpCommandValidator(
            IHttpContextAccessor accessor,
            IsSuiteClientService isSuitHttp,
            IIsSuiteOtpService isSuiteOtp)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _isSuitHttp = isSuitHttp ?? throw new ArgumentNullException(nameof(isSuitHttp));
            _isSuiteOtp = isSuiteOtp ?? throw new ArgumentNullException(nameof(isSuiteOtp));
        }

        public async Task<bool> Handle(VerifyIsSuiteOtpCommand request, CancellationToken cancellationToken)
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);

            _isSuiteOtp.SetCurrentUserOtpCode(request.OtpValue);

            var queryParams = new List<QueryParamModel>()
            {
                new QueryParamModel  { ParameterName  =  "P_NUM_PRSN" , ParameterValue = personnelCode.ToString()},
                new QueryParamModel  { ParameterName  =  "P_COD" , ParameterValue = request.OtpValue.ToString()},
                new QueryParamModel  { ParameterName  =  "P_TYPE" ,ParameterValue = "2"},
            };

            var isResult = await _isSuitHttp.CheckSMSCode(queryParams);

            var isVerified = isResult.lv_res == 5;

            if (!isVerified)
            {
                _isSuiteOtp.RemoveOtpCodeForPersonnel();
            }

            return isVerified;
        }

    }

}
