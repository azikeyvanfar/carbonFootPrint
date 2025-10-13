using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ContractorBackend.Application.Common.Token
{
    public interface ITokenValidatorService
    {
        Task<bool> ValidateAsync(TokenValidatedContext context);
    }
}
