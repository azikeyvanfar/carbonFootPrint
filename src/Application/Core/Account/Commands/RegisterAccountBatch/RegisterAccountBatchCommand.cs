using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ContractorBackend.Application.Core.Account.Commands.RegisterAccountBatch
{
    public class RegisterAccountBatchCommand : IRequest
    {
        public List<PersonM> Personnels { get; set; }
    }

    public class PersonM
    {
        [Required]
        public string PersonnelCode { get; set; }
        [Required]
        public string NationalCode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class RegisterAccountBatchCommandHandler : IRequestHandler<RegisterAccountBatchCommand>
    {
        private readonly UserManager<User> _userManager;


        public RegisterAccountBatchCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(RegisterAccountBatchCommand request, CancellationToken cancellationToken)
        {
            foreach (var personnel in request.Personnels)
            {
                if (_userManager.Users.Any(x => x.PersonnelCode == personnel.PersonnelCode))
                {
                    continue;
                }

                var entity = new User
                {
                    PersonnelCode = personnel.PersonnelCode,
                    UserName = personnel.PersonnelCode,
                    NationalCode = personnel.NationalCode,
                    FirstName = personnel.FirstName,
                    LastName = personnel.LastName,
                };

                var result = await _userManager.CreateAsync(entity, personnel.NationalCode + "Sa@12345");
                if (result.Succeeded)
                {
                    var roleResut = await _userManager.AddToRoleAsync(entity, "Client");
                }

            }

            return Unit.Value;
        }
    }

}
