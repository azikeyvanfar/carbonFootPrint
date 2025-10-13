using AutoMapper;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Application.Mapping.Core
{
    public class LookupProfile : Profile
    {
        public LookupProfile()
        {

            CreateMap<AddLookupDto, Lookup>()
                .ForMember(dest => dest.EnName, opt => opt.MapFrom(src => src.Code.ToLower()))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => LookupType.Lookup));

            CreateMap<Lookup, LookupDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => (src.ParentId != null) ? src.Parent.FaName : null));

            CreateMap<UpdateLookupDto, Lookup>()
                .ForMember(dest => dest.EnName, opt => opt.MapFrom(src => src.Code.ToLower()))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => LookupType.Lookup));

            CreateMap<AddLookupItemDto, Lookup>()
                .ForMember(dest => dest.EnName, opt => opt.MapFrom(src => src.EnName.ToLower()))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToLower()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => LookupType.Item));

            CreateMap<UpdateLookupItemDto, Lookup>()
                .ForMember(dest => dest.EnName, opt => opt.MapFrom(src => src.EnName.ToLower()))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToLower()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => LookupType.Item));


            CreateMap<Lookup, LookupItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EnName, opt => opt.MapFrom(src => src.EnName.ToLower()))
                .ForMember(dest => dest.FaName, opt => opt.MapFrom(src => src.FaName))
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.FaName : string.Empty))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.EnumCode, opt => opt.MapFrom(src => src.EnumCode))
                ;

        }
    }
}
