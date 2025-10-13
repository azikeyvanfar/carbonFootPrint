using System.Collections.Generic;

namespace ContractorBackend.Application.Services
{
    public class IsSuiteResponse<T> where T : class
    {
        public List<T> Items { get; set; }

        public bool HasMore { get; set; }

        public int Limit { get; set; }
        public int offset { get; set; }

        public int Count
        {
            get
            {
                try
                {
                    return Items.Count > 0 ?
                        int.Parse(Items[0].GetType()?.GetProperty("total_count")?.GetValue(Items[0], null)?.ToString() ?? "-1")
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