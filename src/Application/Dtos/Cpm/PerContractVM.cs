using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Cpm
{
    public class PerContractVM : PageableDto
    {
            public string num_pgv_puror{get;set;}
            public string supco_thprt_cod_thprt { get; set; }
            public string des_thprt{get;set;}
            public string cod_cc{get;set;}
            public string cod_busun{get;set;}
            public string des_busun{get;set;}
            public string lkp_cod_area_busun{get;set;}
            public string des_cod_area_busun{get;set;}
            public string dat_str_vald_puror{get;set;}
            public string dat_end_vald_puror{get;set;}
            public string emply_num_prsn_emply{get;set;}
            public string nam_full_nazer{get;set;}
            public string mng1{get;set;}
            public string nam_full_mng1{get;set;}
            public string mng2{get;set;}
            public string nam_full_mng2{get;set;}
    }
}