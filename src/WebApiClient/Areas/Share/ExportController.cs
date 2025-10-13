using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ContractorBackend.Application.Core.Excel.Queries.GetExcelDataFromAction;
using ContractorBackend.Application.Core.Excel.Queries.GetPdfDataFromAction;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;


namespace ContractorBackend.WebApiClient.Areas.Share
{
    [Area("Share")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class ExportController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ExportController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// UI CODE :22-01 
        /// خروجی اکسل دیتا های اکشن درخواستی
        /// </summary>
        /// <param name="query"></param> 
        /// <returns></returns>
        [HttpPost]
        [DisplayName("خروجی اکسل دیتا های اکشن درخواستی")]
        [ErrorCode("22-01")]
        [IsGlobal]
        public async Task<IActionResult> GetExcel([FromBody] GetExcelDataFromActionQuery query)
        {
            var res = await Mediator.Send(query);
            // todo: COMMENT ME
            //#region Test with Export File
            //var bytes = Convert.FromBase64String(res); 
            //var stream = new MemoryStream(bytes); 
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
            //#endregion
            return Ok(res);
        }

        /// <summary>
        /// UI CODE :22-01 
        /// خروجی اکسل دیتا های اکشن درخواستی
        /// </summary>
        /// <param name="query"></param> 
        /// <returns></returns>
        [HttpPost]
        [DisplayName("خروجی اکسل دیتا های اکشن درخواستی")]
        [ErrorCode("22-01")]
        [AllowAnonymous]
        [Obsolete("DO NOT Use This. Only Useable For Backend")]
        public async Task<IActionResult> GetExcelFile([FromBody] GetExcelDataFromActionQuery query)
        {
            if (!_env.IsDevelopment())
            {
                throw new NotImplementedException();
            }

            var res = await Mediator.Send(query);
            // todo: COMMENT ME
            #region Test with Export File
            var bytes = Convert.FromBase64String(res);
            var stream = new MemoryStream(bytes);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
            #endregion
            //return Ok(res);
        }

        /// <summary>
        /// UI CODE :22-02 
        /// خروجی پی دی اف دیتا های اکشن درخواستی
        /// </summary>
        /// <param name="query"></param> 
        /// <returns></returns>
        [HttpPost]
        [DisplayName("خروجی پی دی اف دیتا های اکشن درخواستی")]
        [ErrorCode("22-02")]
        [IsGlobal]
        public async Task<IActionResult> GetPdf([FromBody] GetPdfDataFromActionQuery query)
        {
            var res = await Mediator.Send(query);
            // todo: COMMENT ME
            //#region Test with Export File
            //var bytes = Convert.FromBase64String(res); 
            //var stream = new MemoryStream(bytes); 
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
            //#endregion
            return Ok(res);
        }


        /// <summary>
        /// UI CODE :22-02 
        /// خروجی پی دی اف دیتا های اکشن درخواستی
        /// </summary>
        /// <param name="query"></param> 
        /// <returns></returns>
        [HttpPost]
        [DisplayName("خروجی پی دی اف دیتا های اکشن درخواستی")]
        [ErrorCode("22-02")]
        [AllowAnonymous]
        [Obsolete("DO NOT Use This. Only Useable For Backend")]
        public async Task<IActionResult> GetPdfFile([FromBody] GetPdfDataFromActionQuery query)
        {
            if (!_env.IsDevelopment())
            {
                throw new NotImplementedException();
            }

            var res = await Mediator.Send(query);
            // todo: COMMENT ME
            #region Test with Export File
            var bytes = Convert.FromBase64String(res);
            var stream = new MemoryStream(bytes);
            return File(stream, "application/pdf", $"{query.reportFarsiName}.pdf");
            #endregion
            //return Ok(res);
        }











        ///// <summary>
        ///// فرم شناسایی و ارزیابی فرصت های ایمنی و بهداشت
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("فرم شناسایی و ارزیابی فرصت های ایمنی و بهداشت")]
        //[ErrorCode("46-01")]
        //public async Task<IActionResult> GetReport1([FromBody] GetReport1Query query)
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        throw new NotImplementedException();
        //    }

        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    #region Test with Export File
        //    var bytes = Convert.FromBase64String(res);
        //    var stream = new MemoryStream(bytes);
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
        //    #endregion
        //    //return Ok(res);
        //}


        ///// <summary>
        ///// تعداد خطرات به تفکیک طبقه و سطح ریسک
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("تعداد خطرات به تفکیک طبقه و سطح ریسک")]
        //[ErrorCode("46-01")]
        //public async Task<IActionResult> GetReport2([FromQuery] GetReport2Query query)
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        throw new NotImplementedException();
        //    }

        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    #region Test with Export File
        //    var bytes = Convert.FromBase64String(res);
        //    var stream = new MemoryStream(bytes);
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
        //    #endregion
        //    //return Ok(res);
        //}

        ///// <summary>
        ///// گزارش خطرات در محدوده ALARP,NON-ACCEPTABLE بدون اقدام اصلاحی
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("گزارش خطرات در محدوده ALARP,NON-ACCEPTABLE بدون اقدام اصلاحی")]
        //[ErrorCode("46-01")]
        //public async Task<string> GetAllRiskAlarpNonAcceptableWithoutPlanningReport([FromQuery] GetAllRiskAlarpNonAcceptableWithoutPlanningReportQuery query)
        ////public async Task<IActionResult> GetAllRiskAlarpNonAcceptableWithoutPlanningReport([FromQuery] GetAllRiskAlarpNonAcceptableWithoutPlanningReportQuery query)
        //{


        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    //#region Test with Export File
        //    //var bytes = Convert.FromBase64String(res);
        //    //var stream = new MemoryStream(bytes);
        //    //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
        //    //#endregion

        //    return res;
        //}


        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    #region Test with Export File
        //    var bytes = Convert.FromBase64String(res);
        //    var stream = new MemoryStream(bytes);
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExport.xlsx");
        //    #endregion
        //    //return Ok(res);
        //}


        ///// <summary>
        ///// خطرات با سطح ریسک
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("خطرات با سطح ریسک")]
        //[ErrorCode("46-02")]
        //public async Task<OkApiResult<SearchQueryResponse<RiskManagementReportRisksDto>>> GetRisks([FromQuery] GetRisksQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<RiskManagementReportRisksDto>>(await Mediator.Send(query));

        //}
        ///// <summary>
        ///// خطرات با سطح ریسک
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("خطرات با سطح ریسک")]
        //[ErrorCode("46-01")]
        //public async Task<IActionResult> GetReportRisks([FromQuery] GetReportRisksQuery query)
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        throw new NotImplementedException();
        //    }


        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    //#region Test with Export File
        //    //var bytes = Convert.FromBase64String(res);
        //    //var stream = new MemoryStream(bytes);
        //    //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExportRisks.xlsx");
        //    //#endregion
        //    return Ok(res);
        //}

        ///// <summary>
        ///// پیگیری انجام اقدامات اصلاحی
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("پیگیری انجام اقدامات اصلاحی")]
        //[ErrorCode("46-02")]
        //public async Task<OkApiResult<SearchQueryResponse<ReportCorrectiveDto>>> GetCorrectives([FromQuery] GetCorrectivesQuery query)
        //{
        //    return new OkApiResult<SearchQueryResponse<ReportCorrectiveDto>>(await Mediator.Send(query));

        //}
        ///// <summary>
        /////گزارش پیگیری انجام اقدامات اصلاحی
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception> 
        //[HttpGet]
        //[DisplayName("پیگیری انجام اقدامات اصلاحی")]
        //[ErrorCode("46-01")]
        //public async Task<IActionResult> GetReportCorrective([FromQuery] GetReportCorrectiveQuery query)
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        throw new NotImplementedException();
        //    }


        //    var res = await Mediator.Send(query);
        //    // todo: COMMENT ME
        //    //#region Test with Export File
        //    //var bytes = Convert.FromBase64String(res);
        //    //var stream = new MemoryStream(bytes);
        //    //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "GridExportCorrective.xlsx");
        //    //#endregion
        //    return Ok(res);
        //}


    }
}