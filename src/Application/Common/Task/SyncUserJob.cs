using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Log;
using ContractorBackend.Domain.Entities.Shared;
using ContractorBackend.Domain.Enums.Core;
using Dapper;
using DNTPersianUtils.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContractorBackend.Application.Common.Task
{
    /// <summary>
    /// سرویس همگام سازی کاربران با issuite
    /// </summary>
    public class SyncUserJob
    {
        #region Feilds

        private readonly IUserService _userService;
        private readonly HttpClientMethods _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICustomLogRepository _logRepository;
        private readonly ILogDbContext _logdbContext;
        private readonly ISmsSender _smsSender;
        public SyncUserJob(IUserService userService, HttpClientMethods httpClient, IConfiguration configuration, ICustomLogRepository logRepository, ISmsSender smsSender, ILogDbContext logdbContext)
        {
            _userService = userService;
            _httpClient = httpClient;
            _configuration = configuration;
            _logRepository = logRepository;
            _smsSender = smsSender;
            _logdbContext = logdbContext;
        }

        #endregion

        /// <summary>
        /// سرویس بروز رسانی کاربران با پارامتر تاریخ  
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<(long ModifiedUsersCount, long AddedUsersCount)> SyncUsersFromDateToNow()
        {
            /* Read me ...
             * در کوئری که به سمت issuite  ارسال می شود پارامتر تاریخ اگر ارسال شود دیتایی از تاریخ ارسالی تا به امروز دریافت خواهد شد 
             * برای دریافت دیتایی کلی پارامتر تاریخ ارسال نشود
             * 
             */

            #region requestToIssuite

            #region parameters issuite request 

            var days = _configuration["JobParams:LastFewDays"];
            int fewDays = Convert.ToInt32(days);
            //string date = DateTime.Now.AddDays(-fewDays).ToShamsiDateTime2();
            //string limit = "50000";
            //string offset = "0";

            var lst = new List<ServiceInputModel>
            {
            //      new ServiceInputModel() { ParameterName = "P_date", ParameterValue = date },
            //      new ServiceInputModel() { ParameterName = "limit", ParameterValue = limit },
            //      new ServiceInputModel() { ParameterName = "offset", ParameterValue = offset },
            };

            #endregion
            
            var queryIssuite = await _httpClient.GetService(new CpmperEmployeesVM(), IsSuiteUrlClass.dict[IsSuiteUrlKeyEnum.cpm_cpmper_employees_viw].Url, lst, ServiceEnum.CPM);

            #endregion

            var totalUsers = _userService.GetAll().GetAwaiter().GetResult();
            var listModifiedUsers = queryIssuite.Items.Where(c => totalUsers.Any(x => x.UserName == c.num_prsn_emplc)).GroupBy(x => x.num_prsn_emplc).ToList();
            var listAddedUsers = queryIssuite.Items.Where(c => !totalUsers.Any(x => x.UserName == c.num_prsn_emplc) ).GroupBy(x => x.num_prsn_emplc).ToList();

            #region در صورتی query های بالا بدرستی کار نکرد دوتا list بالا را کامنت کنید و ساخت لیست با استفاده از دستورات linq را فعال کنید
            /*
            var listModifiedUsers = (from p in queryIssuite.Items
                                     join u in totalUsers
                                     on p.num_prsn equals u.UserName
                                     group p by p.num_prsn).ToList();
            var listAddedUsers = (from p in queryIssuite.Items
                                  join u in totalUsers
                                  on p.num_prsn equals u.UserName into userGroup
                                  from u in userGroup.DefaultIfEmpty()
                                  where u == null
                                group p by p.num_prsn).ToList();
            */
            #endregion

            #region Modified Users 

            if (listModifiedUsers.Any())
            {
                foreach (var item in listModifiedUsers)
                {
                    var issuiteUserData = item.FirstOrDefault();
                    var currentUser = await _userService.SearchIssuiteUserByPerNum(issuiteUserData.num_prsn_emplc);

                    if (item.Count() == 1)
                    {
                        await _userService.UpdateUserByIssuiteData(currentUser, issuiteUserData);

                        var role = await _userService.GetRoleIdbyIssuiteRoleName(issuiteUserData.type_prsn);
                        if (role.roleName == "NotFound" || role.roleId == 0) throw new Exception("نام رول پیدا نشد");

                        // افزودن نقش به کاربر

                        var userHasRoleId = await _userService.UserHasRole(role.roleId, currentUser.Id);
                        if (!userHasRoleId)
                        {
                            var reaultAddRole = await _userService.AddUserRole(currentUser, role.roleName);
                            if (!reaultAddRole) throw new Exception($"خطا در افزودن نقش {role.roleName} به کاربر {currentUser.Id}");
                        }

                    }
                    else
                    {
                        // در این حالت کاربر چند نقش دارد

                        var listUserRoles = _userService.GetAllUserRole(currentUser.Id);
                        var roleIdTemp = new List<(long roleId, string roleName)>();

                        foreach (var data in item.Select((value, i) => new { i, value }))
                        {
                            var value = data.value;
                            var index = data.i;
                            if (index == 0)
                            {
                                var systemUser = await _userService.SearchIssuiteUserByPerNum(value.num_prsn_emplc);
                                var updateUser = await _userService.UpdateUserByIssuiteData(systemUser, issuiteUserData);
                            }
                            var role = await _userService.GetRoleIdbyIssuiteRoleName(value.type_prsn);
                            roleIdTemp.Add(role);
                        }

                        var newRoles = roleIdTemp.Where(c => listUserRoles.All(x => x.RoleId != c.roleId)).Select(x => x.roleName).ToList();

                        if (newRoles.Any())
                        {
                            await _userService.AddUserRoles(currentUser, newRoles);
                        }

                    }
                }
            }

            #endregion

            #region Added Users

            if (listAddedUsers.Any())
            {
                foreach (var item in listAddedUsers)
                {
                    var issuiteUserData = item.FirstOrDefault();

                    if (item.Count() == 1)
                    {
                        var currentUser = await _userService.AddNewUserByIssuiteUserData(issuiteUserData);

                        var role = await _userService.GetRoleIdbyIssuiteRoleName(issuiteUserData.type_prsn);
                        if (role.roleName == "NotFound" || role.roleId == 0) throw new Exception("نام رول پیدا نشد");

                        var userHasRoleId = await _userService.UserHasRole(role.roleId, currentUser.Id);
                        if (!userHasRoleId)
                        {
                            var reaultAddRole = await _userService.AddUserRole(currentUser, role.roleName);
                            if (!reaultAddRole) throw new Exception($"خطا در افزودن نقش {role.roleName} به کاربر {currentUser.Id}");
                        }
                    }
                    else
                    {
                        var userTemp = new User();
                        var rolesTemp = new List<(long roleId, string roleName)>();

                        foreach (var data in item.Select((value, i) => new { i, value }))
                        {
                            var value = data.value;
                            var index = data.i;
                            if (index == 0)
                            {
                                userTemp = await _userService.AddNewUserByIssuiteUserData(issuiteUserData);
                            }
                            var role = await _userService.GetRoleIdbyIssuiteRoleName(value.type_prsn);
                            rolesTemp.Add(role);
                        }

                        if (rolesTemp.Any())
                        {
                            var roles = rolesTemp.Select(x => x.roleName).ToList();
                            await _userService.AddUserRoles(userTemp, roles);
                        }

                    }
                }
            }

            #endregion

            return new(listModifiedUsers.Count, listAddedUsers.Count);
        }

        /// <summary>
        /// سرویس بروز رسانی کلیه کاربران با issuite
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async System.Threading.Tasks.Task SyncTotalUsers()
        {
            /* Read me ...
             * کل کاربران از issuite دریافت شده و با جدول فعلی بروز رسانی می شود
             * در اینجا پارامتر تاریخ برای issuite  ارسال نمی شود
             */

            #region requestToIssuite

            // parameters issuite request
            string limit = "50000";
            string offset = "0";

            var lst = new List<ServiceInputModel>
            {
                  new ServiceInputModel() { ParameterName = "limit", ParameterValue = limit },
                  new ServiceInputModel() { ParameterName = "offset", ParameterValue = offset },
            };

            var queryIssuite = await _httpClient.GetService(new CpmperEmployeesVM(), IsSuiteUrlClass.dict[IsSuiteUrlKeyEnum.cpm_cpmper_employees_viw].Url, lst, ServiceEnum.CPM);
            var Items = queryIssuite.Items.GroupBy(x => x.num_prsn_emplc);
            #endregion

            var totalUsers = _userService.GetAll().GetAwaiter().GetResult();
            var listModifiedUsers = queryIssuite.Items.Where(c => totalUsers.Any(x => x.UserName == c.num_prsn_emplc)).GroupBy(x => x.num_prsn_emplc).ToList();

            #region در صورتی query های بالا بدرستی کار نکرد دوتا list بالا را کامنت کنید و ساخت لیست با استفاده از دستورات linq را فعال کنید
            /*
            var listModifiedUsers = (from p in queryIssuite.Items
                                     join u in totalUsers
                                     on p.num_prsn equals u.UserName
                                     group p by p.num_prsn).ToList();
            */
            #endregion

            if (listModifiedUsers.Any())
            {
                foreach (var item in listModifiedUsers)
                {
                    var issuiteUserData = item.FirstOrDefault();
                    var currentUser = await _userService.SearchIssuiteUserByPerNum(issuiteUserData.num_prsn_emplc);

                    if (item.Count() == 1)
                    {
                        await _userService.UpdateUserByIssuiteData(currentUser, issuiteUserData);

                        var role = await _userService.GetRoleIdbyIssuiteRoleName(issuiteUserData.type_prsn);
                        if (role.roleName == "NotFound" || role.roleId == 0) throw new Exception("نام رول پیدا نشد");

                        // افزودن نقش به کاربر
                        var userHasRoleId = await _userService.UserHasRole(role.roleId, currentUser.Id);
                        if (!userHasRoleId)
                        {
                            var reaultAddRole = await _userService.AddUserRole(currentUser, role.roleName);
                            if (!reaultAddRole) throw new Exception($"خطا در افزودن نقش {role.roleName} به کاربر {currentUser.Id}");
                        }

                    }
                    else
                    {
                        // در این حالت کاربر چند نقش دارد

                        var listUserRoles = _userService.GetAllUserRole(currentUser.Id);
                        var roleIdTemp = new List<(long roleId, string roleName)>();

                        foreach (var data in item.Select((value, i) => new { i, value }))
                        {
                            var value = data.value;
                            var index = data.i;
                            if (index == 0)
                            {
                                var systemUser = await _userService.SearchIssuiteUserByPerNum(value.num_prsn_emplc);
                                var updateUser = await _userService.UpdateUserByIssuiteData(systemUser, issuiteUserData);
                            }
                            var role = await _userService.GetRoleIdbyIssuiteRoleName(value.type_prsn);
                            roleIdTemp.Add(role);
                        }

                        var newRoles = roleIdTemp.Where(c => listUserRoles.All(x => x.RoleId != c.roleId)).Select(x => x.roleName).ToList();

                        if (newRoles.Any())
                        {
                            await _userService.AddUserRoles(currentUser, newRoles);
                        }

                    }
                }
            }

        }

        /// <summary>
        /// سرویس همگام سازی با استفاده از hangfire
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task SyncUsersByHangFire()
        {
            var stopWatch = Stopwatch.StartNew();
            var cls = this.GetType();
            string Msg = $"Background Task Information :\r\nName:\t{cls.Name}\r\nNameSpace :\t{cls.Namespace}\r\nStart At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nMessage :\tشروع فرایند همگام سازی کاربران issuite با استفاده از hangfire";
            BackgroundTaskHistory log = new()
            {
                Name = "SyncUsersByHangFire",
                StartProcess = DateTime.Now.ToShortPersianDateTimeString(),
                IsSuccess = false,
                Message = Msg
            };

            try
            {
                _logRepository.AddLogTask(log).GetAwaiter().GetResult();

                var result = SyncUsersFromDateToNow().GetAwaiter().GetResult();

                stopWatch.Stop();
                log.IsSuccess = true;
                log.Message = log.Message + $"\r\nEnd At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nTotal time :\t{stopWatch.Elapsed.TotalMinutes} Minutes\r\nResult:{{\n\tModified Users Count :\t{result.ModifiedUsersCount}\n\t Added Users Count :\t{result.AddedUsersCount}\n\tعملیات همگام سازی با موفقیت به پایان رسید\n}}";
                log.EndProcess = DateTime.Now.ToShortPersianDateTimeString();
                _logRepository.UpdateLogTask(log).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                log.Message = log.Message + $"\r\nEnd At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nTotal time :\t{stopWatch.Elapsed.TotalMinutes} Minutes\r\nResult:\tخطایی رخ داده است\r\nException:\t{ex.Message}\r\nInner Exception{ex.InnerException?.Message}";
                _logRepository.UpdateLogTask(log).GetAwaiter().GetResult();
                throw;
            }
        }

        public async System.Threading.Tasks.Task CheckBackgroundTaskState()
        {
            using (var connection = new SqlConnection(AESService.Decrypt(_configuration["ConnectionStrings:DefaultConnection"])))
            {
                var FilterDate = DateTime.Now.AddDays(-1);
                var BTSState = await _logdbContext.BackgroundTaskHistories.Where(x => EF.Property<DateTime>(x, "CreatedDateTime") >= FilterDate).ToListAsync();
                var mobile = _configuration["JobParams:Phone1"];
                if (BTSState.Any(x => !x.IsSuccess))
                {
                    var job = await connection.QueryFirstOrDefaultAsync<HFJob>("SELECT * FROM [HangFire].[Job] WHERE ID = 29858");
                    if (job.StateName != "Succeeded")
                    {
                        await _smsSender.SendCustomSms(new SmsRequest { Receivers = mobile, SmsText = "سرویس بروز رسانی کاربران را بررسی کنید." }, SmsType.Notify);
                    }
                }
            }
        }

    }

    /// <summary>
    /// بک گراند سرویس سینک شدن یوزر ها هر روز ساعت 1 بامداد
    /// </summary>
    public class UserSyncTaskService : BackgroundService
    {
        #region Feilds

        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public UserSyncTaskService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* read me ...
             * در این جاب ابتدا به روز فعلی ساعت اجرا اضافه میکند اگر زمان فعلی از زمان ران شدن بعدی بزرگتر باشد 
             * یعنی از ساعت اجرایی گذشته و روز بعد باید اجرا شود به همین دلیل یک روز به آن اضافه خواهد شد
             * ابتدا شروع جاب یک لاگ در سیستم ثبت می شود و در انتها نیز یک لاگ پایان ثبت می شود البته در صورت خطا زمان پایان جاب در سیستم ثبت نمی گردد.
             */

            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddHours(23);
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var initialDelay = nextRunTime - now;

            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromDays(1));

            return System.Threading.Tasks.Task.CompletedTask;

        }

        private void DoWork(object state)
        {
            var stopWatch = Stopwatch.StartNew();

            var scope = _serviceProvider.CreateScope();
            var logService = scope.ServiceProvider.GetRequiredService<ICustomLogRepository>();
            var projectName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            #region data log
            var cls = this.GetType();
            string Msg = $"Background Task Information :\r\nName:\t{cls.Name}\r\nNameSpace :\t{cls.Namespace}\r\nStart At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nProjectName :{projectName}\r\nMessage :\tشروع فرایند همگام سازی کاربران issuite";
            #endregion

            BackgroundTaskHistory log = new()
            {
                Name = "UserSyncTaskService",
                StartProcess = DateTime.Now.ToShortPersianDateTimeString(),
                IsSuccess = false,
                Message = Msg
            };

            try
            {
                logService.AddLogTask(log).GetAwaiter().GetResult();

                //start process ---
                var userSyncService = scope.ServiceProvider.GetRequiredService<SyncUserJob>();
                var result = userSyncService.SyncUsersFromDateToNow().GetAwaiter().GetResult();


                // --- end process and submit log
                stopWatch.Stop();
                log.IsSuccess = true;
                log.Message = log.Message + $"\r\nEnd At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nTotal time :\t{stopWatch.Elapsed.TotalMinutes} Minutes\r\nResult:{{\n\tModified Users Count :\t{result.ModifiedUsersCount}\n\t Added Users Count :\t{result.AddedUsersCount}\n\tعملیات همگام سازی با موفقیت به پایان رسید\n}}";
                log.EndProcess = DateTime.Now.ToShortPersianDateTimeString();
                logService.UpdateLogTask(log).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                log.Message = log.Message + $"\r\nEnd At :\t{DateTime.Now.ToShortPersianDateTimeString()}\r\nTotal time :\t{stopWatch.Elapsed.TotalMinutes} Minutes\r\nResult:\tخطایی رخ داده است\r\nException:\t{ex.Message}\r\nInner Exception{ex.InnerException?.Message}";
                logService.UpdateLogTask(log).GetAwaiter().GetResult();
            }

        }

        public override System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }
    }

    /// <summary>
    /// بک گراند سرویس چک کردن وضعیت اجرایی جاب ها و اطلاع رسانی از طریق پیامک
    /// </summary>
    public class CheckBTSState : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public CheckBTSState(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* read me ...
             * در این جاب ابتدا به روز فعلی ساعت اجرا اضافه میکند اگر زمان فعلی از زمان ران شدن بعدی بزرگتر باشد 
             * یعنی از ساعت اجرایی گذشته و روز بعد باید اجرا شود به همین دلیل یک روز به آن اضافه خواهد شد
             */

            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddHours(11);
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var initialDelay = nextRunTime - now;

            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromDays(1));

            return System.Threading.Tasks.Task.CompletedTask;
        }
        public void DoWork(object state)
        {
            var stopWatch = Stopwatch.StartNew();
            var scope = _serviceProvider.CreateScope();
            try
            {
                //start process ---
                var userSyncService = scope.ServiceProvider.GetRequiredService<SyncUserJob>();
                userSyncService.CheckBackgroundTaskState().GetAwaiter().GetResult();

                // --- end process and submit log
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
            }
        }
        public override System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }
    }
}
