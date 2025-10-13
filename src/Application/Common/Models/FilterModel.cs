namespace ContractorBackend.Application.Common.Models
{
    public class FilterModel
    {
        public string ColumnName { get; set; } = null!;
        public object Value { get; set; } = null!;
        public int SearchType { get; set; }
    }
}
