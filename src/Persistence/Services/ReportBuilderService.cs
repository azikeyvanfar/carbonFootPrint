using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Table;
using FastReport.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ReportServer.Services
{
    public class ReportBuilderService : IReportBuilderService
    {
        private readonly ILogger<ReportBuilderService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ReportBuilderService(
            ILogger<ReportBuilderService> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            IMapper mapper
            )
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
            _mapper = mapper;
        }




        /// <summary>
        /// ساخت ریپورت pdf
        /// </summary>
        /// <param name="dynamicList">لیست اطلاعات برای ریپورت</param>
        /// <param name="templateName">نام فایل تمئلیت</param>
        /// <returns></returns>
        public string? GenerateReportFromTemplate(List<dynamic>? dynamicList, List<ColumnOptionDto> columns, string reportFarsiName, string templateName)
        {
            ArgumentNullException.ThrowIfNull(columns);
            try
            {
                List<object> lst = new();
                lst = dynamicList;
                //foreach (var row in dynamicList)
                //{
                //    IDictionary<string, object> rowObject = new ExpandoObject();
                //    foreach (var property in ((IDictionary<string, object>)row).ToList())
                //    {
                //        var val = ((IDictionary<string, object>)row)[property.Key] ?? "-";
                //        if (DateTime.TryParse(val.ToString(), out DateTime now))
                //        {
                //            var pc = new PersianCalendar();
                //            var nowDateStr = pc.GetYear(now).ToString("0000") + "/" + pc.GetMonth(now).ToString("00") + "/" + pc.GetDayOfMonth(now).ToString("00");
                //            var nowTimeStr = now.ToString("HH:mm:ss");
                //            var newDatetime = nowDateStr + " " + nowTimeStr;

                //            row.Add(new KeyValuePair<string, object>(property.Key + "qqq", newDatetime));
                //            rowObject[property.Key] = newDatetime;
                //        }
                //        //else if(bool.TryParse(val.ToString(),out bool boolData))
                //        //{
                //        //    rowObject[property.Key] = boolData;
                //        //}
                //        else
                //        {
                //            rowObject[property.Key] = val;

                //        }
                //    }
                //    lst.Add(rowObject);
                //}


                var json = JsonConvert.SerializeObject(lst);

                var dataBand = CreateDataBandFromJsonList(json, columns);

                FastReport.Report report = new();
                report.Load(Path.Combine(_environment.ContentRootPath, "TemplateFrx", templateName /*"Template1.frx"*/));

                TextObject? ReportHeaderName = report.FindObject("ReportHeaderName") as TextObject;
                ReportHeaderName.Text = reportFarsiName;


                TextObject? TotalCount = report.FindObject("TotalCount") as TextObject;
                TotalCount.Text = dynamicList.Count.ToString();




                ReportPage page = new ReportPage();

                foreach (ReportPage mypage in report.Pages)
                {
                    if (mypage.Name == "Page1")
                    {
                        page = mypage;
                    }
                }

                page.Bands.Add(dataBand);
                report.Pages.Add(page);

                report.Prepare();

                PDFSimpleExport export = new PDFSimpleExport();
                using (MemoryStream ms = new MemoryStream())
                {
                    export.Export(report, ms);
                    ms.Flush();
                    var base64File = Convert.ToBase64String(ms.ToArray());
                    return base64File;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }








        public class KeyPersian
        {
            public string Key { get; set; }
            public string Persian { get; set; }
        }





        /// <summary>
        /// ساخت دیتا بند از لیست اطلاعات
        /// </summary>
        /// <param name="json">لیست اطلاعات</param>
        /// <returns></returns>
        private DataBand CreateDataBandFromJsonList(string json, List<ColumnOptionDto> columns)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

            DataBand dataBand = new DataBand();
            dataBand.Name = "dataBand";
            dataBand.CanGrow = true;
            int rowCount = dt.Rows.Count;


            //-- +1 for row number
            int columnCount = dt.Columns.Count + 1;

            TableObject table = new TableObject();

            table.Columns.Insert(0, new TableColumn() { Width = Units.Millimeters * 15, Name = "index" });
            table.Left = Units.Centimeters * 0.2f;
            for (int i = 1; i < columnCount; i++)
            {
                TableColumn tableColumn = new TableColumn();
                tableColumn.Width = Units.Millimeters * ((float)258 / (columnCount - 1));
                tableColumn.Height = 10000f;
                table.Columns.Insert(i, tableColumn);
            }

            int detailRow = 1;

            // create DataBand --> create table --> and then creating rows and cells
            for (int row = 0; row <= rowCount; row++)
            {
                TableRow tableRow = new TableRow();

                //tableRow.Height = row == 0 ? Units.Centimeters * 1f : Units.Centimeters * 1f;
                tableRow.AutoSize = true;
                table.Rows.Insert(row, tableRow);

                //-- for row number
                table[0, row].Border.Lines = BorderLines.All;
                table[0, row].HorzAlign = HorzAlign.Center;
                table[0, row].VertAlign = VertAlign.Center;
                table[0, row].ParagraphFormat = new ParagraphFormat();
                table[0, row].ParagraphFormat.LineSpacing = Units.Centimeters * .1f;
                table[0, row].Font = new Font("Calibri", 10, FontStyle.Regular);

                //-- col start from 1 for row number
                for (int col = 1; col < columnCount; col++)
                {
                    table[col, row].AllowExpressions = false;

                    table[col, row].Border.Lines = BorderLines.All;
                    table[col, row].HorzAlign = HorzAlign.Center;
                    table[col, row].VertAlign = VertAlign.Center;
                    table[col, row].ParagraphFormat = new ParagraphFormat();
                    table[col, row].ParagraphFormat.LineSpacing = Units.Centimeters * .1f;
                    table[col, row].Font = new Font("Calibri", 10, FontStyle.Regular);

                    if (row % 2 == 0)
                    {
                        table[col, row].FillColor = Color.LightGray;

                        //-- for row number
                        table[0, row].FillColor = Color.LightGray;
                        //--
                    }

                    if (row >= 1)
                    {
                        var val = dt.Rows[row - 1][col - 1];
                        //-- col -1 for row number
                        table[col, row].Text = dt.Rows[row - 1][col - 1].ToString();
                    }
                    else if (row == 0)
                    {
                        var column = columns.FirstOrDefault(x => x.key == dt.Columns[col - 1].ToString());
                        var persianText = column == null ? dt.Columns[col - 1].ToString() : column.persian;
                        //-- col -1 for row number
                        table[col, row].Text = persianText; // dt.Columns[col - 1].ToString();

                        table[col, row].FillColor = Color.LightBlue;
                        table[col, row].Font = new Font("Calibri", 12, FontStyle.Bold);
                    }
                }

                //-- for row number
                if (row != 0)
                {
                    table[0, row].Text = detailRow.ToString();
                    detailRow++;
                }
                //--
            }

            //-- for row number
            table[0, 0].Font = new Font("Calibri", 9, FontStyle.Bold);
            table[0, 0].FillColor = Color.LightBlue;
            table[0, 0].Text = "ردیف";

            //var           columnList = GetColumnList() 

            dataBand.AddChild(table);

            return dataBand;
        }



    }
}
