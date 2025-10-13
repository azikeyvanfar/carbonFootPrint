using System;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Dtos;
using ContractorBackend.Persistence.Services;
using ContractorBackend.WebApiClient.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.WebApiClient.MiddleWare
{
    public class CustomRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public CustomRateLimitMiddleware(RequestDelegate next,
                                      IDistributedCache cache,
                                      IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cache = cache ?? throw new ArgumentNullException(nameof(_cache));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var rateLimitDecorator = _configuration.GetDefaultRateLimit();

            #region RateLimit Overrides
            rateLimitDecorator = await ApplyRateLimitOverrideRules(rateLimitDecorator, context);
            #endregion

            var key = GenerateClientKey(context);
            var _clientStatistics = GetClientStatisticsByKey(key).Result;


            if (_clientStatistics != null
                && DateTime.UtcNow < _clientStatistics.LastSuccessfulResponseTime.AddMinutes(rateLimitDecorator.TimeWindow)
                && _clientStatistics.NumberofRequestsCompletedSuccessfully == rateLimitDecorator.MaxRequests)
            {
                ResponseRoot aa = new ResponseRoot()
                {
                    Title = "تعداد تلاش ها بیش از حد مجاز است",
                    Status = (int)StatusCodes.Status400BadRequest
                };
                context.Response.StatusCode = (int)StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync<ResponseRoot>(aa);
                return;
            }
            await UpdateClientStatisticsAsync(key, rateLimitDecorator.MaxRequests);
            await _next(context);

        }

        /// <summary>
        /// generate ClientKey from the context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GenerateClientKey(HttpContext context)
         => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";


        /// <summary>
        /// Get the client statistics from caching
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<ClientStatistics> GetClientStatisticsByKey(string key) => await _cache.GetCachedValueAsyn<ClientStatistics>(key);

        private async Task UpdateClientStatisticsAsync(string key, int maxRequests)
        {
            var _clientStats = _cache.GetCachedValueAsyn<ClientStatistics>(key).Result;
            if (_clientStats is not null)
            {
                _clientStats.LastSuccessfulResponseTime = DateTime.UtcNow;
                if (_clientStats.NumberofRequestsCompletedSuccessfully >= maxRequests)
                    _clientStats.NumberofRequestsCompletedSuccessfully = 1;
                else
                    _clientStats.NumberofRequestsCompletedSuccessfully++;

                await _cache.SetCachedValueAsync<ClientStatistics>(key, _clientStats);
            }
            else
            {
                var clientStats = new ClientStatistics
                {
                    LastSuccessfulResponseTime = DateTime.UtcNow,
                    NumberofRequestsCompletedSuccessfully = 1
                };

                await _cache.SetCachedValueAsync<ClientStatistics>(key, clientStats);
            }
        }

        /// <summary>
        /// if custom setting is set in appsetting.json return that
        /// otherwise return default
        /// </summary>
        /// <param name="defaultRateLimitDecorator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<RateLimitDecorator> ApplyRateLimitOverrideRules(RateLimitDecorator defaultRateLimitDecorator, HttpContext context)
        {
            var RateLimitCustomRules = _configuration.GetRateLimitCustomRules();//.GetSection("ApplicationSettings")?.GetSection("RateLimitCustomRules")?.Get<List<RateLimitCustomRule>>();

            if (RateLimitCustomRules != null && RateLimitCustomRules.Any())
            {
                foreach (var item in RateLimitCustomRules)
                {
                    if (
                        item.IsHeader == true
                        && item.RateLimitTimeWindow.Value > 0
                        && item.RateLimitMaxRequests.Value > 0
                        && context.Request.Headers.ToList().Any(x => x.Key.Contains(item.Name, StringComparison.InvariantCulture))
                        )
                    {
                        return new RateLimitDecorator
                        {
                            // in Minutes
                            TimeWindow = item.RateLimitTimeWindow.Value,
                            MaxRequests = item.RateLimitMaxRequests.Value
                        };
                    }
                    else if (
                            item.IsHeader == false
                            && item.RateLimitTimeWindow.Value > 0
                            && item.RateLimitMaxRequests.Value > 0
                            && context.Request.Path.Value.Contains(item.Name, StringComparison.InvariantCulture)
                            )
                    {
                        return new RateLimitDecorator
                        {
                            // in Minutes
                            TimeWindow = item.RateLimitTimeWindow.Value,
                            MaxRequests = item.RateLimitMaxRequests.Value
                        };
                    }
                }
            }


            return defaultRateLimitDecorator;

        }
    }

    public class ClientStatistics
    {
        public DateTime LastSuccessfulResponseTime { get; set; }
        public int NumberofRequestsCompletedSuccessfully { get; set; }
    }

    public class ResponseRoot
    {
        public string Title { get; set; }
        public int Status { get; set; }
    }

}
