using System.Threading.Tasks;
using ContractorBackend.Application.Services;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Common.Identity
{
    public interface ISmsSender
    {
        #region BaseClass

        Task<bool> SendSmsAsync(User user, SmsRequest smsRequest, SmsType type);
        Task<bool> SendSmsAsync(SmsRequest smsRequest, SmsType type);
        Task<SmsResponse> SendSMS(SmsRequest body);
        #endregion

        #region CustomMethods

        #endregion
    }
}