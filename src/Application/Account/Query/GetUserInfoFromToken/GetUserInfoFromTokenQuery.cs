using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Account.Query.GetUserInfoFromToken
{
    public class GetUserInfoFromTokenQuery : IRequest<UserMinimalDto>
    {

    }

    public class GetUserInfoFromTokenQueryHandler :
        IRequestHandler<GetUserInfoFromTokenQuery, UserMinimalDto>
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        //private readonly IRepository<UserDetail> _userDetail;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationDbContext _dbContext;

        public GetUserInfoFromTokenQueryHandler(
            IApplicationUserManager userManager,
            //IRepository<UserDetail> userDetail,
            IApplicationRoleManager roleManager,
            IHttpContextAccessor accessor,
            IApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _accessor = accessor;
            //_userDetail = userDetail;
            _roleManager = roleManager;
            _dbContext = dbContext;

        }
        public async Task<UserMinimalDto> Handle(GetUserInfoFromTokenQuery request, CancellationToken cancellationToken)
        {

            List<AccessiblePageRouteClaim> accessiblePageRouteClaims = new List<AccessiblePageRouteClaim>();
            var userId = _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            List<string> roleType = new List<string>();

            foreach (var item in roles)
            {
                var roleId = Convert.ToInt64(item);
                var role = await _roleManager.FindByIdAsync(item);
                roleType.Add(role.Name);
            }

            //var employee = _dbContext.Employees.AsNoTracking().Where(t => t.IsActive && t.UserId == user.Id).FirstOrDefault();

            #region حالت استخدام
            string employmentTypeVal = "";
            string employmentTypeFarsi = "";
            //var employmentTypeCode = user.UserDetails?.CodRecuitmentType;
            //if (!string.IsNullOrWhiteSpace(employmentTypeCode))
            //{
            //    var employmentType = _dbContext.EmploymentTypes.FirstOrDefault(x => x.val == employmentTypeCode);
            //    employmentTypeVal = employmentType.val;
            //    employmentTypeFarsi = employmentType.farsi;
            //}
            #endregion

            return new UserMinimalDto
            {
                FirstNameEng = user.FirstNameEng,
                LastNameEng = user.LastNameEng,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roleType,
                IsPasswordChangeForce = user.IsPasswordChangeForce,
                //PasswordChangeForceMsg = userDet.PasswordChangeForceMsg,
                //SeenProfileGuide = userDet.SeenProfileGuide == true ? true : false,
                //PersonnelCode = user.PersonnelCode,

                //////ProfileImage = (employee == null || employee.ProfileImage == null) ? "" : Convert.ToBase64String(employee?.ProfileImage),

                //ProfileImage = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) :
                //    "data:image/jpeg;base64,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",
                Id = user.Id,
                //ServiceProviderId = user.ServiceProviderUsers.Any() ? user.ServiceProviderUsers.First().ServiceProviderId : System.Guid.Empty,
                //ServiceProviderName = user.ServiceProviderUsers.Any() ? user.ServiceProviderUsers.First().ServiceProvider.Name : string.Empty,
                //RoleNames = roles.ToList(),
                //RoleIds = roleIds
                EmploymentTypeVal = employmentTypeVal,
                EmploymentTypeFarsi = employmentTypeVal
            };
        }
    }
}
