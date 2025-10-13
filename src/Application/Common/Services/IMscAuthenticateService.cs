using System.Threading.Tasks;
using ContractorBackend.Application.Models;

namespace ContractorBackend.Application.Common.Services
{
    public interface IMscAuthenticateService
    {
        string GetReferrer(string code);
        Task<MscLoginResult> CheckMscLoginAsync(string ticket);
        bool IsTicketValid(string ticket, long personnelCode);
    }
}