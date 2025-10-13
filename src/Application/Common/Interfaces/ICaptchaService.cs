using ContractorBackend.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface ICaptchaService
    {
        CaptchaDto GenerateCaptchaImage(int first, int second);
        public GenerateCaptchaCodeDto GenerateCaptchaCode();
        bool ValidateCaptchaCode(string userInputCaptcha, HttpContext httpContext);
        //bool ValidateCaptcha(string token, string value);
    }
}
