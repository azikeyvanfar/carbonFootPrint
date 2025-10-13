using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;
        public AccountService(UserManager<User> userManager, IApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<bool> CheckExistUsername(string username, CancellationToken cancellationToken) => await _userManager.Users.AnyAsync(x => x.UserName == username, cancellationToken);
        public async Task<bool> CheckExistPhoneNumber(string phoneNumber, CancellationToken cancellationToken) => await _userManager.Users.AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

        public bool IsValidNationalCode(string nationalCode)
        {
            var result = nationalCode.IsValidIranianNationalCode();
            return result;
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.IsValidIranianMobileNumber();
        }
        public bool ExistsEmployeePhoneNumber(Guid employeeId)
        {
            //var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == employeeId);

            //if (employee == null)
            //{
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(employee.PhoneNumber.ToString()))
            //{
            //    return false;
            //}
            return true;
        }


    }
}
