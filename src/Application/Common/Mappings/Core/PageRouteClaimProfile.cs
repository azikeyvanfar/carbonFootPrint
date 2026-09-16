using AutoMapper;
using ContractorBackend.Application.Core.PageRouteClaims.Commands.UpdatePageRouteClaim;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;

namespace ContractorBackend.Application.Mapping.Core
{
    public class PageRouteClaimProfile : Profile
    {
        public PageRouteClaimProfile()
        {

            CreateMap<PageRouteClaim, PageRouteClaimDto>()
                .ForMember(c => c.Id, x => x.MapFrom(d => d.Id))
                .ForMember(c => c.PageRouteId, x => x.MapFrom(d => d.PageRouteId))
                .ForMember(c => c.GeneralClaimId, x => x.MapFrom(d => d.GeneralClaimsId))
                .ForMember(c => c.ClaimValue, x => x.MapFrom(d => d.GeneralClaim.ClaimValue))
                .ForMember(c => c.ClaimName, x => x.MapFrom(d => d.GeneralClaim.ClaimName))
                .ForMember(c => c.ClaimRoleTypeId, x => x.MapFrom(d => ((int)d.GeneralClaim.RoleType).ToString()))
                .ForMember(c => c.ClaimRoleType, x => x.MapFrom(d => d.GeneralClaim.RoleType))
                .ForMember(c => c.Route, x => x.MapFrom(d => d.PageRoute.Route))
                .ForMember(c => c.RouteName, x => x.MapFrom(d => d.PageRoute.RouteName))
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

            CreateMap<UpdatePageRouteClaimCommand, PageRouteClaim>()
               .ForMember(c => c.Id, x => x.MapFrom(d => d.Id))
                .ForMember(c => c.PageRouteId, x => x.MapFrom(d => d.PageRouteId))
                .ForMember(c => c.GeneralClaimsId, x => x.MapFrom(d => d.ClaimId))
                  //.ForMember(c => c.ClaimValue, x => x.MapFrom(d => d.ClaimValue))
                  //.ForMember(c => c.ClaimName, x => x.MapFrom(d => d.ClaimName))
                  .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        }
    }
}
