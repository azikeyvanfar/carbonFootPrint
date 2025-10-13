using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ContractorBackend.Application.Core.ApplicationSettingPage.Queries.CheckUserHasAccessToSettings
{
    public class CheckUserHasAccessToSettingsQuery : IRequest<bool>
    {
    }


    public class CheckUserHasAccessToSettingsQueryHandler : IRequestHandler<CheckUserHasAccessToSettingsQuery, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenStoreService _tokenStore;
        private readonly HttpClientMethods _httpClient;

        public CheckUserHasAccessToSettingsQueryHandler(
            IApplicationDbContext dbContext,
            IMapper mapper,
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            ITokenStoreService tokenStore,
            HttpClientMethods httpClient)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = accessor;
            _tokenStore = tokenStore;
            _httpClient = httpClient;
        }
        public async Task<bool> Handle(CheckUserHasAccessToSettingsQuery request, CancellationToken cancellationToken)
        {
            if (!HasAccess())
            {
                throw new UnauthorizedAccessException();
            }
            return true;
        }


        public bool HasAccess()
        {

            if (_httpContextAccessor.HttpContext is null)
            {
                return false;
            }
            List<Claim> claims = new();
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is null || !authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                return false;
            }
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            //checking for datetimeexpiration
            if (jwtSecurityToken.ValidTo <= DateTime.UtcNow)
            {
                return false;
            }
            //checking for existance in database
            long tokenUserId = Convert.ToInt64(jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value);
            if (!_tokenStore.IsUserTokenExist(tokenUserId))
            {
                return false;
            }
            //checking for validation of token itself
            if (!ValidateToken(token, handler))
            {
                return false;
            }

            var jtiValue = jwtSecurityToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
            var userIdValue = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            var identity = new ClaimsIdentity();
            identity.AddClaims(jwtSecurityToken.Claims);
            _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(identity);

            var adminClaims = new List<string> { "core:settings:getapplicationsettings", "core:settings:create" };

            var userclaims = _dbContext.Set<Domain.Entities.Identity.UserRole>().Where(x => x.UserId == tokenUserId).Include(x => x.Role).ThenInclude(x => x.Claims)
                .SelectMany(x => x.Role.Claims)
                .Where(x => adminClaims.Any(l => l == x.ClaimValue));

            if (userclaims == null)
            {
                return false;
            }

            return true;

        }
        private bool ValidateToken(string authToken, JwtSecurityTokenHandler tokenHandler)
        {
            try
            {
                var validationParameters = GetValidationParameters();
                if (validationParameters != null)
                {
                    //first check for expiration
                    var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(authToken);
                    if (jwtSecurityToken.ValidTo > DateTime.UtcNow)
                    {
                        SecurityToken validatedToken;
                        IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                //ValidateAudience = true, // Because there is no audiance in the generated token
                //ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = _configuration["BearerTokensSettings:Issuer"],
                ValidAudience = _configuration["BearerTokensSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["BearerTokensSettings:Key"].Decrypt())) // The same key as the one that generate the token
            };
        }



    }
}
