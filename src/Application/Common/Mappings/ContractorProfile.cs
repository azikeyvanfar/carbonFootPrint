using AutoMapper;
using ContractorBackend.Domain.Entities.IsSuiteEntities;

namespace ContractorBackend.Application.Mapping.IsSuite
{
    public class ContractorProfile : Profile
    {
        public ContractorProfile()
        {
            CreateMap<Contractor, Contractor>()
               .ForMember(c => c.Id, x => x.Ignore());

            //CreateMap<Contractor, ContractorVM>()
            //   .ForMember(d => d.num_pgv_puror, m => m.MapFrom(s => s.IsSuiteId))
            //   ;

            //CreateMap<ContractorVM, Contractor>()
            //    .ForMember(d => d.IsSuiteId, m => m.MapFrom(s => s.num_pgv_puror))
            //    .ForMember(d => d.Id, m => m.Ignore())

            //    .ForMember(d => d.ContractorCode, m => m.MapFrom(s => s.supco_thprt_cod_thprt))
            //    .ForMember(d => d.ContractorName, m => m.MapFrom(s => s.des_thprt))
            //    .ForMember(d => d.ContractorContractNumber, m => m.MapFrom(s => s.num_pgv_puror))
            //    .ForMember(d => d.Description, m => m.MapFrom(s => s.des_sho_puror))
            //    .ForMember(d => d.EmployeeCount, m => m.MapFrom(s => s.cnt_empl))
            //    .ForMember(d => d.AreaCode, m => m.MapFrom(s => s.lkp_cod_area_busun))
            //    .ForMember(d => d.BusinessUnitCode, m => m.MapFrom(s => s.cod_busun))
            //    .ForMember(d => d.BusinessUnitIsSuiteId, m => m.MapFrom(s => s.busun_business_unit_id))
            //    .ForMember(d => d.CEOFullName, m => m.MapFrom(s => s.nam_manager))
            //    .ForMember(d => d.EmployerRepresentitivePersonnelCode, m => m.MapFrom(s => s.num_prsn_agent))
            //    .ForMember(d => d.EmployerRepresentitiveFullName, m => m.MapFrom(s => s.nam_agent))
            //    .ForMember(d => d.SupervisorPersonnelCode, m => m.MapFrom(s => s.emply_num_prsn_emply))
            //    .ForMember(d => d.SupervisorFullName, m => m.MapFrom(s => s.full_nam))
            //    .ForMember(d => d.StartDate, m => m.MapFrom(s => s.dat_str_vald_puror))
            //    .ForMember(d => d.EndDate, m => m.MapFrom(s => s.dat_end_vald_puror))
            //    ;

            //CreateMap<Contractor, ContractorDto>()
            //    .ForMember(d => d.ContractorContractNumber, m => m.MapFrom(s => s.ContractorContractNumber.ToString()))
            //    .ForMember(s => s.CreatedDateTime, m => m.MapFrom(_ => EF.Property<DateTimeOffset?>(_, "CreatedDateTime")))
            //    .ForMember(s => s.ModifiedDateTime, m => m.MapFrom(_ => EF.Property<DateTimeOffset?>(_, "ModifiedDateTime")))
            //    ;

            //CreateMap<ContractorDto, Contractor>()
            ;

        }
    }
}
