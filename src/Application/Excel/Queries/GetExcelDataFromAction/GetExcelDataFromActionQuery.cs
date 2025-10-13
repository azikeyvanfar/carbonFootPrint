using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Resources;
using ContractorBackend.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace ContractorBackend.Application.Excel.Queries.GetExcelDataFromAction
{
    public class GetExcelDataFromActionQuery : IRequest<string>
    {
        [Required(ErrorMessage = "RequestUrl is Required")]
        public string RequestUrl { get; set; }
        public List<KeyPersian>? RequestedProperties { get; set; }
    }
    public class KeyPersian
    {
        [Required(ErrorMessage = "Key is Required")]
        public string Key { get; set; }
        public string Persian { get; set; }
    }

    public class GetExcelDataFromActionQueryHandler : IRequestHandler<GetExcelDataFromActionQuery, string>
    {
        private readonly IMapper _mapper;
        private readonly HttpClientMethods _clientMethods;
        private readonly IHttpContextAccessor _accessor;
        private readonly IStringLocalizer<SharedResource> _localizer;


        public GetExcelDataFromActionQueryHandler(
            IMapper mapper,
            HttpClientMethods clientMethods,
            IHttpContextAccessor accessor,
             IStringLocalizer<SharedResource> localizer)
        {
            _mapper = mapper;
            _clientMethods = clientMethods;
            _accessor = accessor;
            _localizer = localizer;
        }

        public async Task<string> Handle(GetExcelDataFromActionQuery request, CancellationToken cancellationToken)
        {
            var resultList = await CallAPIAndGetDataList(request);

            List<string> alphabet = new() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" ,
                                            "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ" };

            #region Deduct data with Requested Properties
            var resultStrippedList = new List<List<PropertyObject<object>>>();

            for (int i = 0; i < resultList.Count; i++)
            {
                var rowStripped = new List<PropertyObject<object>>();
                for (int col = 0; col < resultList[i].Count; col++)
                {
                    var dict = resultList[i][col];
                    if (IsInRequestedProperties(dict, request.RequestedProperties) && dict.IsPrimitive)
                    {
                        rowStripped.Add(dict);
                    }
                }

                #region Order object(list of props) by index of RequestedProperties
                var sortOrder = request.RequestedProperties.Select(x => x.Key).ToList();
                rowStripped = rowStripped.OrderBy(x => sortOrder.IndexOf(x.Key)).ToList();
                #endregion

                resultStrippedList.Add(rowStripped);

            }
            #endregion

            try
            {
                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    #region  Add Header row to Sheet
                    var headerRow = resultStrippedList[0];
                    for (int col = 0; col < headerRow.Count; col++)
                    {
                        var dict = headerRow[col];
                        if (IsInRequestedProperties(dict, request.RequestedProperties) && dict.IsPrimitive)
                        {
                            worksheet.Cells[alphabet[col] + 1].Value = request.RequestedProperties?.FirstOrDefault(x => x.Key == dict?.Key)?.Persian ?? dict?.Key;
                        }
                    }
                    #endregion

                    #region  Add Data Rows to Sheet
                    for (int i = 0; i < resultStrippedList.Count; i++)
                    {
                        var row = resultStrippedList[i];

                        for (int j = 0; j < row.Count; j++)
                        {
                            var dict = row[j];
                            if (IsInRequestedProperties(dict, request?.RequestedProperties) && dict.IsPrimitive)
                            {
                                worksheet.Cells[alphabet[j] + (i + 2)].Value = dict?.Value?.ToString();
                            }
                        }
                    }
                    #endregion

                    package.Save();
                }
                stream.Position = 0;

                byte[] bytes = stream.ToArray();
                string base64String = Convert.ToBase64String(bytes);

                return base64String;
            }
            catch (Exception)
            {
                throw;
            }

            throw new CustomException("");
        }

        private async Task<List<List<PropertyObject<object>>>> CallAPIAndGetDataList(GetExcelDataFromActionQuery request)
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
                throw new CustomException(_localizer["TokenIsEmpty"]);
            }

            List<List<PropertyObject<object>>> resultListCollection = new();
            List<List<PropertyObject<object>>> resultList = new();
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

                var listJson = GetItemsFromResponseString(res);

                resultList = CreateListOfDynamicProperTyObject(listJson);

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
        private JToken GetItemsFromResponseString(string res)
        {
            try
            {
                JObject masterObject = JObject.Parse(res);
                var masterObjectData = masterObject["data"];

                return masterObjectData["items"];
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

        private bool IsInRequestedProperties(PropertyObject<object> dict, List<KeyPersian>? requestedProperties)
        {
            if (requestedProperties == null || requestedProperties.Count < 1)
            {
                return true;
            }

            try
            {
                if (requestedProperties.Find(x => x.Key.ToLowerInvariant() == dict.Key.ToLowerInvariant()) != null)
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
