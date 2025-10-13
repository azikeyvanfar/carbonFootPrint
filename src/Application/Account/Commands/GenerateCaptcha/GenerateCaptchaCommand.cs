using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Account.Commands.GenerateCaptcha
{
    public class GenerateCaptchaCommand : IRequest<CaptchaDto>
    {
    }

    public class GenerateCaptchaCommandHandler : IRequestHandler<GenerateCaptchaCommand, CaptchaDto>
    {
        private readonly ICaptchaService _captchaService;
        private readonly IRepository<Captcha> _repository;
        public GenerateCaptchaCommandHandler(ICaptchaService captchaService, IRepository<Captcha> repository)
        {
            _captchaService = captchaService;
            _repository = repository;

        }
        public Task<CaptchaDto> Handle(GenerateCaptchaCommand request, CancellationToken cancellationToken)
        {
            var generateCaptchaCode = _captchaService.GenerateCaptchaCode();
            var model = _captchaService.GenerateCaptchaImage(generateCaptchaCode.First, generateCaptchaCode.Second);
            var now = DateTime.UtcNow;

            #region Delete Obsolete Captchas 
            var deleteList = _repository.GetAll().Where(x => x.CreatedDate.AddHours(1) < now);
            if (deleteList.Any())
            {
                _repository.DeleteRange(deleteList);
            }
            #endregion

            _repository.Insert(new Captcha
            {
                Id = Guid.NewGuid(),
                Key = model.Key,
                CreatedDate = now,
                FinalCode = generateCaptchaCode.FinalCode
            });
            return Task.FromResult(new CaptchaDto { Image = model.Image, Key = model.Key });
        }
    }
}
