using AutoMapper;
using ContractorBackend.Application.Dtos;

namespace ContractorBackend.Application.Mapping.Core
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Domain.Entities.Identity.Role, RoleDto>()
              .ForMember(d => d.RoleId, m => m.MapFrom(s => s.Id))
              .ForMember(d => d.Title, m => m.MapFrom(s => s.Name))
              .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
              .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
              .ForMember(d => d.RoleType, m => m.MapFrom(s => s.RoleType))
              .ForMember(d => d.IsAdministrator, m => m.MapFrom(s => s.IsAdministrator))
              .ForMember(d => d.RoleType, m => m.MapFrom(s => s.RoleType))
              .ForMember(d => d.RoleScopeId, m => m.MapFrom(s => s.RoleScopeId))
              .ForMember(d => d.RoleScopeName, m => m.MapFrom(s => s.RoleScope != null ? s.RoleScope.FaName : string.Empty))
              .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
