using System;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Common.Models.SiteSettings;
using ContractorBackend.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ContractorBackend.Persistence.Services.Identity
{
    public class UsedPasswordsService : IUsedPasswordsService
    {
        private readonly int _changePasswordReminderDays;
        private readonly int _notAllowedPreviouslyUsedPasswords;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IApplicationDbContext _uow;
        private readonly DbSet<UserUsedPassword> _userUsedPasswords;

        public UsedPasswordsService(
            IApplicationDbContext uow,
            IPasswordHasher<User> passwordHasher,
            IOptionsSnapshot<SiteSettings> configurationRoot)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));

            _userUsedPasswords = _uow.Set<UserUsedPassword>();
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            if (configurationRoot == null) throw new ArgumentNullException(nameof(configurationRoot));
            var configurationRootValue = configurationRoot.Value;
            if (configurationRootValue == null) throw new ArgumentNullException(nameof(configurationRootValue));
            _notAllowedPreviouslyUsedPasswords = configurationRootValue.NotAllowedPreviouslyUsedPasswords;
            _changePasswordReminderDays = configurationRootValue.ChangePasswordReminderDays;
        }

        public async Task AddToUsedPasswordsListAsync(User user)
        {
            await _userUsedPasswords.AddAsync(new UserUsedPassword
            {
                UserId = user.Id,
                HashedPassword = user.PasswordHash
            });
            await _uow.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastUserPasswordChangeDateAsync(long userId)
        {
            var lastPasswordHistory =
                await _userUsedPasswords//.AsNoTracking() --> removes shadow properties
                                        .OrderByDescending(userUsedPassword => userUsedPassword.Id)
                                        .FirstOrDefaultAsync(userUsedPassword => userUsedPassword.UserId == userId);
            if (lastPasswordHistory == null)
            {
                return null;
            }

            var createdDateValue = _uow.GetShadowPropertyValue(lastPasswordHistory, AuditableShadowProperties.CreatedDateTime);
            DateTime createddate = DateTime.Parse(createdDateValue.ToString());
            return createdDateValue == null ?
                      (DateTime?)null :
                      DateTime.SpecifyKind(createddate, DateTimeKind.Utc);
        }

        public async Task<bool> IsLastUserPasswordTooOldAsync(long userId)
        {
            var createdDateTime = await GetLastUserPasswordChangeDateAsync(userId);
            if (createdDateTime == null)
            {
                return false;
            }
            return createdDateTime.Value.AddDays(_changePasswordReminderDays) < DateTime.UtcNow;
        }

        /// <summary>
        /// This method will be used by CustomPasswordValidator automatically,
        /// every time a user wants to change his/her info.
        /// </summary>
        public async Task<bool> IsPreviouslyUsedPasswordAsync(User user, string newPassword)
        {
            if (user.Id == 0)
            {
                // A new user wants to register at our site
                return false;
            }

            if (_notAllowedPreviouslyUsedPasswords == 0)
            {
                return false;
            }

            var userId = user.Id;
            var usedPasswords = await _userUsedPasswords
                                .AsNoTracking()
                                .Where(userUsedPassword => userUsedPassword.UserId == userId)
                                .OrderByDescending(userUsedPassword => userUsedPassword.Id)
                                .Select(userUsedPassword => userUsedPassword.HashedPassword)
                                .Take(_notAllowedPreviouslyUsedPasswords)
                                .ToListAsync();
            return usedPasswords.Any(hashedPassword => _passwordHasher.VerifyHashedPassword(user, hashedPassword, newPassword) != PasswordVerificationResult.Failed);
        }


    }
}