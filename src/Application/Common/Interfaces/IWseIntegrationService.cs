namespace ContractorBackend.Application.Common.Interfaces
{
    // سرویس اتصال به حقوق و دستمزد
    public interface IWseIntegrationService : IRestsharpRequest
    {
        // متد بررسی توکن
        bool GetToken();

        // متد بررسی لاگین بودن
        bool Login();

        // متد بررسی احراز هویت
        bool CheckAuthenticationProccess();


    }
}
