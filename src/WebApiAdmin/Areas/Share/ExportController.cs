using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ContractorBackend.Application.Excel.Queries.GetExcelDataFromAction;
using ContractorBackend.Application.Excel.Queries.GetPdfDataFromAction;
using ContractorBackend.WebApiAdmin.Controllers;
using ContractorBackend.WebApiAdmin.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;


namespace ContractorBackend.WebApiAdmin.Areas.Share
{
    [Area("Share")]
    [Route("api/[area]/[controller]/[action]")]
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
    }
}