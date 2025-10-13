using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Dtos;
using MediatR;

namespace ContractorBackend.Application.Account.Query.GetUserInfo
{
    public class GetUserInfoQuery : IRequest<UserProfileDto>
    {
        public long UserId { get; set; }
        public GetUserInfoQuery(long id)
        {
            UserId = id;
        }
    }

    public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserProfileDto>
    {
        private readonly IApplicationUserManager _userManager;
        //private readonly IRepository<UserDetail> _userDetail;
        private readonly IMapper _mapper;
        public GetUserInfoQueryHandler(
            IApplicationUserManager userManager,
            //IRepository<UserDetail> userDetail,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            //_userDetail = userDetail;
        }

        public async Task<UserProfileDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NullReferenceException();
            }
            //user.UserDetails = _userDetail.GetAll().Where(t => t.UserId == request.UserId).FirstOrDefault();
            UserProfileDto userDto = _mapper.Map<UserProfileDto>(user);



            return userDto;
        }
    }
}
