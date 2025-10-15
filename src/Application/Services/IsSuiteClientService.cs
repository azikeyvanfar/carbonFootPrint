using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.Dtos.Cpm;
using ContractorBackend.Common.Models;
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

        /// <summary>
        /// لیست پیمانکاران
        /// </summary>
        public async Task<IsSuiteResponse<CpmperEmployeesVM>> GetCpmperEmployeesViwAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.cpm_cpmper_employees_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<CpmperEmployeesVM>>(url, queryParams, service);

            return response;
        }
        /// <summary>
        /// لیست افراد تحت تکفل پیمانکاران
        /// </summary>
        public async Task<IsSuiteResponse<ContractFamiliesVM>> GetContractorFamiliesViwAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.emp_cont_familys_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<ContractFamiliesVM>>(url, queryParams, service);

            return response;
        }
        /// <summary>
        /// لیست قراردادها
        /// </summary>
        public async Task<IsSuiteResponse<PerContractVM>> GetPerContractInfoViwAsync(List<QueryParamModel> queryParams)
        {
            var (url, service) = FetchIsSuiteUrlForApi(IsSuiteUrlKeyEnum.cpm_cpmper_contract_info_viw);
            var response = await _clientMethods.GetService<IsSuiteResponse<PerContractVM>>(url, queryParams, service);

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
