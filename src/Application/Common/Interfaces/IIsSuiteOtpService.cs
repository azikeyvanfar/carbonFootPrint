using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IIsSuiteOtpService
    {
        Task<bool> SendIsSuiteOtpRequestCodeAsync();
        void SetCurrentUserOtpCode(string otpValue);
        string GetCurrentUserOtpCode();
        void RemoveOtpCodeForPersonnel();

        bool GetIsInDebounceTime();
        bool SetIsInDebounceTime();
    }
}
