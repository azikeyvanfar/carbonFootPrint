using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Application.Dtos.Ojc;
using ContractorBackend.Application.Dtos.scs;
using ContractorBackend.Common.Models;
using ContractorBackend.Domain.Entities;
using ContractorBackend.Domain.Enums.Core;
using Microsoft.Extensions.Configuration;


namespace ContractorBackend.Application.Services
{
    public class IsSuiteClientService
    {
        private readonly HttpClientMethods _clientMethods;
        private bool _isIsSuiteMock = true;

        public IsSuiteClientService(
            HttpClientMethods clientMethods,
            IConfiguration configuration)
        {
            _clientMethods = clientMethods;
            _isIsSuiteMock = configuration["IsSuiteMock"] == "True" ? true : false;
        }

        private (string, ServiceEnum) FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum urlEnum)
        {
            //var row = _context.IsSuiteApiInfos.FirstOrDefault(x => x.UrlKey == urlEnum);

            var row = IsSuiteUrlClass.dict[urlEnum];

            if (row is null)
            {
                throw new ArgumentNullException($"url for given key not found.key : {urlEnum.ToString()}");
            }

            return (row.Url, row.Service);
        }




        public async Task<IsSuiteResponseDto> CheckSMSCode(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.check_cod_fun);
            var response = await _clientMethods.PostService<IsSuiteResponseDto>(url, queryParams, null, service);

            return response;
        }

        ///// <summary>
        ///// اطلاعات کاربر 
        ///// pds_per_employees_info_spc_viw
        ///// </summary>
        ///// <param name="queryParams"></param>
        ///// <returns></returns>
        //public async Task<IsSuiteResponse<UserInfoDto>> GetEmployeeInfoViewAsync(List<QueryParamModel> queryParams)
        //{
        //    var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.pds_cpm_employees_viw);
        //    var response = await _clientMethods.GetService<IsSuiteResponse<UserInfoDto>>(url, queryParams, service);

        //    return response;
        //}

        /// <summary>
        /// لیست واحدهای سازمانی 
        /// </summary>
        public async Task<IsSuiteResponse<BusinessUnitVM>> GetOjcPersonnelBusinessUnitsViewAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.ojc_cpm_business_units_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<BusinessUnitVM>>(url, queryParams, service);

            return response;
        }

        /// <summary>
        /// ناحیه ها
        /// </summary>
        public async Task<IsSuiteResponse<AreaVM>> GetOjcAreasViewAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.lkp_cod_area_busun);
            var response = await _clientMethods.GetService<IsSuiteResponse<AreaVM>>(url, queryParams, service);

            return response;
        }


        /// <summary>
        /// اطلاعات پرسنل پیمانکار
        /// </summary> 
        public async Task<IsSuiteResponse<AreaVM>> GetCpmHsewebEmplContrViewAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.cpm_cpmweb_empl_contr_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<AreaVM>>(url, queryParams, service);

            return response;
        }

        /// <summary>
        ///  کاربران فولاد
        /// </summary>
        public async Task<IsSuiteResponse<EmployeeVM>> GetAllEmployeeViewAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.pds_cpm_employees_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<EmployeeVM>>(url, queryParams, service);

            return response;
        }


        /// <summary>
        /// لیست واحد های سازمانی با مرکز هزینه مشترک
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns> 
        public async Task<IsSuiteResponse<BusinessUnitWithCostCenterVM>> GetBusinessUnitsSharedCostCenter(List<QueryParamModel> queryParams)
        {
            //#region Mock
            //IsSuiteResponse<BusinessUnitWithCostCenterVM> response1 = new();
            //response1.Items = new();
            //response1.Items.Add(new BusinessUnitWithCostCenterVM
            //{
            //    BusinessUnitId  =  576,
            //    BusinessUnitCode   = "RMM",
            //    BusinessUnitName   ="واحد انباشت و برداشت",
            //    CostCenterCode    = "1110",
            //    CostCenterName    = "محوطه انباشت وبرداشت سنگ آهن  وگندله",
            //});
            //response1.Items.Add(new BusinessUnitWithCostCenterVM
            //{
            //    BusinessUnitId  =  107,
            //    BusinessUnitCode   = "LCA",
            //    BusinessUnitName   ="آهک‌سازی",
            //    CostCenterCode    = "1120",
            //    CostCenterName    = "واحد آهک سازی",
            //});

            //return response1;
            //#endregion

            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.ojc_busn_share_cc_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<BusinessUnitWithCostCenterVM>>(url, queryParams, service);

            return response;
        }


        /// <summary>
        /// لیست واحد ها و مراکز هزینه زیرین واحد سازمانی در زمان درخواستی
        /// </summary>
        public async Task<IsSuiteResponse<ChildrenBusinessUnitVM>> GetChildrenBusinessUnitCostCentersViewAsync(List<QueryParamModel> queryParams)
        {
            //#region Mock
            //IsSuiteResponse<BusinessUnitWithCostCenterVM> response1 = new();
            //response1.Items = new();
            //response1.Items.Add(new BusinessUnitWithCostCenterVM
            //{
            //    BusinessUnitId     = 576,
            //    BusinessUnitCode   = "RMM",
            //    BusinessUnitName   ="واحد انباشت و برداشت", 
            //    CostCenterCode    = "1110",
            //    CostCenterName    = "محوطه انباشت وبرداشت سنگ آهن  وگندله",
            //});
            //response1.Items.Add(new BusinessUnitWithCostCenterVM
            //{
            //    BusinessUnitId     = 107,
            //    BusinessUnitCode   = "LCA",
            //    BusinessUnitName   ="آهک‌سازی",
            //    CostCenterCode    = "1120",
            //    CostCenterName    = "واحد آهک سازی",
            //});

            //return response1;
            //#endregion


            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.ojc_per_busun_fun);
            var response = await _clientMethods.GetService<IsSuiteResponse<ChildrenBusinessUnitVM>>(url, queryParams, service);

            return response;
        }









    }

    public class IsSuiteResponseBuilder<T> where T : class
    {
        public static IsSuiteResponse<T> Response(List<T> responseList, List<QueryParamModel>? queryParams)
        {
            return new IsSuiteResponse<T>
            {
                Count = responseList.Count,
                HasMore = false,
                Items = responseList,
                Limit = 10, //int.Parse(queryParams.FirstOrDefault(x => x.ParameterName.Contains("limit")).ParameterValue),
                offset = 1 //int.Parse(queryParams.FirstOrDefault(x => x.ParameterName.Contains("offset")).ParameterValue)
            };
        }
    }

}
