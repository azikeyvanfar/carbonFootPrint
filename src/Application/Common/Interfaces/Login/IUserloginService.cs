using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Common.Interfaces.Login
{
    public interface IUserloginService
    {
        Task<User> CheckUserExistsAndHasRoleAndReturnUser(string username, string password, RoleType roleType, CancellationToken cancellationToken);

    }
}
