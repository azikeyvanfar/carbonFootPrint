using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Account.Commands.UpdateAllUsersFromIsSuite
{
    public class UpdateAllUsersFromIsSuiteCommand : IRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public List<string> PersonnelCodes { get; set; }

    }

    public class UpdateAllUsersFromIsSuiteCommandHandler : IRequestHandler<UpdateAllUsersFromIsSuiteCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;
        private readonly IUserService _userService;

        public UpdateAllUsersFromIsSuiteCommandHandler(
            //IAntiForgeryCookieService antiForgery,
            IApplicationDbContext dbContext,
            IApplicationUserManager userManager,
            IUserService userService
            )
        {
            _dbContext = dbContext;
            //_antiForgery = antiForgery;
            _userManager = userManager;
            _userService = userService;
        }


        public async Task<Unit> Handle(UpdateAllUsersFromIsSuiteCommand request, CancellationToken cancellationToken)
        {
            if (request.PersonnelCodes != null && request.PersonnelCodes.Any())
            {
                var users = _dbContext.Set<User>().AsNoTracking()//.Where(x => x.IsActive)
                    .Where(x => request.PersonnelCodes.Any(l => l == x.PersonnelCode)).ToList();
                foreach (var user in users)
                {
                    try
                    {
                        await _userService.UpdateAllUserInfoFromIsSuite(user.Id);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + " inner " + e.InnerException?.Message ?? "");
                    }
                }
            }
            else
            {


                var users = _dbContext.Set<User>().AsNoTracking()//.Where(x => x.IsActive)
                                                                 .Skip(request.Skip).Take(request.Take).ToList();


                foreach (var user in users)
                {
                    try
                    {
                        await _userService.UpdateAllUserInfoFromIsSuite(user.Id);

                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + " inner " + e.InnerException?.Message ?? "");
                    }
                }
            }

            return Unit.Value;
        }
    }

}
















