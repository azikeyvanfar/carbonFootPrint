using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Interfaces.Shared;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Services;
using ContractorBackend.Common.Models;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ContractorBackend.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _services;
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationUserManager _appUserManager;
        private readonly IConfiguration _configuration;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        private readonly ILookupRepository _lookupRepository;
        private readonly IApplicationRoleManager _roleManager;


        public UserService(
            IServiceProvider services,
            IApplicationDbContext dbContext,
            UserManager<User> userManager,
            IApplicationUserManager appUserManager,
            IConfiguration configuration,
            ISender mediator,
            ILookupRepository lookupRepository,
            IApplicationRoleManager roleManager)
        {
            _services = services;
            _dbContext = dbContext;
            _userManager = userManager;
            _appUserManager = appUserManager;
            _configuration = configuration;
            _mediator = mediator;
            _lookupRepository = lookupRepository;
            _roleManager = roleManager;
        }

        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    // await UpdateUserFromIsSuiteAfter90Days()
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

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

                var isResult = await _isSuitHttp.GetAllEmployeeViewAsync(queryParams);
                if (isResult.Items is null || isResult.Items.Count <= 0 || isResult.Items[0] is null)
                {
                    return false;
                }
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


        //public async Task<bool> UpdateUserFromIsSuiteAfterThresholdTime(long userId)
        //{
        //    using (var serviceScope = _services.CreateScope())
        //    {

        //        var _accessor = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        //        var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
        //        var _isSuitHttp = serviceScope.ServiceProvider.GetRequiredService<IsSuiteClientService>();
        //        var _repository = serviceScope.ServiceProvider.GetRequiredService<IRepository<User>>();
        //        var _repositoryEmployee = serviceScope.ServiceProvider.GetRequiredService<IRepository<Employee>>();
        //        var _mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

        //        var thresholdDays = Convert.ToDouble(_configuration.GetSection("thresholdDays").Value);
        //        var user = _userManager.Users.Include(t => t.Employee).FirstOrDefault(x => x.Id == userId);
        //        var employee = _repositoryEmployee.GetAll().FirstOrDefault(x => x.UserId == userId);

        //        var now = DateTimeOffset.Now;
        //        var thresholdDatetime = now.AddDays(-thresholdDays);

        //        if (user.LastLoggedIn is not null && user.LastLoggedIn > thresholdDatetime)
        //        {
        //            return true;
        //        }
        //        var queryParams = new List<QueryParamModel>() { new QueryParamModel { ParameterName = "P_NUM_PRSN", ParameterValue = user.PersonnelCode } };

        //        var isResult = await _isSuitHttp.GetAllEmployeeViewAsync(queryParams);
        //        if (isResult.Items is null || isResult.Items.Count <= 0 || isResult.Items[0] is null)
        //        {
        //            return false;
        //        }
        //        var resUpdateDetail = false;
        //        if (employee is null)
        //        {
        //            employee = _mapper.Map<Employee>(isResult.Items[0]);
        //            employee.UserId = user.Id;
        //            resUpdateDetail = _repositoryEmployee.InsertEntity(employee);
        //        }
        //        else
        //        {
        //            _mapper.Map(isResult.Items[0], employee);
        //            //_mapper.Map(isResult.Items[0], user);
        //            user.Employee = employee;
        //        }
        //        var resUpdate = _repository.UpdateEntity(user);
        //        return resUpdate & resUpdateDetail;
        //    }
        //}


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
    }
}
