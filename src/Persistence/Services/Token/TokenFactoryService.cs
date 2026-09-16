using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Common.Models.SiteSettings;
using ContractorBackend.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ContractorBackend.Persistence.Services.Token
{
    public class TokenFactoryService : ITokenFactoryService
    {
        private readonly ISecurityService _securityService;
        private readonly IOptionsSnapshot<BearerTokensSettings> _configuration;
        private readonly IApplicationRoleManager _rolesService;
        private readonly ILogger<TokenFactoryService> _logger;
        private readonly IHttpContextAccessor _accessor;

        public TokenFactoryService(
            ISecurityService securityService,
            IApplicationRoleManager rolesService,
            IOptionsSnapshot<BearerTokensSettings> configuration,
            ILogger<TokenFactoryService> logger,
            IHttpContextAccessor accessor)
        {

            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
            _rolesService = rolesService ?? throw new ArgumentNullException(nameof(rolesService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accessor = accessor;
        }


        public async Task<JwtTokensData> CreateJwtTokensAsync(User user)
        {
            var (accessToken, claims) = await createAccessTokenAsync(user);
            var (refreshTokenValue, refreshTokenSerial) = createRefreshToken();
            return new JwtTokensData
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                RefreshTokenSerial = refreshTokenSerial,
                Claims = claims
            };
        }

        public async Task<JwtTokensData> CreateJwtTokensAsync(JwtUserInfo user)
        {
            var (accessToken, claims) = await createAccessTokenAsync(user);
            var (refreshTokenValue, refreshTokenSerial) = createRefreshToken();
            return new JwtTokensData
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                RefreshTokenSerial = refreshTokenSerial,
                Claims = claims
            };
        }

        private (string RefreshTokenValue, string RefreshTokenSerial) createRefreshToken()
        {
            string refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString()
                .Replace("-", "", StringComparison.OrdinalIgnoreCase);

            var IPClient = GetIpAddress(_accessor);
            var BrowserClient = _accessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                // for invalidation
                new Claim(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim("ClientIP", IPClient, _configuration.Value.Issuer),
                new Claim("ClientBrowser", BrowserClient, _configuration.Value.Issuer)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                signingCredentials: creds);
            var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return (refreshTokenValue, refreshTokenSerial);
        }

        public string GetRefreshTokenSerial(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return null;
            }

            ClaimsPrincipal decodedRefreshTokenPrincipal = null;
            try
            {
                decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                    refreshTokenValue,
                    new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC())),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = false, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    },
                    out _
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to validate refreshTokenValue: `{refreshTokenValue}`.");
            }

            return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        }

        private async Task<(string AccessToken, IEnumerable<Claim> Claims)> createAccessTokenAsync(User user)
        {
            var IPClient = GetIpAddress(_accessor);
            var BrowserClient = _accessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim("PCode", user.PersonnelCode ?? string.Empty, ClaimValueTypes.String, _configuration.Value.Issuer),
                // custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim("ClientIP", IPClient, _configuration.Value.Issuer),
                new Claim("ClientBrowser", BrowserClient, _configuration.Value.Issuer)
            };

            #region Add Roles

            var roles = _rolesService.FindUserRoles(user.Id);
            // add role names
            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, _configuration.Value.Issuer));

            //    //var roleClaims = await _rolesService.GetClaimsAsync(role).ConfigureAwait(false);

            //    //if (roleClaims != null && roleClaims.Count > 0)
            //    //{
            //    //    claims.AddRange(roleClaims);
            //    //}
            //}
            // add role dto
            var rolesDto = roles.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                RoleType = x.RoleType,
                IsAdministrator = x.IsAdministrator
            });
            #endregion

            var rolesJson = JsonConvert.SerializeObject(rolesDto);
            claims.Add(new Claim(ClaimTypes.Role, rolesJson, ClaimValueTypes.String, _configuration.Value.Issuer));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                signingCredentials: creds);
            return (new JwtSecurityTokenHandler().WriteToken(token), claims);
        }

        private async Task<(string AccessToken, IEnumerable<Claim> Claims)> createAccessTokenAsync(JwtUserInfo user)
        {
            var IPClient = GetIpAddress(_accessor);
            var BrowserClient = _accessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim("DisplayName", user.DisplayName, ClaimValueTypes.String, _configuration.Value.Issuer),
                // to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber, user.SerialNumber ?? "1", ClaimValueTypes.String, _configuration.Value.Issuer),
                // custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                new Claim("ClientIP", IPClient, _configuration.Value.Issuer),
                new Claim("ClientBrowser", BrowserClient, _configuration.Value.Issuer)
            };

            // add roles
            var roles = _rolesService.FindUserRoles(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, _configuration.Value.Issuer));

                //var roleClaims = await _rolesService.GetClaimsAsync(role).ConfigureAwait(false);

                //if (roleClaims != null && roleClaims.Count > 0)
                //{
                //    claims.AddRange(roleClaims);
                //}
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                signingCredentials: creds);
            return (new JwtSecurityTokenHandler().WriteToken(token), claims);
        }

        /// <summary>
        /// ValidateToken return false if it's not valid.
        /// </summary>
        /// <returns></returns>
        private (bool, long) ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            try
            {
                //first check for expiration
                var jwtSecurityToken = tokenHandler.ReadJwtToken(authToken);
                #region check ip

                string ipClient = GetIpAddress(_accessor);
                string ipClientToken = (jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == "ClientIP") != null) ? jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == "ClientIP").Value : null;
                if (ipClient == null || ipClientToken != ipClient)
                {
                    return (false, -1);
                }

                #endregion

                #region check browser

                string browserClient = _accessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
                string browserClientToken = jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == "ClientBrowser").Value;
                if (browserClient == null || browserClientToken != browserClient)
                {
                    return (false, -1);
                }

                #endregion
                if (jwtSecurityToken.ValidTo > DateTime.UtcNow)
                {
                    SecurityToken validatedToken;

                    IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);

                    return (true, Convert.ToInt64(jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value));
                }
                else
                {
                    return (false, -1); // throw new CustomException("10"   );  //توکن معتبر نیست
                }
            }
            catch (Exception)
            {
                return (false, -1); // throw new CustomException("10" );  //توکن معتبر نیست
            }
        }
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = _configuration.Value.Issuer,
                ValidAudience = _configuration.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key.DecryptC())) // The same key as the one that generate the token
            };
        }
        public (bool, long) IsTokenValid(string accessToken)
        {
            return this.ValidateToken(accessToken);
        }
        /// <summary>
        /// دریافت ip کاربر
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetIpAddress(IHttpContextAccessor? httpContextAccessor)
        {
            string ip = "::1";
            var headers = httpContextAccessor?.HttpContext?.Request.Headers;
            if (headers != null && headers.ContainsKey("X-Forwarded-For"))
            {

                try
                {
                    var forwardedFor = httpContextAccessor?.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();
                    var ipList = forwardedFor.Split(',');
                    if (ipList.Length > 0)
                    {
                        var originalIP = System.Net.IPAddress.Parse(ipList[0].Trim());
                        ip = originalIP.ToString();
                    }

                }
                catch (Exception)
                {

                    try { ip = httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "::1"; } catch { ip = "::1"; }
                }

            }
            else
            {
                try { ip = httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "::1"; } catch { ip = "::1"; }
            }
            return ip;
        }


    }
}
