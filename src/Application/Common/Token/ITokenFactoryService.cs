using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Token
{
    public interface ITokenFactoryService
    {
        Task<JwtTokensData> CreateJwtTokensAsync(User user);
        Task<JwtTokensData> CreateJwtTokensAsync(JwtUserInfo user);
        string GetRefreshTokenSerial(string refreshTokenValue);
        (bool, long) IsTokenValid(string accessToken);

        //bool IsUserTokenExist(bool userId);
    }
}
