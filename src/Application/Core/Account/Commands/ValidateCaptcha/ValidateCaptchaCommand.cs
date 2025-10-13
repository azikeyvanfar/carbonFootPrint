using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Core.Account.Commands.ValidateCaptcha
{
    public class ValidateCaptchaCommand : IRequest<bool>
    {
        public string Captcha { get; set; } = null!;
        public string Key { get; set; } = null!;
    }
    public class ValidateCaptchaCommandHandler : IRequestHandler<ValidateCaptchaCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IRepository<Captcha> _repository;
        public ValidateCaptchaCommandHandler(IApplicationDbContext dbContext, IRepository<Captcha> repository)
        {

            _dbContext = dbContext;
            _repository = repository;

        }
        public async Task<bool> Handle(ValidateCaptchaCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var captcha = await _dbContext.Captchas
                .FirstOrDefaultAsync(x =>
                        x.FinalCode == request.Captcha &&
                        x.Key == request.Key
                         && x.CreatedDate.AddMinutes(5) > now
                        ,
                        cancellationToken: cancellationToken);

            if (captcha == null)
            {
                return false;
            }

            _repository.Delete(captcha);

            return true;

        }
    }
}
