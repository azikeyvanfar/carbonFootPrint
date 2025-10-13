using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Identity
{
    public interface ISiteStatService
    {
        Task<List<User>> GetOnlineUsersListAsync(int numbersToTake, int minutesToTake);

        Task UpdateUserLastVisitDateTimeAsync(ClaimsPrincipal claimsPrincipal);
    }
}