using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CheckExistUsername(string username, CancellationToken cancellationToken);
        Task<bool> CheckExistPhoneNumber(string phoneNumber, CancellationToken cancellationToken);
        bool IsValidNationalCode(string nationalCode);
        bool IsValidPhoneNumber(string phoneNumber);
        bool ExistsEmployeePhoneNumber(Guid employeeId);

    }
}
