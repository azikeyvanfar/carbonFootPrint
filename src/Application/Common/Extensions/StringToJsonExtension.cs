using System.Collections.Generic;
using ContractorBackend.Application.Common.Models;
using Newtonsoft.Json;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class StringToJsonExtension
    {
        public static string ToJson(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var list = new List<FilterModel>();
            var items = str.Split(',');
            foreach (var item in items)
            {
                var values = item.Split('=');
                if (values.Length > 0)
                {
                    list.Add(new FilterModel
                    {
                        ColumnName = values[0],
                        Value = values[1].Replace("*", ""),
                        SearchType = values[1].Contains("*") ? 2 : 1
                    });
                }
            }
            return JsonConvert.SerializeObject(list);
        }
    }
}
