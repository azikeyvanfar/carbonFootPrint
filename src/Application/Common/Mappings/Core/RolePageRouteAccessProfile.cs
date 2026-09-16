using AutoMapper;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;

namespace ContractorBackend.Application.Mapping.Core
{
    public class RolePageRouteAccessProfile : Profile
    {
        public RolePageRouteAccessProfile()
        {
            CreateMap<RolePageRouteAccess, RolePageRouteAccessDto>()
            .ForMember(c => c.Id, x => x.MapFrom(d => d.Id))
            .ForMember(c => c.RoleId, x => x.MapFrom(d => d.RoleId))
            .ForMember(c => c.PageRouteId, x => x.MapFrom(d => d.PageRouteId))
            .ForMember(c => c.IsActive, x => x.MapFrom(d => d.IsActive))
            .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

        }
    }
}
