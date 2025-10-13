using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContractorBackend.Application.Services
{
    public class SmsService : ISmsSender
    {
        private readonly ILogger<SmsService> _logger;
        private readonly HttpClientFactory _client;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<SmsHistory> _repository;
        public SmsService(
            HttpClientFactory clientFactory,
            ILogger<SmsService> logger,
            IConfiguration configuration,
            IRepository<SmsHistory> repository,
            IWebHostEnvironment env
            )
        {
            _client = clientFactory;
            _logger = logger;
            _configuration = configuration;
            _repository = repository;
            _env = env;
        }

        public async Task<bool> SendSmsAsync(User user, SmsRequest smsRequest, SmsType type)
        {
            //  در حالت دوولوپ این قسمت برداشته نشود
            //if (_env.IsDevelopment())
            //{
            //    var receivers = _configuration.GetSection("DeveloperHelpers")?["SMSReceivers"];
            //    smsRequest.Receivers = receivers;
            //}


            // JUST FOR TEST : save sms Request before call foulad sms service
            #region save sms Request before call foulad sms service
            var smsRequestPersist = new SmsResponse();
            smsRequestPersist.IsSuccessful = true;
            smsRequestPersist.Message = "Message Request Text:" + smsRequest.SmsText + " At DateTimeOffset:" + DateTimeOffset.Now.ToString();
            PersistSmsResponse(smsRequest, smsRequestPersist, user);
            #endregion


            var smsResponse = new SmsResponse();
            try
            {
                smsResponse = await SendSMS(smsRequest);
            }
            catch (Exception e)
            {
                smsResponse.IsSuccessful = false;
                smsResponse.Message = e.Message;
                smsResponse.StatusCode = 500;
            }

            var res = PersistSmsResponse(smsRequest, smsResponse, user);
            return res;
        }
        public async Task<bool> SendSmsAsync(SmsRequest smsRequest, SmsType type)
        {
            // JUST FOR TEST : save sms Request before call foulad sms service
            #region save sms Request before call foulad sms service
            var smsRequestPersist = new SmsResponse();
            smsRequestPersist.IsSuccessful = true;
            smsRequestPersist.Message = "Message Request Text:" + smsRequest.SmsText + " At DateTimeOffset:" + DateTimeOffset.Now.ToString();
            PersistSmsResponse(smsRequest, smsRequestPersist, null);
            #endregion


            var smsResponse = new SmsResponse();
            try
            {
                smsResponse = await SendSMS(smsRequest);
            }
            catch (Exception e)
            {
                smsResponse.IsSuccessful = false;
                smsResponse.Message = e.Message;
                smsResponse.StatusCode = 500;
            }

            var res = PersistSmsResponse(smsRequest, smsResponse, null);
            return res;
        }


        public async Task<SmsResponse> SendSMS(SmsRequest body)
        {
            var accessToken = new SmsResponse();
            try
            {
                FouladClientCredentialsService serv = new FouladClientCredentialsService(_configuration);
                var smsUrl = "";
                var SmsUsername = "";
                var SmsPassword = "";
                var templateMessage = "";

                if (body.SenderId == "otp")
                {
                    (smsUrl, SmsUsername, SmsPassword, templateMessage) = GetOtpSmsConfiguration();
                }
                else
                {
                    (smsUrl, SmsUsername, SmsPassword, templateMessage) = GetSmsConfiguration();
                }

                var client = _client.GetClient();

                #region Header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("content_type", "application/json");

                string base64Encode = Base64Encode($"{SmsUsername}:{SmsPassword}");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Encode}");
                #endregion

                #region Body

                var bodySerialized = JsonConvert.SerializeObject(body);
                var buffer = System.Text.Encoding.UTF8.GetBytes(bodySerialized);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                #endregion

                using (var response = await client.PostAsync(smsUrl, byteContent))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"{nameof(SendSMS)}: failed to Send SMS with StatusCode ${response.StatusCode}");
                        throw new HttpRequestException();
                    }
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<SmsResponse>(apiResponse);
                    accessToken = res;

                }
                return accessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool PersistSmsResponse(SmsRequest smsRequest, SmsResponse smsResponse, User? user)
        {
            SmsHistory sms = new SmsHistory
            {

                IsActive = true,
                UserId = (user == null) ? null : user.Id,
                Message = smsRequest.SmsText,
                PhoneNumber = smsRequest.Receivers,
                IsSuccessful = smsResponse.IsSuccessful,
                StatusCode = smsResponse.StatusCode,
                SmsResponseMessage = smsResponse.Message,

            };
            var res = _repository.InsertEntity(sms);
            return res;

        }

        public (string, string, string, string) GetSmsConfiguration()
        {

            var smsUrl = _configuration.GetSection("SmsSettings")["smsUrl"].Decrypt();
            var username = _configuration.GetSection("SmsSettings")["Username"].Decrypt();
            var password = _configuration.GetSection("SmsSettings")["Password"].Decrypt();
            var TemplateMessage = _configuration.GetSection("SmsSettings")["TemplateMessage"];

            return (smsUrl, username, password, TemplateMessage);
        }
        public (string, string, string, string) GetOtpSmsConfiguration()
        {

            var smsUrl = _configuration.GetSection("OtpSmsSettings")["smsUrl"].Decrypt();
            var username = _configuration.GetSection("OtpSmsSettings")["Username"].Decrypt();
            var password = _configuration.GetSection("OtpSmsSettings")["Password"].Decrypt();
            var TemplateMessage = _configuration.GetSection("OtpSmsSettings")["TemplateMessage"];

            return (smsUrl, username, password, TemplateMessage);
        }


        #region Tools

        /// <summary>
        /// Encode string  
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



    }

    public class SmsRequest
    {
        public string SmsText { get; set; }
        public string Receivers { get; set; }
        public string SenderId { get; set; } = "";
        public bool IsUnicode { get; set; } = true;
        public int IsPassword { get; set; }

    }

    public class SmsResponse
    {
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

    }



}
