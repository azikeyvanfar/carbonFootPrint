using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.CompareOtp
{
    public class OtpDto
    {
        public string OtpValue { get; set; }
        public OtpType otpType { get; set; } = OtpType.SpecialPage;
        public string PersonnelCode { get; set; }
        public Guid? FormId { get; set; }
    }
    public class CompareOtpCommand : SearchQueryRequest, IRequest<bool>
    {
        public string Otp { get; set; }
        public OtpType otpType { get; set; }
        public string PersonnelCode { get; set; }
        public Guid? FormId { get; set; }
        public CompareOtpCommand(OtpDto otpDto)
        {
            Otp = otpDto.OtpValue;
            otpType = otpDto.otpType;
            PersonnelCode = otpDto.PersonnelCode;
            FormId = otpDto.FormId;
        }
    }
    public class CompareOtpHandler : IRequestHandler<CompareOtpCommand, bool>
    {
        private readonly IRepository<OtpSetting> _otpRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityService _securityService;
        public CompareOtpHandler(IRepository<OtpSetting> otpRepository, IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
        {
            _otpRepository = otpRepository;
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
        }
        public Task<bool> Handle(CompareOtpCommand request, CancellationToken cancellationToken)
        {
            bool result = false;
            if (request.otpType == OtpType.CheckForViewForm)
            {
                if (request.PersonnelCode == "")
                {
                    throw new CustomException("کد پرسنلی را وارد نمایید");
                }
                TimeSpan offset = TimeSpan.FromHours(3.5);
                var now = DateTimeOffset.Now.ToOffset(offset);
                var o = _otpRepository.GetAllAsNoTracking().Where(c => c.PersonnelCode == request.PersonnelCode && c.Otp == request.Otp && c.OtpType == request.otpType);
                if (o.FirstOrDefault() is not null)
                {
                    var otp = o.Where(c => c.WaitConfirmTime >= now).FirstOrDefault();
                    if (otp is not null)
                    {
                        if (o.Where(c => c.IsActive == false && c.WaitConfirmTime >= now).FirstOrDefault() == null)
                        {
                            throw new CustomException("کد قبلا فعال شده!");
                        }
                        else
                        {
                            otp.IsActive = true;
                            result = _otpRepository.UpdateEntity(otp);
                        }
                    }
                    else
                    {
                        throw new CustomException("زمان انتظار به اتمام رسیده است!");
                    }
                }
                else
                {
                    throw new CustomException("مقدار وارد شده نادرست می باشد!");
                }
            }
            else
            {
                string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();
                    var HashToken = _securityService.GetSha256Hash(token);
                    TimeSpan offset = TimeSpan.FromHours(3.5);
                    var now = DateTimeOffset.Now.ToOffset(offset);
                    var o = _otpRepository.GetAllAsNoTracking().Where(c => c.HashToken == HashToken && c.Otp == request.Otp && c.OtpType == request.otpType);
                    if (o.FirstOrDefault() is not null)
                    {
                        var otp = o.Where(c => c.WaitConfirmTime >= now).FirstOrDefault();
                        if (otp is not null)
                        {
                            if (o.Where(c => c.IsActive == false && c.WaitConfirmTime >= now).FirstOrDefault() == null)
                            {
                                throw new CustomException("کد قبلا فعال شده!");
                            }
                            else
                            {
                                otp.IsActive = true;
                                result = _otpRepository.UpdateEntity(otp);
                            }
                        }
                        else
                        {
                            throw new CustomException("زمان انتظار به اتمام رسیده است!");
                        }
                    }
                    else
                    {
                        throw new CustomException("مقدار وارد شده نادرست می باشد!");
                    }
                }
            }
            return Task.FromResult(result);
        }
    }
}