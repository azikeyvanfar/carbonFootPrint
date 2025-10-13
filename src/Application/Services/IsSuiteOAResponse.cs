using System.Collections.Generic;

namespace ContractorBackend.Application.Services
{
    public class IsSuiteOAResponse<T> where T : class
    {
        public List<T> Table { get; set; }

        public bool HasMore { get; set; }

        public int Limit { get; set; }
        public int offset { get; set; }

        public int Count
        {
            get
            {
                try
                {
                    return Table.Count > 0 ?
                        int.Parse(Table[0].GetType()?.GetProperty("TotalCount")?.GetValue(Table[0], null)?.ToString() ?? "-1")
                : -2;
                }
                catch (System.Exception)
                {
                    return -3;
                }

            }
            set { }
        }

    }
}