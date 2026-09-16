using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Core.Account.Commands.GenerateCaptcha;
using ContractorBackend.Application.Core.Account.Commands.LoginFirstStep;
using ContractorBackend.Application.Core.Account.Commands.LoginSecondStep;
using ContractorBackend.Application.Core.Account.Commands.ValidateCaptcha;
using ContractorBackend.Application.Core.Account.Commands.ValidateOtp;
using ContractorBackend.Application.Core.Account.Query.GetUserInfo;
using ContractorBackend.Application.Core.Account.Query.GetUserInfoFromToken;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Common.Token;
using ContractorBackend.Application.Core.Account.Commands.ChangeActiveUser;
using ContractorBackend.Application.Core.Account.Commands.CurrentUserPassChange;
using ContractorBackend.Application.Core.Account.Commands.GenerateOTP;
using ContractorBackend.Application.Core.Account.Commands.LoginAccount;
using ContractorBackend.Application.Core.Account.Commands.LoginDevelop;
using ContractorBackend.Application.Core.Account.Commands.LogoutAccount;
using ContractorBackend.Application.Core.Account.Commands.PassRecreation;
using ContractorBackend.Application.Core.Account.Commands.RegisterAccount;
using ContractorBackend.Application.Core.Account.Commands.RegisterAccountBatch;
using ContractorBackend.Application.Core.Account.Commands.UpdateAccount;
using ContractorBackend.Application.Core.Account.Commands.UpdateAllUsersFromIsSuite;
using ContractorBackend.Application.Core.Account.Query.GetAllList;
using ContractorBackend.Application.Core.Account.Query.GetByIdUser;
using ContractorBackend.Application.Core.Account.Query.GetUserByNationalCodeNIDCard;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Resources;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.Persistence.Services;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class AccountController : ApiControllerBase
    {
        private readonly ITokenFactoryService _tokenFactory;
        private readonly ITokenStoreService _tokenStore;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<AccountController> _logger;



        public AccountController(
            ITokenFactoryService tokenFactory,
            ITokenStoreService tokenStore,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IStringLocalizer<SharedResource> localizer,
            ILogger<AccountController> logger
             )
        {
            _tokenFactory = tokenFactory;
            _tokenStore = tokenStore;
            _env = env;
            _configuration = configuration;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<OkApiResult<bool>> BatchRegister(RegisterAccountBatchCommand command)
        {
            if (!_env.IsDevelopment())
            {
                return new OkApiResult<bool>(false);
            }

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<OkApiResult<bool>> UpdateAllUsers(UpdateAllUsersFromIsSuiteCommand command)
        {
            if (!_env.IsDevelopment())
            {
                return new OkApiResult<bool>(false);
            }

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }


        [HttpPost]
        [DisplayName("ثبت کاربر")]
        public async Task<OkApiResult<bool>> Register(RegisterAccountCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        [HttpPost]
        public async Task<OkApiResult<bool>> Update(UpdateAccountCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }



        /// <summary>
        /// Login for develop only
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns> 
        [HttpPost]
        [DisplayName("لاگین توسعه دهنده")]
        [AllowAnonymous]
        [ErrorCode("100-32")]
        public async Task<OkApiResult<TokenInfo>> LoginDevelop([FromBody] LoginDevelopCommand command)
        {

            if (!_env.IsDevelopment())
            {
                return new OkApiResult<TokenInfo>(new TokenInfo());
            }

            var res = await Mediator.Send(command);
            return new OkApiResult<TokenInfo>(res);
        }


        /// <summary>
        /// Login admin
        /// UI CODE :100-02
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("لاگین")]
        [ErrorCode("100-02")]
        [AllowAnonymous]
        public async Task<OkApiResult<TokenInfo>> Login([FromBody] LoginAccountCommand command)
        {
            var twoStepLogin = _configuration.GetTwoStepLogin();

            #region Captcha Validation
            var captchaQuery = new ValidateCaptchaCommand { Captcha = command.Captcha, Key = command.Key };
            var captchaRes = await Mediator.Send(captchaQuery);

            if (!captchaRes)
            {
                throw new CustomException(_localizer["WrongCaptcha"]);
            }
            #endregion

            var ExceptionOTPEmployees = _configuration.GetSection("DeveloperHelpers")?.GetSection("ExceptionOTPEmployees").Get<string[]>();

            if (twoStepLogin && !(ExceptionOTPEmployees != null && ExceptionOTPEmployees.Any(x => x == command.Username)))
            {

                var innerCommand = new LoginFirstStepCommand
                {
                    Username = command.Username,
                    Password = command.Password,
                    Captcha = command.Captcha,
                    RoleType = command.RoleType
                };

                await Mediator.Send(innerCommand);

                #region Generate OTP
                var otpCommand = new GenerateOtpCommand
                {
                    Username = command.Username
                };
                var otpRes = await Mediator.Send(otpCommand);

                if (!otpRes)
                {
                    throw new CustomException(_localizer["OtpCreateError"]);
                }
                #endregion

                return new OkApiResult<TokenInfo>(new TokenInfo());
            }
            command.RoleType = RoleType.Manager;
            return new OkApiResult<TokenInfo>(await Mediator.Send(command));
        }


        /// <summary>
        /// Login Confirm
        /// UI CODE :100-13
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("تایید لاگین")]
        [AllowAnonymous]
        [ErrorCode("100-13")]
        public async Task<OkApiResult<TokenInfo>> LoginConfirm([FromBody] LoginAccountCommand command)
        {
            var twoStepLogin = _configuration.GetTwoStepLogin(); // _config.Value.TwoStepLogin;

            if (twoStepLogin)
            {
                #region Validate otp 
                var validateCommand = new ValidateOtpCommand
                {
                    Username = command.Username,
                    Otp = command.Otp,
                };
                var resValidate = await Mediator.Send(validateCommand);
                if (!resValidate)
                {
                    throw new CustomException(_localizer["OtpNotValid"]);
                }
                #endregion


                #region Login User if Otp is Valid
                var innerCommand = new LoginSecondStepCommand
                {
                    Username = command.Username,
                    Password = command.Password,
                    Otp = command.Otp,
                    RoleType = command.RoleType
                };
                var res = await Mediator.Send(innerCommand);
                return new OkApiResult<TokenInfo>(new TokenInfo { AccessToken = res.AccessToken, RefreshToken = res.RefreshToken });
                #endregion
            }

            return new OkApiResult<TokenInfo>(new TokenInfo { AccessToken = "", RefreshToken = "" });
        }



        [HttpGet]
        [DisplayName("لیست کاربران")]
        [ErrorCode("100-15")]
        public async Task<OkApiResult<SearchQueryResponse<UserLovDto>>> GetAll([FromQuery] GetAllUserQuery query)
        {
            return new OkApiResult<SearchQueryResponse<UserLovDto>>(await Mediator.Send(query));
        }

        [HttpGet]
        [DisplayName("کاربر")]
        [ErrorCode("100-15")]
        public async Task<OkApiResult<UserDto>> GetById([FromQuery] GetByIdUserQuery query)
        {
            return new OkApiResult<UserDto>(await Mediator.Send(query));
        }

        [HttpPost]
        [DisplayName("تغییر پسورد کاربر جاری")]
        [IsGlobal]
        public async Task<OkApiResult<bool>> CurrentUserPassChange([FromBody] CurrentUserPassChangeCommand command)
        {
            command.RoleType = RoleType.Manager;
            var res = await Mediator.Send(command);
            return new OkApiResult<bool>(res);
        }


        /// <summary>
        /// ForgotPassword UI CODE :100-03
        /// درمرحله اول فراموشی رمز عبور است متود قراخوان میشود
        /// پس از یافتن کاربر در سمت سرور یک otp به شماره  موبایل کاربر ارسال میشود
        /// پسورد حداقل 6 رقمی ترکیبی از حروف کوچک و بزرگ و عدد و سیمبل باشد
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns> 
        [HttpPost]
        [DisplayName("فراموشی رمز عبور مرحله اول")]
        [ErrorCode("100-03")]
        [AllowAnonymous]
        public async Task<OkApiResult<bool>> ForgotPasswordInitialize([FromBody] GetUserByNationalCodeNIDCardQuery query)
        {
            if (query.WithCaptcha)
            {
                #region Captcha Validation
                var captchaQuery = new ValidateCaptchaCommand { Captcha = query.Captcha, Key = query.Key };
                var captchaRes = await Mediator.Send(captchaQuery);

                if (!captchaRes)
                {
                    throw new CustomException(_localizer["WrongCaptcha"]);
                }
                #endregion
            }

            var res = await Mediator.Send(query);
            return new OkApiResult<bool>(res);
        }

        /// <summary>
        /// GenerateOtp UI CODE :100-09
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد رمز یکبار مصرف")]
        [AllowAnonymous]
        [ErrorCode("100-09")]
        public async Task<OkApiResult<bool>> GenerateOtp([FromBody] GenerateOtpCommand command)
        {
            return new OkApiResult<bool>(await Mediator.Send(command));
        }

        /// <summary>
        /// /// SetForgottenPass UI CODE :100-04
        ///   پسورد حداقل 6 رقمی ترکیبی از حروف کوچک و بزرگ و عدد و سیمبل باشد
        /// رمز یکباز مصرف ارسال شده به مویاسل یه همراه رمز جدید و تکرار آن به این متود ارسال میشود
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("ایجاد رمز فراموش شده")]
        [AllowAnonymous]
        [ErrorCode("100-04")]
        public async Task<OkApiResult<bool>> SetForgottenPass([FromBody] PasswordRecreationCommand command)
        {
            var res = await Mediator.Send(command);
            return new OkApiResult<bool>(res);

        }

        /// <summary>
        /// IsTokenValid UI CODE :100-05
        /// checking for token validation in case of expiration date
        /// and content of token an return userModel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ErrorCode("100-05")]
        [AllowAnonymous]
        public async Task<bool> IsTokenValid()
        {

            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                return false;
            }

            authHeader = authHeader.Replace("Bearer ", "");
            var isTokenValid = _tokenFactory.IsTokenValid(authHeader);
            if (isTokenValid.Item1 == false)
            {
                return false;
            }

            if (!_tokenStore.IsUserTokenExist(isTokenValid.Item2))
            {
                return false;

            }

            if (isTokenValid.Item1 && isTokenValid.Item2 != -1)
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        /// <summary>
        /// Logout UI CODE :100-07
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [DisplayName("خروج از حساب کاربری")]
        [ErrorCode("100-07")]
        [AllowAnonymous]
        public async Task<OkApiResult<bool>> Logout([FromBody] LogoutAccountCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }


        /// <summary>
        /// GenerateCaptcha UI CODE :100-08
        /// در تمامی قسمتهایی که باید کپچا برای سرور ارسال شود لطفا علاوه بر حاصلجمع وارد شده توسط کاربر مقدار 
        /// کلیدی که در این مدل (key)به همراه عکس یرای کلاینت ارسال میشود به سرور مجددا برگردانده شود
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("تولید کد امنیتی")]
        [AllowAnonymous]
        [ErrorCode("100-08")]
        public async Task<OkApiResult<CaptchaDto>> GenerateCaptchaAsync()
        {
            var command = new GenerateCaptchaCommand();
            return new OkApiResult<CaptchaDto>(await Mediator.Send(command));

        }

        /// <summary>
        /// /// UI CODE :100-11
        /// گرفتن اطلاعات کامل کاربر جاری
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن اطلاعات کامل کاربر جاری")]
        [ErrorCode("100-11")]
        [Obsolete("Use GetCurrentUserInfo Instead")]
        public async Task<OkApiResult<UserProfileDto>> GetUserInfo()
        {
            long id = HttpContext.GetUserId();
            var model = await Mediator.Send(new GetUserInfoQuery(id));
            return new OkApiResult<UserProfileDto>(model);
        }

        /// <summary>
        /// /// UI CODE :100-13
        /// درصورتیکه توکن معتبر باشد اطلاعات کاربر جاری را برمیگرداند
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("گرفتن اطلاعات کاربر جاری")]
        [ErrorCode("100-13")]
        [IsGlobal]
        public async Task<OkApiResult<UserMinimalDto>> GetCurrentUserInfo()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token?.Replace("Bearer ", "") ?? string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException();
            }

            var model = await Mediator.Send(new GetUserInfoFromTokenQuery());
            return new OkApiResult<UserMinimalDto>(model);
        }

        [HttpPost]
        [DisplayName("تغییر وضعیت کاربر")]
        public async Task<OkApiResult<bool>> ChangeActiveUser([FromBody] ChangeActiveUserCommand command)
        {
            return new OkApiResult<bool>(await Mediator.Send(command));
        }



    }
}
