using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ContractorBackend.Persistence.Services.Token
{
    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly IApplicationUserManager _usersService;
        private readonly ITokenStoreService _tokenStoreService;

        public TokenValidatorService(IApplicationUserManager usersService, ITokenStoreService tokenStoreService)
        {
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            _tokenStoreService = tokenStoreService ?? throw new ArgumentNullException(nameof(tokenStoreService));
        }



        public async Task<bool> ValidateAsync(TokenValidatedContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return false;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                context.Fail("This is not our issued token. It has no serial.");
                return false;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!long.TryParse(userIdString, out long userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return false;
            }

            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _tokenStoreService.IsValidTokenAsync(accessToken.RawData, userId))
            {
                context.Fail("This token is not in our database.");
                return false;
            }

            await _usersService.UpdateUserLastActivityDateAsync(userId);
            return true;
        }
    }
}
