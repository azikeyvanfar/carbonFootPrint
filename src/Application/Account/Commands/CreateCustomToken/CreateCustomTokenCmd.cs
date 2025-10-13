using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Common.Models.SiteSettings;
using ContractorBackend.Persistence.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ContractorBackend.Application.Account.Commands.CreateCustomToken
{
    /// <summary>
    /// صدور توکن برای افرادی که لاگین نکرده اند  و میخواهند فرمی را پر کنند
    /// </summary>
    public class CreateCustomTokenCmd : IRequest<string>
    {
        public Guid formId { get; set; }
        public string personnelCode { get; set; }

        public CreateCustomTokenCmd(string personnelCode, Guid formId)
        {
            this.personnelCode = personnelCode;
            this.formId = formId;
        }
    }

    public class CreateCustomTokenCmdHandler : IRequestHandler<CreateCustomTokenCmd, string>
    {
        private readonly IOptionsSnapshot<BearerTokensSettings> _configuration;

        public CreateCustomTokenCmdHandler(IOptionsSnapshot<BearerTokensSettings> configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<string> Handle(CreateCustomTokenCmd request, CancellationToken cancellationToken)
        {
            var Claims = new List<Claim>
            {
                new Claim("personnelCode",request.personnelCode),
                new Claim("formId",request.formId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                            issuer: _configuration.Value.Issuer,
                            audience: _configuration.Value.Audience,
                            claims: Claims,
                            notBefore: now,
                            expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                            signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }
    }
}
