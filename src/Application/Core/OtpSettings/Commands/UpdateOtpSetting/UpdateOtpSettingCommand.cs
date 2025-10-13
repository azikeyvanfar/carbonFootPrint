using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.Core.OtpSettings.Commands.UpdateOtpSetting
{
    public class UpdateOtpSettingCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public string Otp { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset WaitConfirmTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset ActiveTime { get; set; }

        public string HashToken { get; set; }
        [Required]
        public OtpType otpType { get; set; }
    }

    public class UpdateOtpSettingCommandHandler : IRequestHandler<UpdateOtpSettingCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<OtpSetting> _repository;
        public UpdateOtpSettingCommandHandler(IMapper mapper, IRepository<OtpSetting> OtpSettingRep)
        {
            _mapper = mapper;
            _repository = OtpSettingRep;
        }

        public Task<bool> Handle(UpdateOtpSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);

            if (entity is null)
                throw new NullReferenceException();

            _mapper.Map(request, entity);

            bool flag = _repository.UpdateEntity(entity);

            return Task.FromResult(flag);
        }
    }
}
