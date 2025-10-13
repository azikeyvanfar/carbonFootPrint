using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContractorBackend.Application.Common
{
    public static class Utilities
    {
        /// <summary>
        /// کد lookup حوزه سیستم
        /// </summary>
        public const string ScopeCode = "projectscope";
        /// <summary>
        /// کد فوت در lookup
        /// </summary>
        public const string DeathEnumCode = "1";
        /// <summary>
        /// کد احتمال که سمبل های مربوط به خودش رو واکشی میکنه
        /// P -> A,B,C...E
        /// </summary>
        public const string hasProbabilitySymbol = "Probability";
        /// <summary>
        /// کد شدت که سمبل های مربوط به خودش رو واکشی میکنه
        /// S -> 1,2,3,4,5
        /// </summary>
        public const string hasseveritySymbol = "Severity";

        public const string InitialReviewReason = "InitialReviewReason";
        /// <summary>
        /// کد اب در شاخص - سیستم شاخص های پایداری
        /// </summary>
        public const string H2OEnumCode = "H2O";
        /// <summary>
        /// کد کربن دی اکسید در شاخص - سیستم شاخص های پایداری
        /// </summary>
        public const string CO2EnumCode = "CO2";
        /// <summary>
        /// کد انرژی در شاخص - سیستم شاخص های پایداری
        /// </summary>
        public const string EnergyEnumCode = "Energy";

        /// <summary>
        /// ادرس کال کردن api ستینگ در کلاینت
        /// </summary>
        public const string clientSettingCreateApiRoute = "api/cli/Share/settings/create";
        public const string clientSettingGetActionsApiRoute = "api/cli/Share/settings/GetApplicationActions";
        public const string clientSettingGetApiRoute = "api/cli/Share/settings/GetApplicationSettings";

        /// <summary>
        /// number of decimals to Round 
        /// </summary>
        public const int DecimalPoints = 3;
        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="code">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static bool IsValidNationalCode(string code)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (string.IsNullOrWhiteSpace(code))
                //throw new BadRequestException("لطفا کد ملی را صحیح وارد نمایید");
                return false;


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (code.Length != 10)
                //throw new BadRequestException("طول کد ملی باید ده کاراکتر باشد");
                return false;

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(code))
                //throw new BadRequestException("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");
                return false;

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(code)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = code.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
            var c = b % 11;

            return ((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a));
        }

        public static string GetHashPasswordAsync(string pass)
        {
            byte[] salt;
            byte[] buffer2;

            if (!string.IsNullOrWhiteSpace(pass))
            {

                using (Rfc2898DeriveBytes bytes = new(pass, 0x10, 0x3e8))
                {
                    salt = bytes.Salt;
                    buffer2 = bytes.GetBytes(0x20);
                }
                byte[] dst = new byte[0x31];
                Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
                Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
                return Convert.ToBase64String(dst);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GenerateOtp()
        {
            Random generator = new();
            string generateOtp = generator.Next(0, 999999).ToString("D6");

            return generateOtp;
        }

        /// <summary>
        /// Set QueryParams with following fields:
        /// limit: number of records to show
        /// offset: numbers of records to skip
        /// </summary>
        /// <param name="request">request of queries that extends <see cref="SearchQueryRequest"/> </param>
        /// <returns></returns> 
        public static List<QueryParamModel> GetIsSuiteCoreQueryParams(SearchQueryRequest request)
        {
            var Page = request.Page > 0 ? request.Page : 1;
            var PageSize = request.PageSize > 0 ? request.PageSize : 10;

            var queryParams = new List<QueryParamModel>()     {
              new QueryParamModel  { ParameterName  =  "limit" , ParameterValue=(PageSize>0 ? PageSize : 10 ).ToString() },
              new QueryParamModel  { ParameterName  =  "offset" , ParameterValue=((Page > 0 ? Page-1 : Page) * PageSize).ToString()}
            };

            queryParams = queryParams.Concat(GetRequestFiltersAndTransformToIsSuiteQueryParams(request)).ToList();

            return queryParams;
        }

        /// <summary>
        /// Just for BPMS with page/size
        /// Set QueryParams with following fields:
        /// limit: number of records to show
        /// offset: numbers of records to skip
        /// </summary>
        /// <param name="request">request of queries that extends <see cref="SearchQueryRequest"/> </param>
        /// <returns></returns> 
        public static List<QueryParamModel> GetIsSuiteCoreQueryParamsForBPMS(SearchQueryRequest request)
        {
            var Page = request.Page > 0 ? request.Page : 1;
            var PageSize = request.PageSize > 0 ? request.PageSize : 10;

            var queryParams = new List<QueryParamModel>()     {
              new QueryParamModel  { ParameterName  =  "page" , ParameterValue = (Page-1).ToString() },
              new QueryParamModel  { ParameterName  =  "size" , ParameterValue = PageSize.ToString()}
            };

            queryParams = queryParams.Concat(GetRequestFiltersAndTransformToIsSuiteQueryParams(request)).ToList();

            return queryParams;
        }

        /// <summary>
        /// gets Request.Filter string and transforms all parameters to QueryParams List
        /// </summary>
        /// <param name="request">request of queries that extends <see cref="SearchQueryRequest"/> </param>
        /// <returns></returns> 
        public static List<QueryParamModel> GetRequestFiltersAndTransformToIsSuiteQueryParams(SearchQueryRequest request)
        {
            var queryParamsList = new List<QueryParamModel>();
            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                var segments = request.Filter.Split(",");
                queryParamsList = segments.ToList().Select(x => new
                {
                    Key = x.Split("=")?[0],
                    Value = x.Split("=")?[1]
                }).Select(x => new QueryParamModel
                {
                    ParameterName = x.Key.ToString(),
                    ParameterValue = x.Value.ToString()
                })
               //.Where(x => x.ParameterName != "P_NUM_PRSN")
               .ToList();
            }
            return queryParamsList;
        }


        /// <summary>
        /// Set QueryParams with following fields:
        /// limit: 50,000
        /// offset: 0 
        /// </summary>
        /// <returns></returns> 
        public static List<QueryParamModel> GetIsSuiteMaxLimitOffsetParams()
        {
            var queryParams = new List<QueryParamModel>()     {
              new QueryParamModel  { ParameterName  =  "limit" , ParameterValue= 50_000.ToString() },
              new QueryParamModel  { ParameterName  =  "offset" , ParameterValue= 0.ToString()}
            };
            return queryParams;
        }

        /// <summary>
        /// Get Current User Personnel Code from <see cref="HttpContext"/>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static long GetCurrentUserPersonnelCode(HttpContext httpContext)
        {
            var _accessor = httpContext.RequestServices.GetService<IHttpContextAccessor>();
            var _userManager = httpContext.RequestServices.GetService<IApplicationUserManager>();

            //var userId = _accessor.HttpContext.GetUserId();
            //var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var user = _userManager.GetCurrentUser();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return long.TryParse(user.PersonnelCode, out long code) ? code : throw new ArgumentNullException(nameof(user));
        }

        public static int GetDayOfMonthInPersian(DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetDayOfMonth(dateTime);
        }




        public static T? GetShadowProperty<T>(object d, string shadowPropertyName)
        {
            try
            {
                return EF.Property<T?>(d, shadowPropertyName);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
