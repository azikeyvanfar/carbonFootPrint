using System;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Identity
{
    public interface IUsedPasswordsService
    {
        Task<bool> IsPreviouslyUsedPasswordAsync(User user, string newPassword);
        System.Threading.Tasks.Task AddToUsedPasswordsListAsync(User user);
        Task<bool> IsLastUserPasswordTooOldAsync(long userId);
        Task<DateTime?> GetLastUserPasswordChangeDateAsync(long userId);


    }
}