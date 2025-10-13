using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReportServer.Persistance.Entity;

namespace ContractorBackend.Application.Excel.Queries.GetPdfDataFromAction
{
    public class GetPdfDataFromActionQuery : IRequest<string>
    {
        [Required(ErrorMessage = "RequestUrl is Required")]
        public string RequestUrl { get; set; }
        public List<ColumnOptionDto>? RequestedProperties { get; set; }
        public string reportFarsiName { get; set; }
    }

    public class GetPdfDataFromActionQueryHandler : IRequestHandler<GetPdfDataFromActionQuery, string>
    {
        private readonly IMapper _mapper;
        private readonly HttpClientMethods _clientMethods;
        private readonly IHttpContextAccessor _accessor;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IReportBuilderService _reportBuilderService;

        public GetPdfDataFromActionQueryHandler(
            IMapper mapper,
            HttpClientMethods clientMethods,
            IHttpContextAccessor accessor,
             IStringLocalizer<SharedResource> localizer,
             IReportBuilderService reportBuilderService)
        {
            _mapper = mapper;
            _clientMethods = clientMethods;
            _accessor = accessor;
            _localizer = localizer;
            _reportBuilderService = reportBuilderService;
        }

        public async Task<string> Handle(GetPdfDataFromActionQuery request, CancellationToken cancellationToken)
        {
            var resultList = await CallAPIAndGetDataList(request);


            #region Deduct data with Requested Properties
            var resultStrippedList = new List<object>();

            for (int i = 0; i < resultList.Count; i++)
            {
                JObject rowStripped = new();

                var resultListRow = resultList[i];

                var props = (resultListRow as JObject).Properties();

                foreach (var item in props)
                {
                    if (IsInRequestedProperties(item.Name, request.RequestedProperties))
                    {
                        rowStripped.Add(item);
                    }
                }

                resultStrippedList.Add(rowStripped);

            }
            #endregion


            #region Generate Pdf File with given template 
            string base64String = _reportBuilderService.GenerateReportFromTemplate(resultStrippedList, request.RequestedProperties, request.reportFarsiName, "Template1.frx");
            return base64String;
            #endregion



            throw new CustomException("");
        }

        private async Task<List<object>> CallAPIAndGetDataList(GetPdfDataFromActionQuery request)
        {
            #region Make Http Call to Project And get list Data
            string token = null;
            string authHeader = _accessor.HttpContext.Request.Headers["Authorization"];

            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            List<object> resultList = new();
            List<object> resultListCollection = new();
            var page = 1;
            var pageSize = 1000;
            do
            {
                request.RequestUrl = UpdatePagePageSizeToGetChunks(request.RequestUrl, page, pageSize);


                var res = await _clientMethods.Get(request.RequestUrl, token);
                if (res == null)
                {
                    return null;
                }

                resultList = GetItemsFromResponseString(res);

                page++;

                if (resultList == null || resultList.Count > 0)
                {
                    resultListCollection.AddRange(resultList);
                }
            }
            while (resultList.Count > 0);

            #endregion
            if (resultListCollection == null || resultListCollection.Count < 1)
            {
                throw new CustomException(_localizer["ListIsEmptyError"]);
            }
            return resultListCollection;
        }

        private string UpdatePagePageSizeToGetChunks(string requestUrl, int page, int pageSize)
        {
            var uriBuilder = new UriBuilder(requestUrl);
            var queries = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            queries["page"] = page.ToString();
            queries["pageSize"] = pageSize.ToString();

            uriBuilder.Query = queries.ToString();
            return uriBuilder.ToString();
        }

        /// <summary>
        /// تبدیل دیتای ورودی از درخواست به تایپ داینامیک JToken
        /// </summary>
        /// <param name="res">استرینگ ورودی از درخواست</param>
        /// <returns>JToken Dynamic Object</returns>
        /// <exception cref="CustomException"></exception>
        private List<object> GetItemsFromResponseString(string res)
        {
            try
            {

                JObject masterObject = JObject.Parse(res);
                var masterObjectData = masterObject["data"];
                var list = masterObjectData["items"];
                var aa = list.ToObject<List<object>>();
                return aa;
            }
            catch (Exception)
            {
                throw new CustomException(_localizer["ConvertResponseStringToJTokenError"]);
            }
        }


        /// <summary>
        /// خطا در تبدیل لیست داینامیک اکسل
        /// </summary>
        /// <param name="listJson">Jtoken Object that Contains Items From request</param>
        /// <returns>List of Items in PropertyObject Type</returns>
        /// <exception cref="CustomException"></exception>
        private List<List<PropertyObject<object>>> CreateListOfDynamicProperTyObject(JToken listJson)
        {
            try
            {
                var resultList = new List<List<PropertyObject<object>>>();

                foreach (var item in listJson)
                {
                    var rowJson = item.Children();
                    var rowObject = new List<PropertyObject<object>>();

                    foreach (var itemJson in rowJson)
                    {
                        if (itemJson is JProperty property)
                        {
                            var a = property.Name;
                            var b = GetNormalizedValue(property);
                            var couple = new PropertyObject<object>()
                            {
                                Key = a,
                                Value = b,
                                IsPrimitive = IsPrimitiveProperty(property)
                            };

                            rowObject.Add(couple);
                        }
                    }

                    resultList.Add(rowObject);
                }
                return resultList;
            }
            catch (Exception)
            {
                throw new CustomException(_localizer["CreateDynamicListOfJTokenError"]);
            }
        }

        private bool IsInRequestedProperties(string propName, List<ColumnOptionDto>? requestedProperties)
        {
            if (requestedProperties == null || requestedProperties.Count < 1)
            {
                return true;
            }

            try
            {
                if (requestedProperties.Find(x => x.key.ToLowerInvariant() == propName.ToLowerInvariant()) != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsPrimitiveProperty(JProperty property)
        {
            var primitiveList = new List<string>() {
                "None", "Property", "Comment", "Integer", "Float", "String", "Boolean", "Null", "Undefined", "Date", "Bytes", "Guid", "Uri", "TimeSpan"
            };

            try
            {
                if (primitiveList.Contains(property.Value.Type.ToString()))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetNormalizedValue(JProperty property)
        {
            try
            {
                var type = property.Value.Type;
                var val = "";
                switch (type)
                {
                    case JTokenType.Date:
                        PersianCalendar pc = new PersianCalendar();
                        var now = DateTime.Parse(property.Value.ToString());
                        val = pc.GetYear(now).ToString("0000") + "/" + pc.GetMonth(now).ToString("00") + "/" + pc.GetDayOfMonth(now).ToString("00") + " " + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
                        break;

                    //case JTokenType.None:
                    //    break;
                    //case JTokenType.Object:
                    //    break;
                    //case JTokenType.Array:
                    //    break;
                    //case JTokenType.Constructor:
                    //    break;
                    //case JTokenType.Property:
                    //    break;
                    //case JTokenType.Comment:
                    //    break;
                    //case JTokenType.Integer:
                    //    break;
                    //case JTokenType.Float:
                    //    break;
                    //case JTokenType.String:
                    //    break;
                    //case JTokenType.Boolean:
                    //    break;
                    //case JTokenType.Null:
                    //    break;
                    //case JTokenType.Undefined:
                    //    break;                              
                    //case JTokenType.Raw:
                    //    break;
                    //case JTokenType.Bytes:
                    //    break;
                    //case JTokenType.Guid:
                    //    break;
                    //case JTokenType.Uri:
                    //    break;
                    //case JTokenType.TimeSpan:
                    //    break;
                    default:
                        val = property.Value.ToString();
                        break;
                }
                return val;
            }
            catch (Exception)
            {
                return "";
            }
        }


    }

    public class PropertyObject<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public bool IsPrimitive { get; set; }
    }

}
