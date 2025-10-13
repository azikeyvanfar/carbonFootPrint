using System.Security.Claims;
using ContractorBackend.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.WebApiClient.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? UserId
        {
            get
            {
                long? userId = null;
                var userIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (long.TryParse(userIdValue, out long result))
                {
                    userId = result;
                }

                return userId;
            }
        }
    }
}
