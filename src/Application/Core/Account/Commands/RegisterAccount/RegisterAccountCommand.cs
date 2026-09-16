using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.Account.Commands.RegisterAccount
{
    public class RegisterAccountCommand : IRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonnelCode { get; set; }
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// ایدی کاربر فولاد در صورت انتخاب
        /// در صورت پر بودن فیلد های زیر خالی حساب می شود
        ///  FirstName - LastName - PersonnelCode - NationalCode 
        /// </summary>
        public Guid? EmployeeId { get; set; }



        public bool IsActive { get; set; } = true;

    }

    public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;
        private readonly IAccountService _accountService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RegisterAccountCommandHandler(UserManager<User> userManager, IMapper mapper, IApplicationDbContext dbContext, IAccountService accountService, IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _accountService = accountService;
            _localizer = localizer;
        }

        //public async Task<Unit> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        //{
        //    var user = new User();
        //    if (request.EmployeeId is not null)
        //    {
        //        // var employee = await _dbContext.Employees.FirstOrDefaultAsync(c => c.Id == request.EmployeeId, cancellationToken);
        //        //ArgumentNullException.ThrowIfNull(employee);

        //        //user = new User
        //        //{
        //        //    PersonnelCode = employee.PersonnelCode.Value.ToString(),
        //        //    FirstName     = employee.FirstName,
        //        //    LastName      = employee.LastName,
        //        //    NationalCode  = employee.NationalCode.Value.ToString(),
        //        //    BirthDate     = employee.BirthDate,
        //        //    PhoneNumber   = !string.IsNullOrWhiteSpace(request.PhoneNumber) ?
        //        //                                request.PhoneNumber :
        //        //                                employee.PhoneNumber.ToString(),
        //        //    IsActive      = request.IsActive,
        //        //    EmployeeId    = request.EmployeeId,
        //        //    UserName      = employee.ContractNumber == null ? employee.PersonnelCode.Value.ToString() : employee.NationalCode.Value.ToString()
        //        //};
        //    }
        //    else
        //    {
        //        user = new User
        //        {
        //            PersonnelCode = request.PersonnelCode,
        //            FirstName = request.FirstName,
        //            LastName = request.LastName,
        //            NationalCode = request.NationalCode,
        //            IsActive = request.IsActive,
        //            EmployeeId = request.EmployeeId,
        //            PhoneNumber = request.PhoneNumber,
        //            UserName = !string.IsNullOrWhiteSpace(request.NationalCode) ? request.NationalCode : request.PersonnelCode
        //        };
        //    }

        //    var resValidate = await _accountService.CheckExistUsername(user.UserName, cancellationToken);
        //    if (resValidate)
        //    {
        //        throw new CustomException(_localizer["UserNameAlreadyExists"]);
        //    }

        //    var result = await _userManager.CreateAsync(user, request.Password);
        //    if (!result.Succeeded)
        //    {
        //        throw new CustomException(_localizer["CreateuserFailed"], result.Errors); // JsonConvert.SerializeObject(result.Errors).ToString()
        //    }

        //    // link created User to Employee
        //    //var empl = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId, cancellationToken);
        //    //if (empl is not null)
        //    //{
        //    //    var addedUser = await _userManager.FindByNameAsync(user.UserName);
        //    //    empl.UserId =  addedUser.Id;
        //    //    _dbContext.Employees.Update(empl);
        //    //    await _dbContext.SaveChangesAsync(cancellationToken);
        //    //}

        //    return Unit.Value;
        //}

        async Task IRequestHandler<RegisterAccountCommand>.Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            if (request.EmployeeId is not null)
            {
                // var employee = await _dbContext.Employees.FirstOrDefaultAsync(c => c.Id == request.EmployeeId, cancellationToken);
                //ArgumentNullException.ThrowIfNull(employee);

                //user = new User
                //{
                //    PersonnelCode = employee.PersonnelCode.Value.ToString(),
                //    FirstName     = employee.FirstName,
                //    LastName      = employee.LastName,
                //    NationalCode  = employee.NationalCode.Value.ToString(),
                //    BirthDate     = employee.BirthDate,
                //    PhoneNumber   = !string.IsNullOrWhiteSpace(request.PhoneNumber) ?
                //                                request.PhoneNumber :
                //                                employee.PhoneNumber.ToString(),
                //    IsActive      = request.IsActive,
                //    EmployeeId    = request.EmployeeId,
                //    UserName      = employee.ContractNumber == null ? employee.PersonnelCode.Value.ToString() : employee.NationalCode.Value.ToString()
                //};
            }
            else
            {
                user = new User
                {
                    PersonnelCode = request.PersonnelCode,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    NationalCode = request.NationalCode,
                    IsActive = request.IsActive,
                    EmployeeId = request.EmployeeId,
                    PhoneNumber = request.PhoneNumber,
                    UserName = !string.IsNullOrWhiteSpace(request.NationalCode) ? request.NationalCode : request.PersonnelCode
                };
            }

            var resValidate = await _accountService.CheckExistUsername(user.UserName, cancellationToken);
            if (resValidate)
            {
                throw new CustomException(_localizer["UserNameAlreadyExists"]);
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new CustomException(_localizer["CreateuserFailed"], result.Errors); // JsonConvert.SerializeObject(result.Errors).ToString()
            }

            // link created User to Employee
            //var empl = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId, cancellationToken);
            //if (empl is not null)
            //{
            //    var addedUser = await _userManager.FindByNameAsync(user.UserName);
            //    empl.UserId =  addedUser.Id;
            //    _dbContext.Employees.Update(empl);
            //    await _dbContext.SaveChangesAsync(cancellationToken);
            //}

        }
    }
}
