using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Identity
{
    public interface IEmailSender
    {
        #region BaseClass

        System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string message);

        #endregion

        #region CustomMethods

        System.Threading.Tasks.Task SendEmailAsync<T>(string email, string subject, string viewNameOrPath, T model);

        #endregion
    }
}