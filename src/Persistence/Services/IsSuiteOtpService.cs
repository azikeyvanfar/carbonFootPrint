using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace ContractorBackend.Persistence.Services
{
    public class IsSuiteOtpService : IIsSuiteOtpService
    {
        private readonly IsSuiteClientService _isSuitHttp;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMemoryCache _memoryCache;

        public IsSuiteOtpService(IsSuiteClientService isSuitHttp, IHttpContextAccessor accessor, IMemoryCache memoryCache)
        {
            _isSuitHttp = isSuitHttp;
            _accessor = accessor;
            _memoryCache = memoryCache;
        }

        public string GetCurrentUserOtpCode()
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            var otpCodeRecord = (string)_memoryCache.Get(personnelCode + _accessor.HttpContext.GetCurrentUserIp() + "is-otp");

            return otpCodeRecord;
        }

        public async Task<bool> SendIsSuiteOtpRequestCodeAsync()
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);

            var queryParams = new List<QueryParamModel>()
            {
                new QueryParamModel  { ParameterName  =  "P_NUM_PRSN" ,ParameterValue = personnelCode.ToString()},
                new QueryParamModel  { ParameterName  =  "P_COD" ,ParameterValue = ""},
                new QueryParamModel  { ParameterName  =  "P_TYPE" ,ParameterValue = "1"},
            };

            var result = await _isSuitHttp.CheckSMSCode(queryParams);


            ///P_COD=NULL && lv_res=-1 ارور در انجام کار
            ///P_COD=NULL && lv_res=0 کد ارسال شد
            ///P_COD=NULL && lv_res=1 چهار دقیقه از ارسال پیام نگذشته است
            ///P_COD=NULL && lv_res=2 چهار دقیقه از ارسال پیام گذشته است و ارسال مجدد صورت گرفته
            ///P_COD!=NULL && lv_res=3 کد وارد شده اشتباه است
            ///P_COD!=NULL && lv_res=4 زمان 10 دقیقه گذشته (expire)
            ///P_COD!=NULL && lv_res=5 کد وارد شده صحیح است
            return result.lv_res == 0 || result.lv_res == 1 || result.lv_res == 2;
        }

        public void SetCurrentUserOtpCode(string otpCode)
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            //_memoryCache.Set(personnelCode + "otp", otpCode, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(9), Size = 1 });
            _memoryCache.Set(personnelCode + _accessor.HttpContext.GetCurrentUserIp() + "is-otp", otpCode, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10), Size = 1 });
        }

        public void RemoveOtpCodeForPersonnel()
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            _memoryCache.Remove(personnelCode);
        }



        public bool GetIsInDebounceTime()
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            var record = _memoryCache.Get<bool>(personnelCode + "tolerance");
            return record;
        }

        public bool SetIsInDebounceTime()
        {
            var personnelCode = Utilities.GetCurrentUserPersonnelCode(_accessor.HttpContext);
            _memoryCache.Set(personnelCode + "tolerance", true, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1), Size = 1 });
            return true;
        }

    }
}
