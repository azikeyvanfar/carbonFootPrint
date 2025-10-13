using AutoMapper;
using ContractorBackend.Application.Dtos;

namespace ContractorBackend.Application.Mapping.Core
{
    public class RoleClaimProfile : Profile
    {
        public RoleClaimProfile()
        {
            CreateMap<Domain.Entities.Identity.RoleClaim, RoleClaimDto>()
                .ForMember(c => c.PageRouteClaimId, x => x.MapFrom(d => d.PageRouteClaimId))
                .ForMember(c => c.RoleId, x => x.MapFrom(d => d.RoleId))
                .ForMember(c => c.PageRouteId, x => x.MapFrom(d => d.PageRouteClaim.PageRouteId))
                .ForMember(c => c.Route, x => x.MapFrom(d => d.PageRouteClaim.PageRoute.Route))
                .ForMember(c => c.RouteName, x => x.MapFrom(d => d.PageRouteClaim.PageRoute.RouteName))
                .ForMember(c => c.GeneralClaimId, x => x.MapFrom(d => d.PageRouteClaim.GeneralClaim.Id))
                .ForMember(c => c.ClaimValue, x => x.MapFrom(d => d.PageRouteClaim.GeneralClaim.ClaimValue))
                .ForMember(c => c.RoleName, x => x.MapFrom(d => d.Role.Name));
        }
    }
}
