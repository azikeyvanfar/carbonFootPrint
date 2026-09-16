using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Core.UserRole.Commands.CreateUserRole
{
    public class CreateUserRoleCommand : IRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public List<long> RoleIdList { get; set; }

    }
    public class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommand>
    {
        private readonly IRepository<Domain.Entities.Identity.UserRole> _repository;
        private readonly IRepository<User> _userRepo;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<User> _userManager;

        public CreateUserRoleCommandHandler(
            IRepository<Domain.Entities.Identity.UserRole> repository,
            IRepository<User> userRepo,
            IStringLocalizer<SharedResource> localizer,
            UserManager<User> userManager
            )
        {
            _repository = repository;
            _userRepo = userRepo;
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<Unit> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var extUrlRoleList = _repository.GetAll().Where(c => c.UserId == request.UserId).ToList();
            var deleteList = new List<Domain.Entities.Identity.UserRole>();
            User user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                throw new CustomException(_localizer["UserNotFound"]);
            }



            if (extUrlRoleList.Any(_ => !request.RoleIdList.Any(x => x == _.RoleId)))
            {
                deleteList = extUrlRoleList.Where(x => !request.RoleIdList.Any(c => c == x.RoleId)).ToList();
                _repository.DeleteRange(deleteList);
            }
            var remainedList = extUrlRoleList.Except(deleteList).ToList();
            var insertList = request.RoleIdList.Where(x => !remainedList.Any(c => c.RoleId == x)).ToList();
            foreach (var item in insertList)
            {
                _repository.Insert(new Domain.Entities.Identity.UserRole { RoleId = item, UserId = request.UserId });
            }

            return await Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<CreateUserRoleCommand>.Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}