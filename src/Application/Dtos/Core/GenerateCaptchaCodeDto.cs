namespace ContractorBackend.Application.Dtos.Core
{
    public class GenerateCaptchaCodeDto
    {

        public int First { get; set; }
        public int Second { get; set; }
        public string FinalCode
        {
            get { return (First + Second).ToString(); }
        }

    }
}
