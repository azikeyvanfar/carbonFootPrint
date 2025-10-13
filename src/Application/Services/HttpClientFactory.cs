using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Common.Models;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContractorBackend.Application.Services
{
    public class HttpClientFactory
    {
        private readonly HttpClient _clientFactory;
        public HttpClientFactory(IHttpClientFactory clientHelper)
        {
            _clientFactory = clientHelper.CreateClient();
        }
        public HttpClient GetClient()
        {
            return _clientFactory;
        }
    }

    public class HttpClientMethods
    {
        private readonly ILogger<HttpClientMethods> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationDbContext _dbContext;

        private readonly string baseUrl = "http://services.msc.ir/";
        private readonly string baseUrlCPM = "http://cpm.msc.ir/";
        private readonly string BPMSBaseUrl = "http://services.msc.ir/";
        private readonly string baseUrlWithoutAuth = "http://services.msc.ir/";
        private readonly string loginUrl = "http://services.msc.ir/ords/fnd/public/ords_issuite_login";
        private readonly string authUrl = "http://services.msc.ir/ords/oauth/token";
        private readonly string bpmsAuthUrl = "http://services.msc.ir/bpms/oauth/token";
        private readonly string oaAuthUrl = "http://services.msc.ir/ords/oauth/token";

        private readonly HttpClientFactory _client;
        public HttpClientMethods(
            HttpClientFactory clientFactory,
            ILogger<HttpClientMethods> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            IApplicationDbContext dbContext)
        {
            _client = clientFactory;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _accessor = accessor;
            _dbContext = dbContext;

            baseUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteBaseUrl"]) ? _configuration["IsSuiteBaseUrl"].Decrypt() : baseUrl;
            baseUrlCPM = !string.IsNullOrWhiteSpace(_configuration["BaseUrlCPM"]) ? _configuration["BaseUrlCPM"].Decrypt() : baseUrlCPM;
            BPMSBaseUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteBPMSBaseUrl"]) ? _configuration["IsSuiteBPMSBaseUrl"].Decrypt() : BPMSBaseUrl;
            baseUrlWithoutAuth = !string.IsNullOrWhiteSpace(_configuration["IsSuiteBaseUrlWithoutAuth"]) ? _configuration["IsSuiteBaseUrlWithoutAuth"].Decrypt() : baseUrlWithoutAuth;
            loginUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteLoginUrl"]) ? _configuration["IsSuiteLoginUrl"].Decrypt() : loginUrl;
            authUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteAuthUrl"]) ? _configuration["IsSuiteAuthUrl"].Decrypt() : authUrl;
            bpmsAuthUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteBPMSUrl"]) ? _configuration["IsSuiteBPMSUrl"].Decrypt() : bpmsAuthUrl;
            oaAuthUrl = !string.IsNullOrWhiteSpace(_configuration["IsSuiteOAAuthUrl"]) ? _configuration["IsSuiteOAAuthUrl"].Decrypt() : oaAuthUrl;
        }



        #region Self HttpRequest
        /// <summary>
        /// صدا زدن هر api در خود پروژه
        /// </summary> 
        public async Task<string> Get(string url, string token)
        {
            var client = _client.GetClient();

            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Add("PersonnelHeaderSelfCall", url);

                _logger.LogInformation($"Self Http Call {nameof(GetService)} Method Call Url : {url}");

                using (var response = await client.GetAsync(url))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Self Http Call {nameof(GetService)} Method Response body : {stringResponse}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetService)} url failed. and response is :{stringResponse}");
                        throw new AccessViolationException($" Self Http Call {nameof(GetService)} Error With Code: {response.StatusCode}");
                    }

                    //var apiResponse = JObject.Parse(stringResponse);
                    //var apiResponse = JsonConvert.DeserializeObject<myClassTest<dynamic>>(stringResponse);
                    return stringResponse;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<TResponse> Post<TBody, TResponse>(string url, TBody body, string token)
        {
            try
            {
                var client = _client.GetClient();

                var contentStr = System.Text.Json.JsonSerializer.Serialize(body);
                var stringContent = new StringContent(contentStr, Encoding.UTF8, MediaTypeNames.Application.Json);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("content_type", "application/json");
                client.DefaultRequestHeaders.Add("client_authentication", "header");

                _logger.LogInformation($"Self Http Call {nameof(Post)} Method Call Url : {url}");
                using (var response = await client.PostAsync(url, stringContent))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Self Http Call {nameof(Post)} Method Response body : {response.Content}");
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(Post)} url failed. and response is :{stringResponse}");
                        throw new AccessViolationException($" Self Http Call {nameof(Post)} Error With Code: {response.StatusCode}");
                    }
                    var apiResponse = JsonConvert.DeserializeObject<TResponse>(stringResponse);
                    return apiResponse;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region ORDS Http

        /// <summary>
        /// Provide and cache Token for valid period of Provided token
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProvideTokenAndLogin(ServiceEnum service)
        {
            var validToken = (TokenCache)_memoryCache.Get(service.ToString() + "-code");

            if (validToken is not null && validToken.ExpireDateTime > DateTime.UtcNow.AddMinutes(5))
            {
                return validToken.AccessToken;
            }
            var tokenCache = await GetTokenAsync(service);
            var loginResult = await IsISSuiteLogin(tokenCache.AccessToken);

            if (loginResult && !string.IsNullOrEmpty(tokenCache.AccessToken))
            {
                try
                {
                    _memoryCache.Set(service.ToString() + "-code", new TokenCache
                    {
                        AccessToken = tokenCache.AccessToken,
                        ExpireDateTime = tokenCache.ExpireDateTime
                    }, new MemoryCacheEntryOptions { AbsoluteExpiration = tokenCache.ExpireDateTime, Size = 1 });

                }
                catch (Exception)
                {
                    throw;
                }
                return tokenCache.AccessToken;
            }
            return "";
        }

        /// <summary>
        /// get access token from foulad subsystem
        /// </summary>
        /// <returns></returns>
        public async Task<TokenCache> GetTokenAsync(ServiceEnum system)
        {
            var accessToken = new TokenCache();
            try
            {
                FouladClientCredentialsService serv = new FouladClientCredentialsService(_configuration);
                var credentials = serv.GetClientCredentials(system);

                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                string base64Encode = Base64Encode($"{credentials.ClientId}:{credentials.ClientSecret}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {base64Encode}");
                #endregion

                #region Body
                var dict = new Dictionary<string, string>();
                dict.Add("grant_type", "client_credentials");
                var requestContent = new FormUrlEncodedContent(dict);
                #endregion

                using (var response = await client.PostAsync(authUrl, requestContent))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetTokenAsync)}: failed with StatusCode:{response.StatusCode}. for service {system}, UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    var res = JsonConvert.DeserializeObject<TokenResponseObject>(apiResponse);

                    var hasExpireIn = int.TryParse(res.expires_in, out var expireSeconds);
                    accessToken.AccessToken = res.access_token;
                    accessToken.ExpireDateTime = DateTime.UtcNow.AddSeconds(hasExpireIn ? expireSeconds : 0);
                }
                return accessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Login to Is-Suite with project Username and Password
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsISSuiteLogin(string token)
        {
            var uri = new Uri(loginUrl);
            //var username = "PORTAL_ORDS_USR";
            //var password = "Prt%159^753";
            var username = "RSA@123456789";
            var password = "qazwsx@123";
            bool loginResult = false;
            try
            {
                string apiResponse = string.Empty;
                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                #endregion 

                #region Body
                var dict = new Dictionary<string, string>();
                dict.Add("P_USER_NAME", username);
                dict.Add("P_PASS", password);
                var requestContent = new FormUrlEncodedContent(dict);
                #endregion

                using (var response = await client.PostAsync(uri, requestContent))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(IsISSuiteLogin)}: failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    loginResult = JsonConvert.DeserializeObject<LoginResponseObject>(apiResponse).result;
                }
                return loginResult;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetService<T>(string url, List<QueryParamModel>? queryParams, ServiceEnum service)
        {
            var token = await ProvideTokenAndLogin(service);

            var client = _client.GetClient();

            string finalUrl = "";

            // check if http call is sensitive and if it is attach OTP QueryParams to request
            if (await CheckIsSensitiveUrlAndCheckOtp(url))
            {
                queryParams = AttachOtpCodeToQueryParams(queryParams);
            }

            try
            {
                finalUrl = $"{url}";
                if (queryParams is not null && queryParams.Count > 0)
                {
                    var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                    finalUrl = finalUrl + "?" + queryParamsStr;
                }

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Call Url : {finalUrl}");

                using (var response = await client.GetAsync(baseUrl + finalUrl))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Response body : {response.Content}");
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, Url is {finalUrl}, and response is :{stringResponse}");
                        throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {await response.Content.ReadAsStringAsync()}");
                    }
                    var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                    return apiResponse;
                }

                //return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> PostService<T>(string url, List<QueryParamModel>? queryParams, object? content, ServiceEnum system)
        {
            var token = await ProvideTokenAndLogin(system);

            var client = _client.GetClient();

            var contentStr = System.Text.Json.JsonSerializer.Serialize(content);


            var finalUrl = $"{url}";
            if (queryParams is not null && queryParams.Count > 0)
            {
                var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                finalUrl = finalUrl + "?" + queryParamsStr;
            }

            var stringContent = new StringContent(contentStr, Encoding.UTF8, MediaTypeNames.Application.Json);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("content_type", "application/json");
            client.DefaultRequestHeaders.Add("client_authentication", "header");

            using (var response = await client.PostAsync(baseUrl + finalUrl, content != null ? stringContent : null))
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Response body : {response.Content}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{stringResponse}");
                    throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {response.Content}");
                }
                var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                return apiResponse;
            }
        }
        #endregion



        #region OSB Http

        /// <summary>
        /// Http Get call Without Authentication
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <param name="service"></param>
        /// <returns></returns> 
        public async Task<T> GetServiceWithoutAuthentication<T>(string url, List<QueryParamModel>? queryParams, ServiceEnum service)
        {
            var client = _client.GetClient();

            string finalUrl = "";

            try
            {
                finalUrl = $"{url}";
                if (queryParams is not null && queryParams.Count > 0)
                {
                    var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                    finalUrl = finalUrl + "?" + queryParamsStr;
                }
                client.DefaultRequestHeaders.Clear();

                using (var response = await client.GetAsync(baseUrlWithoutAuth + finalUrl))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetServiceWithoutAuthentication)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{stringResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);

                    ArgumentNullException.ThrowIfNull(apiResponse);

                    return apiResponse;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion



        #region BPMS Http

        /// <summary>
        /// Provide and cache Token for valid period of Provided token
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProvideTokenAndLoginForBPMSSrvices(ServiceEnum service)
        {
            var validToken = (TokenCache)_memoryCache.Get(service.ToString() + "-bp-code");

            if (validToken is not null && validToken.ExpireDateTime > DateTime.UtcNow.AddMinutes(5))
            {
                return validToken.AccessToken;
            }
            var tokenCache = await BPMSSystemLogin(service);

            if (!string.IsNullOrEmpty(tokenCache.AccessToken))
            {
                try
                {
                    _memoryCache.Set(service.ToString() + "-bp-code", new TokenCache
                    {
                        AccessToken = tokenCache.AccessToken,
                        ExpireDateTime = tokenCache.ExpireDateTime
                    }, new MemoryCacheEntryOptions { AbsoluteExpiration = tokenCache.ExpireDateTime, Size = 1 });

                }
                catch (Exception)
                {
                    throw;
                }
                return tokenCache.AccessToken;
            }
            return "";
        }

        public async Task<TokenCache> BPMSSystemLogin(ServiceEnum system)
        {
            var accessToken = new TokenCache();
            try
            {
                FouladClientCredentialsService serv = new FouladClientCredentialsService(_configuration);
                var credentials = serv.GetClientCredentials(system);
                var usernamePassword = serv.GetClientUsernamePassword(system);

                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                #endregion

                #region Body
                var body = JsonConvert.SerializeObject(new
                {
                    client_id = credentials.ClientId,
                    client_secret = credentials.ClientSecret,
                    grant_type = "password",
                    username = usernamePassword.Username,
                    password = usernamePassword.Password,
                });
                var buffer = System.Text.Encoding.UTF8.GetBytes(body);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                #endregion

                using (var response = await client.PostAsync(bpmsAuthUrl, byteContent))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(BPMSSystemLogin)}: failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, for service {system}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted. BPMS");
                    }
                    var res = JsonConvert.DeserializeObject<TokenResponseObject>(apiResponse);

                    var hasExpireIn = int.TryParse(res.expires_in, out var expireSeconds);
                    accessToken.AccessToken = res.access_token;
                    accessToken.ExpireDateTime = DateTime.UtcNow.AddSeconds(hasExpireIn ? expireSeconds : 0);
                }
                return accessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Http Get Specially for BPMS Services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <param name="body"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<T> GetBPMSService<T>(string url, List<QueryParamModel>? queryParams, object? body, ServiceEnum service)
        {
            var token = await ProvideTokenAndLoginForBPMSSrvices(service);

            var client = _client.GetClient();

            string finalUrl = "";

            try
            {
                finalUrl = $"{url}";
                if (queryParams is not null && queryParams.Count > 0)
                {
                    var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                    finalUrl = finalUrl + "?" + queryParamsStr;
                }

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var baseUrl = BPMSBaseUrl;
                using (var response = await client.GetAsync(baseUrl + finalUrl))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetBPMSService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{stringResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                    return apiResponse;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<T> PostBPMSService<T>(string url, List<QueryParamModel>? queryParams, object? body, ServiceEnum service)
        {
            var token = await ProvideTokenAndLoginForBPMSSrvices(service);

            var client = _client.GetClient();

            string finalUrl = "";

            try
            {
                finalUrl = $"{url}";
                if (queryParams is not null && queryParams.Count > 0)
                {
                    var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                    finalUrl = finalUrl + "?" + queryParamsStr;
                }

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                #region Body
                var bodySerialized = JsonConvert.SerializeObject(body);
                var buffer = System.Text.Encoding.UTF8.GetBytes(bodySerialized);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                #endregion

                var baseUrl = BPMSBaseUrl;
                using (var response = await client.PostAsync(baseUrl + finalUrl, byteContent))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(T);
                    }
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(PostBPMSService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{stringResponse}");
                        throw new AccessViolationException(String.Format("Access Api with Error {0}.", response.StatusCode));
                    }
                    var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                    return apiResponse;
                }

                //return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion



        #region OA Http

        /// <summary>
        /// Provide and cache Token for valid period of Provided token
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProvideTokenAndLoginOA(ServiceEnum service)
        {
            var validToken = (TokenCache)_memoryCache.Get(service.ToString() + "-code-oa");

            if (validToken is not null && validToken.ExpireDateTime > DateTime.UtcNow.AddMinutes(1))
            {
                return validToken.AccessToken;
            }
            var tokenCache = await GetTokenAsyncOA(service);
            var loginResult = true; //await IsISSuiteLoginOA(tokenCache.AccessToken);

            if (loginResult && !string.IsNullOrEmpty(tokenCache.AccessToken))
            {
                try
                {
                    _memoryCache.Set(service.ToString() + "-code-oa", new TokenCache
                    {
                        AccessToken = tokenCache.AccessToken,
                        ExpireDateTime = tokenCache.ExpireDateTime
                    }, new MemoryCacheEntryOptions { AbsoluteExpiration = tokenCache.ExpireDateTime, Size = 1 });

                }
                catch (Exception)
                {
                    throw;
                }
                return tokenCache.AccessToken;
            }
            return "";
        }

        /// <summary>
        /// get access token from foulad subsystem
        /// </summary>
        /// <returns></returns>
        public async Task<TokenCache> GetTokenAsyncOA(ServiceEnum system)
        {
            var accessToken = new TokenCache();
            try
            {
                FouladClientCredentialsService serv = new FouladClientCredentialsService(_configuration);
                var credentials = serv.GetClientCredentials(system);

                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                #endregion

                #region Body
                var body = JsonConvert.SerializeObject(new
                {
                    ClientId = credentials.ClientId,
                    ClientSecret = credentials.ClientSecret,

                });
                var buffer = System.Text.Encoding.UTF8.GetBytes(body);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                #endregion


                using (var response = await client.PostAsync(oaAuthUrl, byteContent))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetTokenAsync)}: failed with StatusCode:{response.StatusCode}. for service {system}, UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    var res = JsonConvert.DeserializeObject<OATokenResponseObject>(apiResponse);

                    accessToken.AccessToken = res.token;
                    accessToken.ExpireDateTime = res.expiration;
                }
                return accessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Login to Is-Suite with project Username and Password
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsISSuiteLoginOA(string token)
        {
            var uri = new Uri(loginUrl);
            //var username = "PORTAL_ORDS_USR";
            //var password = "Prt%159^753";
            var username = "RSA@123456789";
            var password = "qazwsx@123";
            bool loginResult = false;
            try
            {
                string apiResponse = string.Empty;
                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                #endregion 

                #region Body
                var dict = new Dictionary<string, string>();
                dict.Add("P_USER_NAME", username);
                dict.Add("P_PASS", password);
                var requestContent = new FormUrlEncodedContent(dict);
                #endregion

                using (var response = await client.PostAsync(uri, requestContent))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(IsISSuiteLogin)}: failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    loginResult = JsonConvert.DeserializeObject<LoginResponseObject>(apiResponse).result;
                }
                return loginResult;

            }
            catch (Exception)
            {
                throw;
            }
        }

        ///// <summary>
        ///// Http Get For Office Automation (OA)
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <param name="queryParams"></param>
        ///// <param name="service"></param>
        ///// <returns></returns>
        //public async Task<T> GetServiceOA<T>(string url, List<QueryParamModel>? queryParams, ServiceEnum service)
        //{
        //    var token = await ProvideTokenAndLoginOA(service);

        //    var client = _client.GetClient();

        //    string finalUrl = "";

        //    // check if http call is sensitive and if it is attach OTP QueryParams to request
        //    if (await CheckIsSensitiveUrlAndCheckOtp(url))
        //    {
        //        queryParams = AttachOtpCodeToQueryParams(queryParams);
        //    }

        //    try
        //    {
        //        finalUrl = $"{url}";
        //        if (queryParams is not null && queryParams.Count > 0)
        //        {
        //            var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
        //            finalUrl = finalUrl + "?" + queryParamsStr;
        //        }

        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //        _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Call Url : {finalUrl}");

        //        using (var response = await client.GetAsync(baseUrl + finalUrl))
        //        {
        //            var stringResponse = await response.Content.ReadAsStringAsync();
        //            _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Response body : {response.Content}");
        //            if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //            {
        //                _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, Url is {finalUrl}, and response is :{stringResponse}");
        //                throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {await response.Content.ReadAsStringAsync()}");
        //            }
        //            var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
        //            return apiResponse;
        //        }

        //        //return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Http Post For Office Automation (OA)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <param name="content"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException"></exception>
        public async Task<T> PostServiceOA<T>(string url, List<QueryParamModel>? queryParams, object? content, ServiceEnum system)
        {
            var token = await ProvideTokenAndLoginOA(system);

            var client = _client.GetClient();

            var contentStr = System.Text.Json.JsonSerializer.Serialize(content);


            var finalUrl = $"{url}";
            if (queryParams is not null && queryParams.Count > 0)
            {
                var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                finalUrl = finalUrl + "?" + queryParamsStr;
            }

            var stringContent = new StringContent(contentStr, Encoding.UTF8, MediaTypeNames.Application.Json);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("content_type", "application/json");
            client.DefaultRequestHeaders.Add("client_authentication", "header");

            using (var response = await client.PostAsync(baseUrl + finalUrl, content != null ? stringContent : null))
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Response body : {response.Content}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{stringResponse}");
                    throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {response.Content}");
                }
                var res1 = JsonConvert.DeserializeObject<RootResultOA>(stringResponse);

                if (res1 == null || string.IsNullOrWhiteSpace(res1.Result))
                {
                    throw new Exception($"Error in deserialize object for OA api call Url:{baseUrl + finalUrl} , with response {stringResponse}");
                }
                string clearString = res1.Result.Replace("\\\"", "\"").Trim('\"');
                try
                {
                    var resultApi = JsonConvert.DeserializeObject<T>(clearString);
                    return resultApi;

                }
                catch (Exception e)
                {
                    var resultApi = JsonConvert.DeserializeObject<OAExceptionResult>(clearString);
                    throw new CustomException(resultApi.Result);
                }
            }
        }

        #endregion





        #region Old CPM Http

        /// <summary>
        /// Provide and cache Token for valid period of Provided token
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProvideCPMTokenAndLogin(string serviceName)
        {
            var validToken = (TokenCache)_memoryCache.Get(serviceName + "-code");

            if (validToken is not null && validToken.ExpireDateTime > DateTime.UtcNow.AddMinutes(5))
            {
                return validToken.AccessToken;
            }
            var tokenCache = await GetTokenCPMAsync();

            if (!string.IsNullOrEmpty(tokenCache.AccessToken))
            {
                try
                {
                    _memoryCache.Set(serviceName + "-code", new TokenCache
                    {
                        AccessToken = tokenCache.AccessToken,
                        ExpireDateTime = tokenCache.ExpireDateTime
                    }, new MemoryCacheEntryOptions { AbsoluteExpiration = tokenCache.ExpireDateTime, Size = 1 });

                }
                catch (Exception)
                {
                    throw;
                }
                return tokenCache.AccessToken;
            }
            return "";
        }

        /// <summary>
        /// get access token from foulad subsystem
        /// </summary>
        /// <returns></returns>
        public async Task<CPMTokenCache> GetTokenCPMAsync()
        {
            var accessToken = new CPMTokenCache();
            try
            {
                var loginUrl = "api/Users/Login";
                var username = "newcpm";
                var password = "Cpm@2025@new";
                //var username = "1271546361";
                //var password = "mn@123";

                var client = _client.GetClient();

                object content = new { username, password };

                var contentStr = System.Text.Json.JsonSerializer.Serialize(content);

                var stringContent = new StringContent(contentStr, Encoding.UTF8, MediaTypeNames.Application.Json);

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("content_type", "application/json");
                #endregion

                using (var response = await client.PostAsync(new Uri(baseUrlCPM + loginUrl), stringContent))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetTokenCPMAsync)}: failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, and response is :{apiResponse}");
                        throw new AccessViolationException("Access to Get token Api NOT granted.");
                    }
                    var res = JsonConvert.DeserializeObject<CpmTokenResponseObject>(apiResponse);

                    accessToken.AccessToken = res.token;

                    #region Get exp from token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(res.token);
                    accessToken.ExpireDateTime = jwtSecurityToken.Payload.ValidTo;
                    #endregion

                }
                return accessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetServiceCPM<T>(string url, List<QueryParamModel>? queryParams)
        {
            var token = await ProvideCPMTokenAndLogin("oldcpm");

            var client = _client.GetClient();

            string finalUrl = "";

            // check if http call is sensitive and if it is attach OTP QueryParams to request
            if (await CheckIsSensitiveUrlAndCheckOtp(url))
            {
                queryParams = AttachOtpCodeToQueryParams(queryParams);
            }

            try
            {
                finalUrl = $"{url}";
                if (queryParams is not null && queryParams.Count > 0)
                {
                    var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                    finalUrl = finalUrl + "?" + queryParamsStr;
                }

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                _logger.LogInformation($"Old CPM {nameof(GetService)} Method Call Url : {finalUrl}");

                using (var response = await client.GetAsync(baseUrlCPM + finalUrl))
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Is-Suite {nameof(GetService)} Method Response body : {response.Content}");
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, Url is {baseUrlCPM + finalUrl}, and response is :{stringResponse}");
                        throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {await response.Content.ReadAsStringAsync()}");
                    }
                    var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                    return apiResponse;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> PostServiceCPM<T>(string url, List<QueryParamModel>? queryParams, object? content)
        {
            var token = await ProvideCPMTokenAndLogin("oldcpm");

            var client = _client.GetClient();

            var contentStr = System.Text.Json.JsonSerializer.Serialize(content);


            var finalUrl = $"{url}";
            if (queryParams is not null && queryParams.Count > 0)
            {
                var queryParamsStr = string.Join("&", queryParams.Select(x => x.ParameterName + "=" + x.ParameterValue));
                finalUrl = finalUrl + "?" + queryParamsStr;
            }

            var stringContent = new StringContent(contentStr, Encoding.UTF8, MediaTypeNames.Application.Json);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("content_type", "application/json");
            client.DefaultRequestHeaders.Add("client_authentication", "header");

            using (var response = await client.PostAsync(new Uri(baseUrlCPM + finalUrl), content != null ? stringContent : null))
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Old CPM {nameof(GetService)} Method Response body : {response.Content}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"{nameof(GetService)} url failed with StatusCode:{response.StatusCode}. UserId:{_accessor.HttpContext.GetUserIdForLogging()}, Url is {baseUrlCPM + finalUrl}, and response is :{stringResponse}");
                    throw new AccessViolationException($"{nameof(GetService)} Error With Code: {response.StatusCode} - ErrorMessage {response.Content}");
                }
                var apiResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                return apiResponse;
            }
        }
        #endregion




        #region Tools

        /// <summary>
        /// Encode string used for coding Client Id/Secret
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        private string Base64Encode(string toEncode)
        {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        #endregion


        public async Task<bool> CheckIsSensitiveUrlAndCheckOtp(string url)
        {
            var uri = url.Replace(baseUrl, "");
            var isSensitiveUrl = IsSuiteUrlClass.sensitiveUrls2.Values.Any(x => uri.ToLowerInvariant().Equals(x.Url.ToLowerInvariant()));
            if (isSensitiveUrl)
            {
                var otpValidationResult = await CallSensitiveApiOtpCheck();
                if (otpValidationResult.lv_res != 5)
                {
                    throw new HPRException("", "این صفحه نیاز به تایید کد می باشد", StatusCodes.Status500InternalServerError, "000-11");
                }
                return true;
            }
            return false;
        }


        public async Task<IsSuiteResponseDto> CallSensitiveApiOtpCheck()
        {
            //var row = IsSuiteUrlClass.dict[IsSuiteUrlKeyEnum.pds_per_contracts_viw];

            //if (row is null)
            //{
            //    throw new ArgumentNullException($"url for given key not found.key : {IsSuiteUrlKeyEnum.pds_per_contracts_viw.ToString()}");
            //}

            //var url = row.Url;
            //var service = row.Service;

            //var userId = _accessor.HttpContext.GetUserId();
            //var user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == userId);
            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //var personnelCode = long.TryParse(user.PersonnelCode, out long code) ? code : throw new ArgumentNullException(nameof(user));
            //var pCode = (string)_memoryCache.Get(personnelCode + _accessor.HttpContext.GetCurrentUserIp() + "is-otp");

            //var queryParams = new List<QueryParamModel>()
            //{
            //    new QueryParamModel  { ParameterName  =  "P_NUM_PRSN" , ParameterValue = personnelCode.ToString()},
            //    new QueryParamModel  { ParameterName  =  "P_COD" , ParameterValue = pCode},
            //    new QueryParamModel  { ParameterName  =  "P_TYPE" , ParameterValue = "2"}
            //};

            //var response = await PostService<IsSuiteResponseDto>(url, queryParams, null, service);
            //return response;
            return null;
        }


        public List<QueryParamModel> AttachOtpCodeToQueryParams(List<QueryParamModel> queryParams)
        {
            var userId = _accessor.HttpContext.GetUserIdForLogging();
            var user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var personnelCode = long.TryParse(user.PersonnelCode, out long code) ? code : throw new ArgumentNullException(nameof(user));
            var pCode = (string)_memoryCache.Get(personnelCode + _accessor.HttpContext.GetCurrentUserIp() + "is-otp");

            queryParams = queryParams ?? new List<QueryParamModel>();
            queryParams.Add(new QueryParamModel() { ParameterName = "P_COD", ParameterValue = pCode });

            return queryParams;
        }



    }

    public class CpmTokenResponseObject
    {
        public string token { get; set; }
    }

    public class TokenResponseObject
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }

    }

    public class OATokenResponseObject
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }

    }
    public class LoginResponseObject
    {
        public bool result { get; set; }

    }


    public class TokenCache
    {
        public string AccessToken { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }


    public class CPMTokenCache
    {
        public string AccessToken { get; set; }
        public DateTime ExpireDateTime { get; set; }

    }


    public class SensitiveOtpApiCheckResponse
    {
        public int lv_res { get; set; }

    }

    public class ApiResult<T>
    {
        public bool IsSuccess { get; private set; }

        public int Status { get; private set; }

        public string? MethodCode { get; set; }

        public string[]? Message { get; private set; }

        public T Data { get; private set; }
    }



    public class RootResultOA
    {
        public string Result { get; set; }
    }

    public class TableResultOA<T>
    {
        public List<T> Table { get; set; }
    }

    public class OAExceptionResult
    {
        public string Result { get; set; }
    }



}
