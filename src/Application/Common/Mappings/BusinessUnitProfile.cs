using AutoMapper;
using ContractorBackend.Application.Dtos.Ojc;

namespace ContractorBackend.Application.Mapping.IsSuite
{
    public class BusinessUnitProfile : Profile
    {
        public BusinessUnitProfile()
        {
            //CreateMap<BusinessUnit, BusinessUnit>()
            //   .ForMember(c => c.Id, x => x.Ignore());

            //CreateMap<BusinessUnit, BusinessUnitVM>()
            //   .ForMember(d => d.id, m => m.MapFrom(s => s.IsSuiteId))
            //   .ForMember(d => d.flg_is_active, m => m.MapFrom(s => s.IsActive))
            //   ;

            //CreateMap<BusinessUnitVM, BusinessUnit>()
            //   .ForMember(d => d.IsSuiteId, m => m.MapFrom(s => s.id))
            //   .ForMember(d => d.IsActive, m => m.MapFrom(s => s.flg_is_active))
            //   .ForMember(d => d.Id, m => m.Ignore())
            //   ;

            //CreateMap<BusinessUnit, BusinessUnitDto>()
            //    .ForMember(s => s.CreatedDateTime, m => m.MapFrom(_ => EF.Property<DateTimeOffset?>(_, "CreatedDateTime")))
            //    .ForMember(s => s.ModifiedDateTime, m => m.MapFrom(_ => EF.Property<DateTimeOffset?>(_, "ModifiedDateTime")))
            //    ;

            //CreateMap<BusinessUnitDto, BusinessUnit>()
            ;

            CreateMap<BusinessUnitWithCostCenterVM, BusinessUnitWithCostCenterDto>()
                .ForMember(d => d.BusinessUnitId, m => m.MapFrom(s => s.business_unit_id))
                .ForMember(d => d.BusinessUnitCode, m => m.MapFrom(s => s.cod_busun))
                .ForMember(d => d.BusinessUnitName, m => m.MapFrom(s => s.des_busun))
                .ForMember(d => d.CostCenterId, m => m.MapFrom(s => s.cost_center_id))
                .ForMember(d => d.CostCenterCode, m => m.MapFrom(s => s.cod_cc_ccntr))
                .ForMember(d => d.CostCenterName, m => m.MapFrom(s => s.des_cc_ccntr))
                ;

            CreateMap<ChildrenBusinessUnitVM, ChildrenBusinessUnitDto>()
                .ForMember(d => d.BusinessUnitId, m => m.MapFrom(s => s.business_unit_id))
                .ForMember(d => d.BusinessUnitCode, m => m.MapFrom(s => s.cod_busun))
                .ForMember(d => d.BusinessUnitName, m => m.MapFrom(s => s.des_busun))
                .ForMember(d => d.CostCenterId, m => m.MapFrom(s => s.cost_center_id_busun))
                .ForMember(d => d.CostCenterCode, m => m.MapFrom(s => s.cod_cc_busun))
                .ForMember(d => d.CostCenterName, m => m.MapFrom(s => s.des_cc_busun))
                ;




        }
    }
}
