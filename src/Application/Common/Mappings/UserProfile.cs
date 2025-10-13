using AutoMapper;
using ContractorBackend.Application.Core.Account.Commands.RegisterAccount;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserLovDto>()
                .ForMember(d => d.FullName, m => m.MapFrom(s => s.FirstName + " " + s.LastName))
                ;

            CreateMap<User, UserDto>()
                .ForMember(d => d.FullName, m => m.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(d => d.Roles, m => m.MapFrom(s => s.Roles))
                ;

            CreateMap<UserRole, RoleDto>()
                .ForMember(d => d.RoleId, m => m.MapFrom(s => s.RoleId))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Role.Name))
                .ForMember(d => d.RoleType, m => m.MapFrom(s => s.Role.RoleType))
                .ForMember(d => d.IsActive, m => m.MapFrom(s => s.Role.IsActive))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Role.Description))
                .ForMember(d => d.IsAdministrator, m => m.MapFrom(s => s.Role.IsAdministrator))
                .ForMember(d => d.RoleScopeId, m => m.MapFrom(s => s.Role.RoleScopeId))
                .ForMember(d => d.RoleScopeName, m => m.MapFrom(s => s.Role.RoleScope != null ? s.Role.RoleScope.FaName : string.Empty))
                ;

            CreateMap<User, UserVM>()
                 .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                 .ForMember(d => d.FirstName, m => m.MapFrom(s => s.FirstName))
                 .ForMember(d => d.LastName, m => m.MapFrom(s => s.LastName))
                 .ForMember(d => d.PersonnelCode, m => m.MapFrom(s => s.PersonnelCode))
                 .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate))
                 .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.PhoneNumber))
                 .ForMember(d => d.Email, m => m.MapFrom(s => s.Email))
                 .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.PhoneNumber))
                 .ForMember(d => d.NationalCode, m => m.MapFrom(s => s.NationalCode))
                 //.ForMember(d => d.DesCategoryJob, m => m.MapFrom(s => s.UserDetails.DesCategoryJob))
                 //.ForMember(d => d.DesCostCenter, m => m.MapFrom(s => s.UserDetails.DesCostCenter))
                 //.ForMember(d => d.InYearsCount, m => m.MapFrom(s => s.UserDetails.InYearsCount))
                 //.ForMember(d => d.OutYearsCount, m => m.MapFrom(s => s.UserDetails.OutYearsCount))
                 //.ForMember(d => d.AllMatchYears, m => m.MapFrom(s => s.UserDetails.AllMatchYears))
                 //.ForMember(d => d.DesSex, m => m.MapFrom(s => s.UserDetails.DesSex))
                 //.ForMember(d => d.DesAssistance, m => m.MapFrom(s => s.UserDetails.DesAssistance))
                 //.ForMember(d => d.DesPost, m => m.MapFrom(s => s.UserDetails.DesPost))
                 //.ForMember(d => d.ProfileImage, m => m.MapFrom(s => s.UserDetails.ProfileImage != null ? Convert.ToBase64String(s.UserDetails.ProfileImage) : string.Empty))
                 .ForAllOtherMembers(x => x.Ignore());


            CreateMap<RegisterAccountCommand, User>();

            CreateMap<UserInfoDto, User>()
                .ForMember(d => d.PersonnelCode, m => m.MapFrom(s => s.num_prsn_emply))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.num_prsn_emply))
                .ForMember(d => d.NormalizedUserName, m => m.MapFrom(s => s.num_prsn_emply))
                .ForMember(d => d.FirstName, m => m.MapFrom(s => s.nam_first_emply))
                .ForMember(d => d.LastName, m => m.MapFrom(s => s.nam_last_emply))
                .ForMember(d => d.NationalCode, m => m.MapFrom(s => s.cod_nat_emply))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.dat_birth_emply))
                .ForMember(d => d.FirstNameEng, m => m.MapFrom(s => s.nam_first_eng_emply))
                .ForMember(d => d.LastNameEng, m => m.MapFrom(s => s.nam_last_eng_emply))
                .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.num_mobil_emply.ToString()))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.des_email_emply))
                .ForMember(d => d.UpLevel, m => m.MapFrom(s => s.up_lvl_emp))
            .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<UserInfoNoImageDto, User>()
              .ForMember(d => d.PersonnelCode, m => m.MapFrom(s => s.num_prsn_emply))
              .ForMember(d => d.UserName, m => m.MapFrom(s => s.num_prsn_emply))
              .ForMember(d => d.NormalizedUserName, m => m.MapFrom(s => s.num_prsn_emply))
              .ForMember(d => d.FirstName, m => m.MapFrom(s => s.nam_first_emply))
              .ForMember(d => d.LastName, m => m.MapFrom(s => s.nam_last_emply))
              .ForMember(d => d.NationalCode, m => m.MapFrom(s => s.cod_nat_emply))
              .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.dat_birth_emply))
              .ForMember(d => d.FirstNameEng, m => m.MapFrom(s => s.nam_first_eng_emply))
              .ForMember(d => d.LastNameEng, m => m.MapFrom(s => s.nam_last_eng_emply))
              .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.num_mobil_emply.ToString()))
              .ForMember(d => d.Email, m => m.MapFrom(s => s.des_email_emply))
              .ForMember(d => d.UpLevel, m => m.MapFrom(s => s.up_lvl_emp))
              .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<User, UserInfoDto>()
                .ForMember(d => d.num_prsn_emply, m => m.MapFrom(s => s.PersonnelCode))
                .ForMember(d => d.nam_first_emply, m => m.MapFrom(s => s.FirstName))
                .ForMember(d => d.nam_last_emply, m => m.MapFrom(s => s.LastName))
                .ForMember(d => d.cod_nat_emply, m => m.MapFrom(s => s.NationalCode))
                .ForMember(d => d.nam_first_eng_emply, m => m.MapFrom(s => s.FirstNameEng))
                .ForMember(d => d.nam_last_eng_emply, m => m.MapFrom(s => s.LastNameEng))
                .ForMember(d => d.num_mobil_emply, m => m.MapFrom(s => long.Parse(s.PhoneNumber ?? "0")))
                .ForMember(d => d.dat_birth_emply, m => m.MapFrom(s => s.BirthDate))
                .ForMember(d => d.des_email_emply, m => m.MapFrom(s => s.Email))
                .ForMember(d => d.up_lvl_emp, m => m.MapFrom(s => s.UpLevel))
                .ForAllOtherMembers(opt => opt.Ignore());


            CreateMap<User, UserProfileDto>()
                .ForMember(d => d.num_prsn_emply, m => m.MapFrom(s => s.PersonnelCode))
                .ForMember(d => d.nam_first_emply, m => m.MapFrom(s => s.FirstName))
                .ForMember(d => d.nam_last_emply, m => m.MapFrom(s => s.LastName))
                .ForMember(d => d.cod_nat_emply, m => m.MapFrom(s => s.NationalCode))
                .ForMember(d => d.dat_birth_emply, m => m.MapFrom(s => s.BirthDate))
                .ForMember(d => d.nam_first_eng_emply, m => m.MapFrom(s => s.FirstNameEng))
                .ForMember(d => d.nam_last_eng_emply, m => m.MapFrom(s => s.LastNameEng))
                .ForMember(d => d.num_mobil_emply, m => m.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.des_email_emply, m => m.MapFrom(s => s.Email))

               // .ForMember(d => d.image_prsn_emply, m => m.MapFrom(s => s.UserDetails.Image))
               //.ForMember(d => d.des_busun, m => m.MapFrom(s => s.UserDetails.DesBusun))
               //.ForMember(d => d.des_cc, m => m.MapFrom(s => s.UserDetails.DesCostCenter))
               //.ForMember(d => d.des_lkp_cod_sex_emply, m => m.MapFrom(s => s.UserDetails.DesSex))
               //.ForMember(d => d.des_lkp_cod_mrid_emply, m => m.MapFrom(s => s.UserDetails.DesMarid))
               // .ForMember(d => d.lkp_cod_mrid_emply, m => m.MapFrom(s => s.UserDetails.CodMarid))

               //.ForMember(d => d.tot_child_emply, m => m.MapFrom(s => s.UserDetails.ChildCount))
               //.ForMember(d => d.nam_fathr_emply, m => m.MapFrom(s => s.UserDetails.FatherName))
               //.ForMember(d => d.des_gepos_geog_position_id_a, m => m.MapFrom(s => s.UserDetails.DesBirthPosition))
               //.ForMember(d => d.des_gepos_geog_position_id_b, m => m.MapFrom(s => s.UserDetails.DesRegBirthPlacePosition))
               //.ForMember(d => d.dat_crt_emply, m => m.MapFrom(s => s.UserDetails.CrtyDate))
               //.ForMember(d => d.num_crt_emply, m => m.MapFrom(s => s.UserDetails.CrtNum))
               //.ForMember(d => d.num_ser_crt_emply, m => m.MapFrom(s => s.UserDetails.CrtSerNum))
               //.ForMember(d => d.des_lkp_cod_natlty_emply, m => m.MapFrom(s => s.UserDetails.DesNationaly))
               //.ForMember(d => d.des_lkp_cod_rlgn_emply, m => m.MapFrom(s => s.UserDetails.DesReligion))

               //.ForMember(d => d.num_tel_emply, m => m.MapFrom(s => s.UserDetails.TellNum))
               //.ForMember(d => d.num_tel_emrgy_emply, m => m.MapFrom(s => s.UserDetails.EmergencyTellNum))
               //.ForMember(d => d.num_tel_int_emply, m => m.MapFrom(s => s.UserDetails.InternalTellNumFirst))
               //.ForMember(d => d.num_tel_int2_emply, m => m.MapFrom(s => s.UserDetails.InternalTellNumSec))

               //.ForMember(d => d.des_lkp_cod_area_busun, m => m.MapFrom(s => s.UserDetails.DesArea))
               //.ForMember(d => d.cod_post_emply, m => m.MapFrom(s => s.UserDetails.CodePost))
               //.ForMember(d => d.des_per_postd, m => m.MapFrom(s => s.UserDetails.DesPost))
               //.ForMember(d => d.des_lkp_sta_emplt_emply, m => m.MapFrom(s => s.UserDetails.DesEmployedStatus))
               //.ForMember(d => d.des_wrktm, m => m.MapFrom(s => s.UserDetails.DesWorkTime))
               //.ForMember(d => d.des_job, m => m.MapFrom(s => s.UserDetails.DesJob))
               //.ForMember(d => d.des_lkp_cod_class_job, m => m.MapFrom(s => s.UserDetails.DesCategoryJob))
               //.ForMember(d => d.sen_int, m => m.MapFrom(s => s.UserDetails.InYearsCount))
               //.ForMember(d => d.sen_out, m => m.MapFrom(s => s.UserDetails.OutYearsCount))
               //.ForMember(d => d.tatbigh_all, m => m.MapFrom(s => s.UserDetails.AllMatchYears))
               //.ForMember(d => d.des_movenat, m => m.MapFrom(s => s.UserDetails.DesAssistance))
               //.ForMember(d => d.des_pycnd, m => m.MapFrom(s => s.UserDetails.DesEmploymentType))
               //.ForMember(d => d.des_cod_emplt, m => m.MapFrom(s => s.UserDetails.DesRecuitmentType))
               //.ForMember(d => d.des_lkp_grp_obg_emply, m => m.MapFrom(s => s.UserDetails.DesReward))
               //.ForMember(d => d.num_team_teams, m => m.MapFrom(s => s.UserDetails.CodTeam))

               //.ForMember(d => d.CodJob, m => m.MapFrom(s => s.UserDetails.CodJob))
               //.ForMember(d => d.CodCostCenter, m => m.MapFrom(s => s.UserDetails.CodCostCenter))
               //.ForMember(d => d.DesJob, m => m.MapFrom(s => s.UserDetails.DesJob))
               //.ForMember(d => d.DesCostCenter, m => m.MapFrom(s => s.UserDetails.DesCostCenter))
               .ForAllOtherMembers(opt => opt.Ignore());

        }

    }
}
