using Microsoft.Extensions.Logging;

namespace ContractorBackend.Common.Models.SiteSettings
{
    public class Loglevel
    {
        public LogLevel Default { get; set; }
        public LogLevel System { get; set; }
        public LogLevel Microsoft { get; set; }
    }
}
