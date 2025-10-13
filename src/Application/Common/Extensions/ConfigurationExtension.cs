using System.Collections.Generic;
using ContractorBackend.Application.Common.Dtos;
using ContractorBackend.Application.Dtos.Setting;
using ContractorBackend.Application.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ContractorBackend.Persistence.Services
{
    public static class ConfigurationExtension
    {

        public static bool GetTwoStepLogin(this IConfiguration configuration)
        {
            return configuration.GetSection("ApplicationSettings")?.GetValue<bool>("TwoStepLogin") ?? false;
        }

        public static RateLimitDecorator GetDefaultRateLimit(this IConfiguration configuration)
        {
            var rateLimitTimeWindow = configuration.GetSection("ApplicationSettings")?.GetValue<int>("RateLimitTimeWindow") ?? 1;
            var rateLimitMaxRequests = configuration.GetSection("ApplicationSettings")?.GetValue<int>("RateLimitMaxRequests") ?? 100;

            var res = new RateLimitDecorator
            {
                TimeWindow = rateLimitTimeWindow,
                MaxRequests = rateLimitMaxRequests
            };
            return res;
        }

        public static List<RateLimitCustomRuleDto> GetRateLimitCustomRules(this IConfiguration configuration)
        {
            var rateLimitCustomRules = new List<RateLimitCustomRuleDto>();
            var aa = configuration.GetSection("ApplicationSettings")?.GetSection("RateLimitCustomRules");
            if (aa.Value != null)
            {
                rateLimitCustomRules = JsonConvert.DeserializeObject<List<RateLimitCustomRuleDto>>(aa.Value);
            }
            return rateLimitCustomRules;
        }


        public static RateLimitDecorator SetDefaultRateLimit(this IConfiguration configuration, RateLimitDecorator rateLimitDecorator)
        {
            configuration.GetSection("ApplicationSettings")["RateLimitTimeWindow"] = rateLimitDecorator.TimeWindow.ToString();
            configuration.GetSection("ApplicationSettings")["RateLimitMaxRequests"] = rateLimitDecorator.MaxRequests.ToString();

            return configuration.GetDefaultRateLimit();
        }


        /// <summary>
        ///این قسمت کار نمیکند و ابجکت مورد نظر را در ستینگ فایل ذخیره نمیکند
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="rateLimitCustomRules"></param>
        /// <returns></returns>
        public static List<RateLimitCustomRuleDto> SetRateLimitCustomRules(this IConfiguration configuration, List<RateLimitCustomRuleDto> rateLimitCustomRules)
        {
            configuration.AddOrRemove<RateLimitCustomRuleDto>(rateLimitCustomRules);
            return configuration.GetRateLimitCustomRules();
        }

        public static bool AddOrRemove<T>(this IConfiguration configuration, List<T> items)
        {
            var configurationSection = configuration.GetSection("ApplicationSettings").GetSection("RateLimitCustomRules");
            var rateLimitCustomRules = configuration.GetSection("ApplicationSettings")?.GetSection("RateLimitCustomRules")?.Get<List<RateLimitCustomRuleDto>>();

            var list = configurationSection.Get<List<T>>() ?? new();

            list.Clear();


            foreach (var item in items)
            {
                list.Add(item);
            }

            configuration.GetSection("ApplicationSettings").GetSection("RateLimitCustomRules").Value = JsonConvert.SerializeObject(list);

            return true;
        }




        public static string DecryptC(this string strEncry)
        {
            var strDe = AESService.Decrypt(strEncry);
            return strDe;
        }
        public static string EncryptC(this string strdecry)
        {
            var strEn = AESService.Encrypt(strdecry);
            return strEn;
        }

    }
    //public static class ConfigurationSectionExtension
    //{  
    //    public static string DecryptS(this string strEncry)
    //    {
    //        var strDe = AESService.Decrypt(strEncry);
    //        return strDe;
    //    }
    //    public static string EncryptS(this string strdecry)
    //    {
    //        var strEn = AESService.Encrypt(strdecry);
    //        return strEn;
    //    }

    //}

}
