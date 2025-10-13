using System;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Services
{
    public class FouladClientCredentialsService
    {
        private readonly IConfiguration _configuration;
        public FouladClientCredentialsService(IConfiguration configuration)
        {
            _configuration = configuration;

        }


        public ClientCredentials GetClientCredentials(ServiceEnum system)
        {
            var clientId = "";
            var ClientSecret = "";

            clientId = _configuration.GetSection("ClientCredentials").GetSection(system.ToString())["ClientId"];
            ClientSecret = _configuration.GetSection("ClientCredentials").GetSection(system.ToString())["ClientSecret"];
            try
            {
                clientId = AESService.Decrypt(clientId);
                ClientSecret = AESService.Decrypt(ClientSecret);
            }
            catch (Exception)
            {
                throw;
            }

            return new ClientCredentials
            {
                ClientId = clientId,
                ClientSecret = ClientSecret,
            };
        }

        public ClientUsernamePassword GetClientUsernamePassword(ServiceEnum system)
        {
            var username = "";
            var password = "";

            username = _configuration.GetSection("ClientCredentials").GetSection(system.ToString())["Username"];
            password = _configuration.GetSection("ClientCredentials").GetSection(system.ToString())["Password"];
            try
            {
                username = AESService.Decrypt(username);
                password = AESService.Decrypt(password);
            }
            catch (Exception)
            {
                throw;
            }

            return new ClientUsernamePassword
            {
                Username = username,
                Password = password
            };
        }
    }


    public class ClientCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class ClientUsernamePassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}
