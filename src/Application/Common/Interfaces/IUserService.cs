using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Core;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IUserService
    {
        //Task<bool> UpdateUserFromIsSuiteAfterThresholdTime(long userId);

        Task<bool> UpdateAllUserInfoFromIsSuite(long userId);


        Task<bool> CheckExistUserByPersonnelCode(string personnelCode, CancellationToken cancellationToken);

        IQueryable<LookupItemDto> GetCurrentUserScopes();

        bool IsInCurrentUserScopes(Guid scopeId);



    }
}
