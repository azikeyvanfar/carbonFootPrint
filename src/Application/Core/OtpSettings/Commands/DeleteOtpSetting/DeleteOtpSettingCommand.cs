using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using MediatR;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.DeleteOtpSetting
{
    public class DeleteOtpSettingCommand : IRequest
    {
        public Guid OtpSettingId { get; set; }
        public DeleteOtpSettingCommand(Guid id)
        {
            OtpSettingId = id;
        }
    }

    public class DeleteOtpSettingCommandHandler : IRequestHandler<DeleteOtpSettingCommand>
    {
        private readonly IRepository<OtpSetting> _otpRepository;
        public DeleteOtpSettingCommandHandler(IRepository<OtpSetting> otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public Task<Unit> Handle(DeleteOtpSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = _otpRepository.GetById(request.OtpSettingId);
            if (entity is null)
                throw new NullReferenceException();

            _otpRepository.Delete(entity);
            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<DeleteOtpSettingCommand>.Handle(DeleteOtpSettingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
