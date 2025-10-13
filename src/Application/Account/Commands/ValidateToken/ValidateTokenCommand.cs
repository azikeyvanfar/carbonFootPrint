using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Token;
using MediatR;

namespace ContractorBackend.Application.Account.Commands.ValidateToken
{
    public class ValidateTokenCommand : IRequest<bool>
    {
        public string AccessToken { get; set; } = null!;
        public long UserId { get; set; }
    }

    public class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, bool>
    {
        private readonly ITokenStoreService _tokenStoreService;

        public ValidateTokenCommandHandler(ITokenStoreService tokenStoreService)
        {
            _tokenStoreService = tokenStoreService;
        }

        public async Task<bool> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == 0)
                throw new CustomException("کاربری با این مشخصات یافت نشد");
            return await _tokenStoreService.IsValidTokenAsync(request.AccessToken, request.UserId);
        }
    }
}
