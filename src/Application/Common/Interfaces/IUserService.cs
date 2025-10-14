using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> SearchIssuiteUserByPerNum(string perNum);
        Task<User> UpdateUserByIssuiteData(User user, ContractorDto issuiteUser);
        Task<(long roleId, string roleName)> GetRoleIdbyIssuiteRoleName(string issuiteRole);
        Task<bool> UserHasRole(long roleId, long userId);
        Task<bool> AddUserRole(User user, string roleName);
        System.Threading.Tasks.Task AddUserRoles(User user, List<string> roleNames);
        IQueryable<UserRole> TableUserRoles { get; }
        IQueryable<UserRole> GetAllUserRole(long userId);
        Task<User> AddNewUserByIssuiteUserData(ContractorDto issuiteUserData);

        //Task<bool> UpdateUserFromIsSuiteAfterThresholdTime(long userId);

        Task<bool> UpdateAllUserInfoFromIsSuite(long userId);


        Task<bool> CheckExistUserByPersonnelCode(string personnelCode, CancellationToken cancellationToken);

        IQueryable<LookupItemDto> GetCurrentUserScopes();

        bool IsInCurrentUserScopes(Guid scopeId);



    }
}
