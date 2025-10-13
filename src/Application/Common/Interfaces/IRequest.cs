using System.Threading.Tasks;
using RestSharp;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IRestsharpRequest
    {
        /// <summary>
        /// آدرس اصلی وب سرویس
        /// </summary>
        string BaseAddress { get; set; }
        IRestClient Client { get; }
        /// <summary>
        /// ساخت شی request با توجه به ساختار هر وب سرویس
        /// </summary>
        /// <param name="action">action to excute</param>
        /// <param name="method">method type</param>
        /// <param name="input">پارامتر های مورد نیاز در وب سرویس</param>
        /// <returns></returns>
        IRestRequest ConfigRequest(string action, Method method, object input);
        /// <summary>
        /// اجرای درخواست و برگردان شی در پارامتر دیتا ، با توجه به نوع ارسالی
        /// </summary>
        /// <typeparam name="T">نوع داده شی بازگشتی</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        IRestResponse<T> ExecuteRequest<T>(IRestRequest request) where T : new();
        /// <summary>
        /// اجرای درخواست
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IRestResponse ExecuteRequest(IRestRequest request);
        /// <summary>
        /// اجرای درخواست و برگردان شی در پارامتر دیتا ، با توجه به نوع ارسالی
        /// به صورت آسینک
        /// </summary>
        /// <typeparam name="T">نوع داده شی بازگشتی</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IRestResponse<T>> ExecuteRequestAsync<T>(IRestRequest request);

        Task<IRestResponse> ExecuteRequestAsync(IRestRequest request);
    }
}
