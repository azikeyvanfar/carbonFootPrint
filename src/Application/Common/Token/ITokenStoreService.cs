using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Token
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(JwtUserToken userToken);
        Task AddUserTokenAndSaveChangesAsync(JwtUserToken userToken);
        Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task AddUserTokenAsync(long userId, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task<bool> IsValidTokenAsync(string accessToken, long userId);
        Task DeleteExpiredTokensAsync();
        Task<JwtUserToken?> FindTokenAsync(string refreshTokenValue);
        Task DeleteTokenAsync(string refreshTokenValue);
        Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        Task InvalidateUserTokensAsync(long userId);
        bool IsUserTokenExist(long userId);
        Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue);
        Task RevokeUserBearerTokensAndSaveChangesAsync(string userIdValue, string refreshTokenValue);
    }
}
