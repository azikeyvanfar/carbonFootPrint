using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Cpm
{
    public class ContractFamiliesVM : PageableDto
    {
        public string num_prsn_emplc {get;set;}
        public string num_national_emplc {get;set;}
        public string nam_first_famco {get;set;}
        public string nam_last_famco {get;set;}
        public string nam_fathr_famco {get;set;}
        public string dat_birth_famco {get;set;}
        public string lkp_cod_sex_famco {get;set;}
        public string des_lkp_cod_sex {get;set;}
        public string des_lkp_cod_dpncy {get;set;}
        public string lkp_cod_dpncy_famco {get;set;}
        public string des_lkp_cod_mrid {get;set;}
        public string lkp_cod_mrid_famco {get;set;}
        public string num_mobil_emplc {get;set;}
        public string dat_end_dpncy_famco {get;set;}
        public string lkp_cod_age_famco {get;set;}
        public string cod_crt_famco {get;set;}
        public string cod_nat_famco {get;set;}
        public string lkp_cod_job_famco {get;set;}
        public string des_lkp_cod_job { get; set; }
    }
}