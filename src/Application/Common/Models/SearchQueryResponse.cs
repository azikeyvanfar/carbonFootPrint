using System;
using System.Collections.Generic;
using Gridify;
using Newtonsoft.Json;


namespace ContractorBackend.Application.Common.Models
{
    public class SearchQueryResponse<T>
    {
        // تابع سازنده
        public SearchQueryResponse(SearchQueryRequest searchQueryRequest, Paging<T> paging)
        {
            Page = searchQueryRequest.Page != 0 ? searchQueryRequest.Page : 1;
            PageSize = searchQueryRequest.PageSize != 0 ? searchQueryRequest.PageSize : 10;
            Filter = searchQueryRequest.Filter;
            //PageSize = 10;
            Items = paging.Data;
            TotalCount = paging.Count;
            var filters = string.Empty;
            //var filters = !string.IsNullOrEmpty(searchQueryRequest.Filter) ? searchQueryRequest.Filter.ToJson() : null;

            if (!string.IsNullOrEmpty(filters))
            {
                ColumnsFilter = JsonConvert.DeserializeObject<List<FilterModel>>(filters);
            }

        }

        /// <summary>
        /// صفحه
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// اندازه صفحه
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// آیتم ها
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// تعداد کل
        /// </summary>
        public long TotalCount { get; set; }

        public string Filter { get; set; }

        /// <summary>
        /// تعداد کل نمایشی
        /// </summary>
        public string TotalCountDisplay => $"تعداد کل : {TotalCount}";

        /// <summary>
        /// تعداد کل صفحات
        /// </summary>
        public int TotalPages => Convert.ToInt32(Math.Ceiling(decimal.Divide(TotalCount, PageSize)));

        /// <summary>
        /// صفحه قبلی دارد
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// صفحه بعدی دارد
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// تعداد کل صفحات نمایشی
        /// </summary>
        public string TotalPagesDisplay => $"تعداد کل صفحات : {TotalPages}";

        /// <summary>
        /// شمارنده صفحه
        /// </summary>
        public string PageCountAndCurrentLocationDisplay => $@"صفحه {Page} از {TotalPages}";

        public long RowCounterNumber
        {
            get
            {
                if (Page == 0)
                {
                    return 1;
                }

                return (Page - 1) * PageSize + 1;
            }
        }

        public List<FilterModel>? ColumnsFilter { get; set; }
    }
}
