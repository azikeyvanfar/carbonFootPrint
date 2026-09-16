using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Account.Commands.UpdateAccount
{
    public class UpdateAccountCommand : IRequest
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonnelCode { get; set; }
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ایدی کاربر فولاد در صورت انتخاب
        /// در صورت پر بودن فیلد های زیر خالی حساب می شود
        ///  FirstName - LastName - PersonnelCode - NationalCode 
        /// </summary>
        public Guid? EmployeeId { get; set; }

        public bool IsActive { get; set; } = true;

    }

    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _dbContext;

        public UpdateAccountCommandHandler(UserManager<User> userManager, IApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        //public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        //{
        //    var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
        //    ArgumentNullException.ThrowIfNull(user);

        //    if (request.EmployeeId is not null)
        //    {
        //        //var employee = await _dbContext.Employees.FirstOrDefaultAsync(c => c.Id == request.EmployeeId, cancellationToken);
        //        //  ArgumentNullException.ThrowIfNull(employee);


        //        //user.PersonnelCode = employee.PersonnelCode.Value.ToString();
        //        //user.FirstName     = employee.FirstName;
        //        //user.LastName      = employee.LastName;
        //        //user.NationalCode  = employee.NationalCode.Value.ToString();
        //        //user.BirthDate     = employee.BirthDate;
        //        //user.PhoneNumber   = !string.IsNullOrWhiteSpace(request.PhoneNumber) ?
        //        //                            request.PhoneNumber :
        //        //                            employee.PhoneNumber.ToString();
        //        //user.IsActive      = request.IsActive;
        //        //user.EmployeeId    = request.EmployeeId;
        //        //user.UserName      = employee.ContractNumber !=null ? employee.PersonnelCode.Value.ToString() : employee.NationalCode.Value.ToString();


        //    }
        //    else
        //    {
        //        user.PersonnelCode = request.PersonnelCode;
        //        user.FirstName = request.FirstName;
        //        user.LastName = request.LastName;
        //        user.NationalCode = request.NationalCode;
        //        user.IsActive = request.IsActive;
        //        user.EmployeeId = request.EmployeeId;
        //        user.PhoneNumber = request.PhoneNumber;
        //        user.UserName = !string.IsNullOrWhiteSpace(request.PersonnelCode) ? request.PersonnelCode : request.NationalCode;

        //    }

        //    var result = await _userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        throw new CustomException("Create User Failed", result.Errors); // JsonConvert.SerializeObject(result.Errors).ToString()
        //    }

        //    return Unit.Value;
        //}

        async Task IRequestHandler<UpdateAccountCommand>.Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.Id);
            ArgumentNullException.ThrowIfNull(user);

            if (request.EmployeeId is not null)
            {
                //var employee = await _dbContext.Employees.FirstOrDefaultAsync(c => c.Id == request.EmployeeId, cancellationToken);
                //  ArgumentNullException.ThrowIfNull(employee);


                //user.PersonnelCode = employee.PersonnelCode.Value.ToString();
                //user.FirstName     = employee.FirstName;
                //user.LastName      = employee.LastName;
                //user.NationalCode  = employee.NationalCode.Value.ToString();
                //user.BirthDate     = employee.BirthDate;
                //user.PhoneNumber   = !string.IsNullOrWhiteSpace(request.PhoneNumber) ?
                //                            request.PhoneNumber :
                //                            employee.PhoneNumber.ToString();
                //user.IsActive      = request.IsActive;
                //user.EmployeeId    = request.EmployeeId;
                //user.UserName      = employee.ContractNumber !=null ? employee.PersonnelCode.Value.ToString() : employee.NationalCode.Value.ToString();


            }
            else
            {
                user.PersonnelCode = request.PersonnelCode;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.NationalCode = request.NationalCode;
                user.IsActive = request.IsActive;
                user.EmployeeId = request.EmployeeId;
                user.PhoneNumber = request.PhoneNumber;
                user.UserName = !string.IsNullOrWhiteSpace(request.PersonnelCode) ? request.PersonnelCode : request.NationalCode;

            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new CustomException("Create User Failed", result.Errors); // JsonConvert.SerializeObject(result.Errors).ToString()
            }
        }
    }
}
