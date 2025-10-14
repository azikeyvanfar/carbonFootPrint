namespace ContractorBackend.Application.Common.Models
{
    public class SmsRequest
    {
        public string SmsText { get; set; }
        public string Receivers { get; set; }
        //public bool IsUnicode { get; set; } = true;
        //public int IsPassword { get; set; }
    }
}
