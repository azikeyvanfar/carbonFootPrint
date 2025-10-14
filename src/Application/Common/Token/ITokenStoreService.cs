using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Token
{
    public interface ITokenStoreService
    {
        System.Threading.Tasks.Task AddUserTokenAsync(JwtUserToken userToken);
        System.Threading.Tasks.Task AddUserTokenAndSaveChangesAsync(JwtUserToken userToken);
        System.Threading.Tasks.Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        System.Threading.Tasks.Task AddUserTokenAsync(long userId, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task<bool> IsValidTokenAsync(string accessToken, long userId);
        System.Threading.Tasks.Task DeleteExpiredTokensAsync();
        Task<JwtUserToken?> FindTokenAsync(string refreshTokenValue);
        System.Threading.Tasks.Task DeleteTokenAsync(string refreshTokenValue);
        System.Threading.Tasks.Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        System.Threading.Tasks.Task InvalidateUserTokensAsync(long userId);
        bool IsUserTokenExist(long userId);
        System.Threading.Tasks.Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue);
        System.Threading.Tasks.Task RevokeUserBearerTokensAndSaveChangesAsync(string userIdValue, string refreshTokenValue);
    }
}
