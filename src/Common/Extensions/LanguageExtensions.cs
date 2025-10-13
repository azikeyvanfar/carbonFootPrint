using System;

namespace ContractorBackend.Common.Extensions
{
    public class LanguageExtensions
    {
        public static Guid GetLanguageId()
        {
            //var claimsIdentity = httpContext?.User.Identity as ClaimsIdentity;
            //var userIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
            //if (long.TryParse(userIdValue, out long userId))
            //{
            //    if (userId == 0)
            //    {
            //        throw new UnauthorizedAccessException();
            //    }

            //    return userId;
            //}

            //throw new UnauthorizedAccessException();
            return Guid.Parse("48cca39c-8157-4abc-bcc8-d7f8f362640e");
        }
    }
}
