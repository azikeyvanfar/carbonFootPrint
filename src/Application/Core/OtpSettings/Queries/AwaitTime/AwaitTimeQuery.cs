using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Core.OtpSettings.Queries.AwaitTime
{
    public class AwaitTimeQuery : SearchQueryRequest, IRequest<double>
    {
        public OtpType otpType { get; set; } = OtpType.SpecialPage;
        public string personnelCode { get; set; } = "";
        public AwaitTimeQuery(OtpType type)
        {
            otpType = type;
        }
        public AwaitTimeQuery(OtpType type, string PersonnelCode)
        {
            otpType = type;
            personnelCode = PersonnelCode;
        }
    }
    public class AwaitTimeHandler : IRequestHandler<AwaitTimeQuery, double>
    {
        private readonly IRepository<OtpSetting> _otpRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityService _securityService;
        public AwaitTimeHandler(IRepository<OtpSetting> otpRepository, IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
        {
            _otpRepository = otpRepository;
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
        }
        public Task<double> Handle(AwaitTimeQuery request, CancellationToken cancellationToken)
        {
            if (request.otpType == OtpType.CheckForViewForm)
            {
                if (request.personnelCode != "")
                {
                    var query = _otpRepository.GetAllAsNoTracking().Where(c => c.PersonnelCode == request.personnelCode && c.IsActive == false && c.OtpType == request.otpType);
                    if (!query.Any())
                    {
                        return Task.FromResult(0.0);
                    }
                    else
                    {
                        var date = query.Max(c => c.WaitConfirmTime);
                        TimeSpan offset = TimeSpan.FromHours(3.5);
                        var now = DateTimeOffset.Now.ToOffset(offset);
                        var result = date - now;

                        return Task.FromResult(result.TotalMilliseconds);
                    }
                }
                return Task.FromResult(0.0);
            }
            else
            {
                string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();
                    var HashToken = _securityService.GetSha256Hash(token);
                    var query = _otpRepository.GetAllAsNoTracking().Where(c => c.HashToken == HashToken && c.IsActive == false && c.OtpType == request.otpType);
                    if (!query.Any())
                    {
                        return Task.FromResult(0.0);
                    }
                    else
                    {
                        var date = query.Max(c => c.WaitConfirmTime);
                        TimeSpan offset = TimeSpan.FromHours(3.5);
                        var now = DateTimeOffset.Now.ToOffset(offset);
                        var result = date - now;

                        return Task.FromResult(result.TotalMilliseconds);
                    }
                }
                else
                {
                    return Task.FromResult(0.0);
                }
            }
        }
    }
}