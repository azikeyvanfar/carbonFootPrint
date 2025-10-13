using System;

namespace ContractorBackend.Domain.Entities.Core
{
    public class Captcha
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDate { get; set; }

        public string FinalCode { get; set; }

    }
}
