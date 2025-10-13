using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Common.Constants;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ContractorBackend.WebApiAdmin.Filters
{
    public class DynamicAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationDbContext _context;
        private readonly DbSet<UserRole> _userRoles;
        private readonly DbSet<User> _users;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ITokenStoreService _tokenStore;

        public DynamicAuthorizeFilter(IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache, IApplicationDbContext context,
            ITokenStoreService tokenStore,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _context = context;
            _userRoles = _context.Set<UserRole>();
            _users = _context.Set<User>();
            _configuration = configuration;
            _tokenStore = tokenStore;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .SingleOrDefault();
            if (attribute is not null)
            {
                return;
            }
            if (_httpContextAccessor.HttpContext is null)
            {
                await ThrowError(StatusCodes.Status500InternalServerError, "Context NOT Found", "", "10", context); return;
            }
            List<Claim> claims = new();
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                //checking for datetimeexpiration
                if (jwtSecurityToken.ValidTo <= DateTime.UtcNow)
                {
                    await ThrowError(StatusCodes.Status500InternalServerError, "Token Validation Date Expired", "", "10", context); return;
                }
                //checking for existance in database
                long tokenUserId = Convert.ToInt64(jwtSecurityToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value);
                if (!_tokenStore.IsUserTokenExist(tokenUserId))
                {
                    await ThrowError(StatusCodes.Status500InternalServerError, "User Token NOT Exists", "", "10", context); return;
                }
                //checking for validation of token itself
                if (!ValidateToken(token, handler))
                {
                    await ThrowError(StatusCodes.Status500InternalServerError, "Token is NOT Valid", "", "10", context); return;
                }

                var jtiValue = jwtSecurityToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
                var userIdValue = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                var identity = new ClaimsIdentity();
                identity.AddClaims(jwtSecurityToken.Claims);
                _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(identity);

                var cacheClaims = _memoryCache.Get(jtiValue);
                if (cacheClaims is null)
                {
                    var userId = long.Parse(userIdValue);
                    var clientRoles = await _userRoles
                                       .Include(p => p.Role).ThenInclude(x => x.Claims)
                                       .ThenInclude(x => x.PageRouteClaim).ThenInclude(_ => _.GeneralClaim)
                                       .Where(p => p.UserId == userId && p.Role.RoleType == RoleType.Manager)
                                       .Select(p => new ClientRole
                                       {
                                           Role = p.Role.Name,
                                           ClientRoleClaims = p.Role.Claims
                                               .Select(q => new ClientRoleClaim
                                               {
                                                   ClaimType = q.ClaimType,
                                                   ClaimValue = q.PageRouteClaim.GeneralClaim.ClaimValue ?? "",
                                                   ClaimId =
                                                        q.PageRouteClaim != null && q.PageRouteClaim.GeneralClaim != null ?
                                                        q.PageRouteClaim.GeneralClaim.Id : Guid.NewGuid(),
                                               })
                                               .ToList(),
                                       })
                                       .AsNoTracking()
                                       .ToListAsync();

                    if (clientRoles != null)
                    {
                        foreach (var clientRole in clientRoles)
                        {
                            if (clientRole.ClientRoleClaims != null)
                            {
                                foreach (var claim in clientRole.ClientRoleClaims)
                                {
                                    if (claim.ClaimType == ConstantPolicies.DynamicPermission)
                                    {
                                        if (!string.IsNullOrWhiteSpace(claim.ClaimValue))
                                        {
                                            claims.Add(new Claim(ConstantPolicies.DynamicPermission,
                                            claim.ClaimValue,
                                            ClaimValueTypes.String));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!claims.Any())
                    {
                        await ThrowError(StatusCodes.Status500InternalServerError, "Forbidden. claim for current Api not found", "", "000-01", context); return;
                    }

                    _memoryCache.Set(jtiValue, claims, new MemoryCacheEntryOptions { Size = 1 });
                }
                else
                {
                    claims = (List<Claim>)cacheClaims;
                }

                if (_httpContextAccessor.HttpContext.User.Identity is null)
                {
                    await ThrowError(StatusCodes.Status500InternalServerError, "User is Unauthorized", "", "000-02", context); return;
                }

                var routeData = _httpContextAccessor.HttpContext.GetRouteData();

                var areaName = routeData?.Values["area"]?.ToString();
                var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;

                var controllerName = routeData?.Values["controller"]?.ToString();
                var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

                var actionName = routeData?.Values["action"]?.ToString();
                var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;

                string currentClaimValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(area))
                {
                    currentClaimValue = $"{area}:{controller}:{action}";
                }
                else
                {
                    currentClaimValue = $"{controller}:{action}";
                }

                var isGlobalClaim = _context.GeneralClaims.Any(x => x.IsGlobal && x.ClaimValue == currentClaimValue);
                if (claims.Any(claim => claim.Type == ConstantPolicies.DynamicPermission && claim.Value.ToLower() == currentClaimValue.ToLower()) || isGlobalClaim)
                {
                    var listUser = _users.Where(x => x.IsActive).Where(t => t.Id == long.Parse(userIdValue));
                    if (!listUser.Any())
                    {
                        await ThrowError(StatusCodes.Status500InternalServerError, "کاربر مورد نظر غیرفعال میباشد", "", "000-05", context); return;
                    }

                    #region Change Password For Security Reasons
                    if (!isGlobalClaim)
                    {
                        //var PassChangeForce = listUser.Include(_ => _.  Employee).Select(_ => new { _.IsPasswordChangeForce, _.PasswordChangeForceMsg }).FirstOrDefault();
                        //if (PassChangeForce != null && PassChangeForce.IsPasswordChangeForce)
                        //{
                        //    await ThrowError(StatusCodes.Status500InternalServerError, "به دلیل مسائل امنیتی شما باید رمز عبور خود را تغییر دهید", "", "000-05", context); return;
                        //}
                    }
                    #endregion
                    return;
                }
                else
                {
                    await ThrowError(StatusCodes.Status500InternalServerError, "شما دسترسی به این درخواست ندارید", "", "000-01", context); return;
                }
            }
            else
            {
                await ThrowError(StatusCodes.Status500InternalServerError, "Forbidden. Bearer token not found", "", "10", context); return;
            }
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

        /// <summary>
        /// ThrowError
        /// </summary>
        /// <param name="Status">500</param>
        /// <param name="Detail">No Need</param>
        /// <param name="Title">Persian Error Message</param>
        /// <param name="Type">JUST for Logout User (Code 10)</param>
        /// <param name="context">AuthorizationFilterContext</param>
        /// <returns></returns>
        private static Task ThrowError(int? Status, string? Title, string? Detail, string? Type, AuthorizationFilterContext context)
        {
            var details = new ProblemDetails
            {
                Status = Status,
                Title = Title,
                Detail = Detail,
                Type = Type
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = Status
            };
            return Task.CompletedTask;
        }
    }
}
