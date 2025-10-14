using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Log;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ContractorBackend.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _services;
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ICustomLogRepository _logRepository;
        private readonly IApplicationRoleManager _roleManager;


        public UserService(
            IServiceProvider services,
            IApplicationDbContext dbContext,
            UserManager<User> userManager,
            IApplicationRoleManager roleManager,
            ICustomLogRepository logRepository)
        {
            _services = services;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logRepository = logRepository;
        }

        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    // await UpdateUserFromIsSuiteAfter90Days()
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

        private DbSet<User> _entities { get; set; }
        public DbSet<User> Entites
        {
            get
            {
                if (_entities is null)
                {
                    _entities = _dbContext.Set<User>();
                }
                return _entities;
            }
        }
        private DbSet<UserRole> _userRoles { get; set; }
        private DbSet<UserRole> UserRoles
        {
            get
            {
                if (_userRoles == null)
                {
                    _userRoles = _dbContext.Set<UserRole>();
                }
                return _userRoles;
            }
        }
        public IQueryable<User> Table => Entites;
        public IQueryable<User> TableAsNoTracking => Entites.AsNoTracking();
        public IQueryable<UserRole> TableUserRoles => UserRoles;
        public async Task<bool> UpdateAllUserInfoFromIsSuite(long userId)
        {
            using (var serviceScope = _services.CreateScope())
            {

                var _accessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var _isSuitHttp = serviceScope.ServiceProvider.GetRequiredService<IsSuiteClientService>();
                var _repository = serviceScope.ServiceProvider.GetRequiredService<IRepository<User>>();
                //  var _repositoryEmployee = serviceScope.ServiceProvider.GetRequiredService<IRepository<Employee>>();
                var _mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

                var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                //  var employee = _repositoryEmployee.GetAll().FirstOrDefault(x => x.UserId == userId);

                var queryParams = new List<QueryParamModel>() { new QueryParamModel { ParameterName = "P_NUM_PRSN", ParameterValue = user.PersonnelCode } };

                //var isResult = await _isSuitHttp.GetAllEmployeeViewAsync(queryParams);
                //if (isResult.Items is null || isResult.Items.Count <= 0 || isResult.Items[0] is null)
                //{
                //    return false;
                //}
                var resUpdateEmployee = false;
                //if (employee is null)
                //{
                //    employee = _mapper.Map<Employee>(isResult.Items[0]);
                //    employee.UserId = user.Id;
                //    resUpdateEmployee = _repositoryEmployee.InsertEntity(employee);
                //}
                //else
                //{
                //    _mapper.Map(isResult.Items[0], employee);
                //    _mapper.Map(isResult.Items[0], user);
                //    user.Employee = employee;
                //}
                var resUpdate = _repository.UpdateEntity(user);
                return resUpdate & resUpdateEmployee;
            }
        }


        public async Task AddUserRoles(User user, List<string> roleNames)
        {
            foreach (var role in roleNames)
            {
                var result = await AddUserRole(user, role);
                if (!result) throw new Exception($"خطا در افزودن نقش {role} به کاربر {user.Id}");
            }
        }

        public async Task<bool> CheckExistUserByPersonnelCode(string personnelCode, CancellationToken cancellationToken)
        {
            return await _userManager.Users.AnyAsync(x => x.PersonnelCode == personnelCode, cancellationToken);
        }

        public IQueryable<LookupItemDto> GetCurrentUserScopes()
        {
            string code = Utilities.ScopeCode;

            //var query = _lookupRepository.GetAllItemQuery;
            var query = _dbContext.Lookups.Include(x => x.Parent).Where(x => x.Type == LookupType.Item).OrderBy(x => x.Priority).AsQueryable();
            query = query.Where(x => x.Code == code);
            var userScopeIds = _roleManager.GetRoleScopeIdsforCurrentUser().Select(x => x.Value);
            var query1 = query.ToList().Where(x => userScopeIds.Contains(x.Id)).Select(x => new LookupItemDto
            {
                Id = x.Id,
                EnName = x.EnName,
                FaName = x.FaName,
                Code = x.Code,
                ListName = _dbContext.Lookups.FirstOrDefault(p => p.Type == LookupType.Lookup && p.EnName.ToLower() == x.Code.ToLower()).FaName,
                ParentId = (x.ParentId == null) ? null : x.ParentId,
                ParentName = (x.ParentId == null) ? null : x.Parent.FaName,
                ParentListName = (x.ParentId == null) ? null : _dbContext.Lookups.FirstOrDefault(p => p.Type == LookupType.Lookup && p.EnName.ToLower() == x.Parent.Code.ToLower()).FaName,
                Priority = (x.Priority == null) ? 1 : x.Priority.Value

            }).AsQueryable();
            return query1;
        }

        public bool IsInCurrentUserScopes(Guid scopeId)
        {
            var query = GetCurrentUserScopes();

            var hasScope = query.Any(x => x.Id == scopeId);

            return hasScope;
        }


        [Display(Name = "جست و جو کاربر issute بر اساس پرسنلی")]
        public async Task<User> SearchIssuiteUserByPerNum(string perNum)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.UserName == perNum);
            return user;
        }

        [Display(Name = "لیست کلیه کاربران")]
        public async Task<IEnumerable<User>> GetAll()
        {
            return await TableAsNoTracking.ToListAsync();
        }

        [Display(Name = "بروز رسانی کاربر در سیستم بر اساس اطلاعات issuite")]
        public async Task<User> UpdateUserByIssuiteData(User user, ContractorDto issuiteUser)
        {
            if (user != null)
            {
                try
                {
                    user.FirstName = issuiteUser.nam_first_emplc;
                    user.LastName = issuiteUser.nam_last_emplc;
                    user.MobileNumber = "0" + issuiteUser.num_mobil_emplc;
                    user.PhoneNumber = "0" + issuiteUser.num_mobil_emplc;
                    user.NationalCode = issuiteUser.num_national_emplc ?? null;

                    //user.IsActive = (issuiteUser.flg == "2") ? false : true;
                    if (!string.IsNullOrWhiteSpace(issuiteUser.lkp_cod_sex_emplc))
                        user.Gender = issuiteUser.lkp_cod_sex_emplc.Trim().Equals("1") ? true : false;

                    if (issuiteUser.type_prsn== "1")
                    {
                        user.NationalCode = issuiteUser?.num_national_emplc ?? null;
                        user.PersonnelCode = issuiteUser?.num_prsn_emplc ?? null; 
                        user.UserType = UserType.Contractor;
                    }
                    else
                    {
                        user.NationalCode = issuiteUser?.num_national_emplc ?? null;
                        user.PersonnelCode = issuiteUser?.num_prsn_emplc ?? null;
                        user.UserType = UserType.Company;
                    }


                    await UpdateUser(user);

                    await _logRepository.AddLogEvent(EventSyncType.Modified_Action, EventStatus.Success, $"اطلاعات کاربر به اطلاعات زیر آپدیت گردید . \n" +
                                           $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}");

                    return user;
                }
                catch (Exception ex)
                {
                    await _logRepository.AddLogEvent(EventSyncType.Modified_Action, EventStatus.Feild, $"عملیات بروز رسانی اطلاعات کاربر با خطا مواجه شد. \n" +
                        $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}\nException:\t{ex.Message}");

                }


            }

            return null;
        }

        [Display(Name = "بروز رسانی کاربر")]
        public async Task<User> UpdateUser(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            _dbContext.Set<User>().Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        [Display(Name = "جست وجو role براساس نام نقش issuite")]
        public async Task<(long roleId, string roleName)> GetRoleIdbyIssuiteRoleName(string issuiteRole)
        {
            switch (issuiteRole)
            {
                //مدرس خارجی
                case "EX":
                    return new(273, "ExternalTeacher");

                //مدرس داخلی
                case "IN":
                    return new(272, "InternalTeacher");

                //تسهیلگران آموزش
                case "ASSESOR":
                    return (270, "EducationFacilitate");

                //هماهنگ کننده
                case "CORDINATOR":
                    return (268, "Coordinator");

                //فراگیران
                case "PRSN":
                    return new(267, "Learner");

                //موسسات
                case "INSTITUTE":
                    return new(275, "Institutes");

                //مسئولین ارزیابی عملکرد
                case "EVLMNG":
                    return new(262, "Evaluate");

                //مسئول دوره-رابط دوره
                case "MEDIATOR":
                    return new(271, "Mediator");

                //ذینفعان
                case "STSFACE":
                    return new(292, "StsFace");

                // default            
                default:
                    return new(0, "NotFound");
            }
        }

        [Display(Name = "آیا کاربر role را دارد")]
        public async Task<bool> UserHasRole(long roleId, long userId)
        {
            var result = await TableUserRoles.AnyAsync(x => x.RoleId == roleId && x.UserId == userId);
            return result;
        }

        [Display(Name = "افزودن نقش به کاربر")]
        public async Task<bool> AddUserRole(User user, string roleName)
        {
            try
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    /* log successful result to comment
                     * await _logRepository.AddLogEvent(EventSyncType.Added_Function, EventStatus.Success, $"نقش کاربر با موفقیت تخصیص داده شد . \n" +
                             $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}\nRole:\t{roleName}"); */

                    return true;
                }

                await _logRepository.AddLogEvent(EventSyncType.Added_Function, EventStatus.Feild, $"نقش کاربر با خطا مواجه شده است . \n" +
                            $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}\nRole:\t{roleName}");
                return false;

            }
            catch (Exception)
            {
                await _logRepository.AddLogEvent(EventSyncType.Added_Function, EventStatus.Feild, $"نقش کاربر با خطا مواجه شده است . \n" +
                        $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}\nRole:\t{roleName}");
                return false;
            }
        }

        public IQueryable<UserRole> GetAllUserRole(long userId)
        {
            return TableUserRoles.Where(x => x.UserId == userId).AsQueryable();
        }
        [Display(Name = "افزودن کاربر جدید با اطلاعات issuite")]
        public async Task<User> AddNewUserByIssuiteUserData(ContractorDto issuiteUserData)
        {
            try
            {
                var user = new User();

                user.UserName = issuiteUserData.num_prsn_emplc;
                user.FirstName = issuiteUserData.nam_first_emplc ?? null;
                user.LastName = issuiteUserData.nam_last_emplc ?? null;
                user.MobileNumber = "0" + issuiteUserData.num_mobil_emplc;
                user.PhoneNumber = "0" + issuiteUserData.num_mobil_emplc;

                if (!string.IsNullOrWhiteSpace(issuiteUserData.lkp_cod_sex_emplc))
                    user.Gender = issuiteUserData.lkp_cod_sex_emplc.Trim().Equals("1") ? true : false;

                if (issuiteUserData.type_prsn== "1")
                {
                    user.NationalCode = issuiteUserData.num_national_emplc ?? null;
                    user.PersonnelCode = issuiteUserData?.num_prsn_emplc ?? null;
                    user.UserType = UserType.Contractor;
                }
                else
                {
                    user.NationalCode = issuiteUserData.num_national_emplc ?? null;
                    user.PersonnelCode = issuiteUserData.num_prsn_emplc ?? null;
                    user.UserType = UserType.Company;
                }

                user.IsActive = true;
                user.IsPasswordChangeForce = true;
                var pwdf = AESService.Decrypt("h7Nk5QGr7T6pFbrhFi6d7w==");
                var result = await _userManager.CreateAsync(user, pwdf);

                if (!result.Succeeded) throw new Exception($"خطا در افزودن کاربر {issuiteUserData.nam_first_emplc + " " + issuiteUserData.nam_last_emplc} با پرسنلی / کد ملی {issuiteUserData.num_mobil_emplc} ");


                await _logRepository.AddLogEvent(EventSyncType.Added_Function, EventStatus.Success, $"کاربر با موفقیت ایجاد گردید . \n" +
                                                $"Id:\t{user.Id}\nنام:\t{user.FirstName + " " + user.LastName}\nUserName:\t{user.UserName}");


                return user;
            }
            catch (Exception ex)
            {

                await _logRepository.AddLogEvent(EventSyncType.Added_Function, EventStatus.Feild, $"عملیات تعریف کاربر با خطا مواجه گردید. \n" +
                                                $"نام:\t{issuiteUserData.nam_first_emplc + " " + issuiteUserData.nam_last_emplc}\nUserName:\t{issuiteUserData.num_prsn_emplc}\nException:\t{ex.Message}");

                return null;
            }


        }

    }
}
