using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static long GetUserId(this HttpContext? httpContext)
        {
            var claimsIdentity = httpContext?.User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
            if (long.TryParse(userIdValue, out long userId))
            {
                if (userId == 0)
                {
                    throw new UnauthorizedAccessException();
                }

                return userId;
            }

            throw new UnauthorizedAccessException();
        }


        /// <summary>
        /// Get User Id If Exists JUST FOR LOGGING PURPOSES - DO NOT USE IN PROJECT
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static long? GetUserIdForLogging(this HttpContext? httpContext)
        {
            var claimsIdentity = httpContext?.User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
            if (long.TryParse(userIdValue, out long userId))
            {
                if (userId == 0)
                {
                    return null;
                }

                return userId;
            }
            return null;
        }


        public static string GetCurrentUserIp(this HttpContext? httpContext)
        {
            return httpContext?.Connection?.RemoteIpAddress?.ToString();
        }
    }
}
