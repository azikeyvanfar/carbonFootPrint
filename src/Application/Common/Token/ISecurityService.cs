using System;

namespace ContractorBackend.Application.Common.Token
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
        Guid CreateCryptographicallySecureGuid();

        string EncryptString(string key, string plainText);
        string DecryptString(string key, string cipherText);
    }
}