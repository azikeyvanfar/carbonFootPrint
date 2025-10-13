using System.Collections.Generic;
using ContractorBackend.Application.Dtos.Core;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IReportBuilderService
    {
        string GenerateReportFromTemplate(List<dynamic>? dynamicList, List<ColumnOptionDto> columns, string reportFarsiName, string templateName);
    }
}
