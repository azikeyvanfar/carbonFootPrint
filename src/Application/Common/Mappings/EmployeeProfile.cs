using AutoMapper;

namespace ContractorBackend.Application.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            //CreateMap<Employee, EmployeeDto>() 
            //    .ForMember(d => d.PersonnelCode, m => m.MapFrom(s => s.PersonnelCode.Value.ToString()??string.Empty))
            //    ;

            //CreateMap<EmployeeVM, Employee>()
            //    .ForMember(d => d.PersonnelCode, m => m.MapFrom(s => s.num_prsn_emply))
            //    .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.num_mobil_emply))
            //    .ForMember(d => d.FirstName, m => m.MapFrom(s => s.nam_first_emply))
            //    .ForMember(d => d.LastName, m => m.MapFrom(s => s.nam_last_emply))
            //    .ForMember(d => d.NationalCode, m => m.MapFrom(s => s.cod_nat_emply))
            //    .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.dat_birth_emply))
            //    .ForMember(d => d.CodTeam, m => m.MapFrom(s => s.num_team_emply))
            //    .ForMember(d => d.TeamId, m => m.MapFrom(s => s.team_id))
            //    .ForMember(d => d.Age, m => m.MapFrom(s => s.age))
            //    .ForMember(d => d.CodJob, m => m.MapFrom(s => s.cod_job))
            //    .ForMember(d => d.DesJob, m => m.MapFrom(s => s.des_job))
            //    .ForMember(d => d.CodEducation, m => m.MapFrom(s => s.cod_educa))
            //    .ForMember(d => d.DesEducation, m => m.MapFrom(s => s.des_educa))
            //    .ForMember(d => d.WorkHistoryYear, m => m.MapFrom(s => s.sen_int_y))
            //    .ForMember(d => d.WorkHistorymonth, m => m.MapFrom(s => s.sen_int_m))
            //    .ForMember(d => d.WorkPlace, m => m.MapFrom(s => s.workplace))
            //    .ForMember(d => d.ContractNumber, m => m.MapFrom(s => s.num_ord_empca))
            //    .ForMember(d => d.ContractorCode, m => m.MapFrom(s => s.cod_cntr_empca))
            //    .ForMember(d => d.IdNumber, m => m.MapFrom(s => s.num_crt_emply))
            //    .ForMember(d => d.FatherName, m => m.MapFrom(s => s.nam_fathr_emply))
            //    .ForMember(d => d.GenderCode, m => m.MapFrom(s => s.lkp_cod_sex_emply))
            //    .ForMember(d => d.Gender, m => m.MapFrom(s => s.des_lkp_cod_sex_emply))
            //    .ForMember(d => d.BusinessUnitIsSuiteId, m => m.MapFrom(s => s.busun_business_unit_id))
            //    .ForMember(d => d.BusinessUnitCode, m => m.MapFrom(s => s.cod_busun_emply))
            //    .ForMember(d => d.Image, m => m.MapFrom(s => s.image_date))
            //    ;


        }

    }
}
