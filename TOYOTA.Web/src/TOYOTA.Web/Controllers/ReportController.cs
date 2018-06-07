using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TOYOTA.Web.Common;
using TOYOTA.Web.Common.Module;
using Npoi.Core.SS.UserModel;
using Npoi.Core.SS.Util;
using System.IO;
using Npoi.Core.HSSF.Util;
using Microsoft.AspNetCore.Hosting;
using Npoi.Core.XSSF.UserModel;
using Npoi.Core.HSSF.UserModel;
using System.Net.Http;
using System.Drawing;
using System.IO.Compression;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TOYOTA.Web.Controllers
{
    public class ReportController : Controller
    {

        private IHostingEnvironment _environment;
        public ReportController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        //[Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult REP010()
        {
            DateTime now = DateTime.Now;
            ViewBag.CurrentDate = now.ToString("yyyy-MM-dd");
            ViewBag.FirstDay = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            return View();
        }
        public IActionResult REP020()
        {
            DateTime now = DateTime.Now;
            ViewBag.CurrentDate = now.ToString("yyyy-MM-dd");
            ViewBag.FirstDay = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            return View();
        }
        public IActionResult REP030()
        {
            DateTime now = DateTime.Now;
            ViewBag.CurrentDate = now.ToString("yyyy-MM-dd");
            ViewBag.FirstDay = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            return View();
        }
        public IActionResult REP040()
        {
            DateTime now = DateTime.Now;
            ViewBag.CurrentDate = now.ToString("yyyy-MM-dd");
            ViewBag.FirstDay = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            return View();
        }
        public async Task<ActionResult> DownLoadForRename(string fileName = "", string sourcepath = "")
        {
            string contentType = "application/octet-stream";
            byte[] bytes = await CommonHelper.Current.HttpGetFileBytes(sourcepath);
            return this.File(bytes, contentType, fileName);
        }
        [HttpPost]
        public async Task<ActionResult> UploadScoreAjaxDown(string disCode, string startTime, string endTime, string status, string Pid, string disname, string name)
        {
            try
            {
                string result = await CommonHelper.GetHttpClient().GetStringAsync(CommonHelper.Current.GetAPIBaseUrl + "Tour/GetTaskListByDisIdForExcel/" + disCode + "/" + startTime + "/" + endTime + "/" + status + "/" + Pid);
                var apiResult = CommonHelper.DecodeString<APIResult>(result);
                if (apiResult.ResultCode == ResultType.Success)
                {
                    ExcelResult er = CommonHelper.DecodeString<ExcelResult>(apiResult.Body);
                    List<ResultExcelDto> listAll = er.ResultList;
                    string discode = listAll.FirstOrDefault<ResultExcelDto>().DisCode;

                    List<ResultExcelDto> list = (from a in listAll where a.Score == "0" select a).ToList<ResultExcelDto>();

                    var uploads = Path.Combine(_environment.WebRootPath, "Template");
                    var newFile = Path.Combine(uploads, "ScoreViewList_" + discode + "_" + disname + DateTime.Now.ToString("yyyyMMdd hhmmss") + ".xlsx");
                    if (!System.IO.Directory.Exists(uploads))
                    {
                        System.IO.Directory.CreateDirectory(uploads);
                    }
                    using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                    {

                        IWorkbook workbook = new XSSFWorkbook();

                        ISheet sheet1 = workbook.CreateSheet("得分登记");
                        sheet1.PrintSetup.Landscape = true;
                        sheet1.PrintSetup.PaperSize = 9;

                        IFont font_b = workbook.CreateFont();
                        font_b.FontName = "Microsoft Yahei";
                        font_b.FontHeightInPoints = 10;

                        var style1 = (XSSFCellStyle)workbook.CreateCellStyle();
                        //style1.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                        byte[] rgb = new byte[3] { 116, 163, 210 };
                        style1.SetFillForegroundColor(new XSSFColor(rgb));

                        style1.FillPattern = FillPattern.SolidForeground;
                        style1.Alignment = HorizontalAlignment.Center;
                        style1.BorderLeft = BorderStyle.Thin;
                        style1.BorderBottom = BorderStyle.Thin;
                        style1.BorderRight = BorderStyle.Thin;
                        style1.BorderTop = BorderStyle.Thin;
                        style1.SetFont(font_b);

                        var style_Stand = workbook.CreateCellStyle();
                        style_Stand.Alignment = HorizontalAlignment.Left;
                        style_Stand.WrapText = true;
                        style_Stand.SetFont(font_b);

                        var style_b2 = workbook.CreateCellStyle();
                        style_b2.WrapText = true;
                        style_b2.VerticalAlignment = VerticalAlignment.Center;
                        style_b2.SetFont(font_b);


                        var style2 = workbook.CreateCellStyle();
                        style2.Alignment = HorizontalAlignment.Left;
                        style2.VerticalAlignment = VerticalAlignment.Bottom;
                        style2.WrapText = true;
                        style2.SetFont(font_b);

                        var style3 = workbook.CreateCellStyle();
                        style3.Alignment = HorizontalAlignment.Right;
                        style3.SetFont(font_b);

                        var style4 = workbook.CreateCellStyle();
                        style4.Alignment = HorizontalAlignment.Center;
                        style4.VerticalAlignment = VerticalAlignment.Center;
                        style4.SetFont(font_b);

                        var style_t = (XSSFCellStyle)workbook.CreateCellStyle();
                        style_t.Alignment = HorizontalAlignment.Center;
                        style_t.SetFillForegroundColor(new XSSFColor(rgb));
                        //style_t.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                        style_t.FillPattern = FillPattern.SolidForeground;

                        IFont font_t = workbook.CreateFont();
                        font_t.FontHeightInPoints = 18;
                        font_t.FontName = "Microsoft Yahei";
                        style_t.SetFont(font_t);

                        //第一行，插入图片
                        IRow row_0 = sheet1.CreateRow(0);
                        row_0.Height = 600;
                        CellRangeAddress region0 = new CellRangeAddress(0, 0, 0, 8);
                        sheet1.AddMergedRegion(region0);
                        byte[] bytes = System.IO.File.ReadAllBytes(_environment.WebRootPath + "/images/project_logo.png");
                        int pictureIdx = workbook.AddPicture(bytes, PictureType.PNG);
                        var patriarch = sheet1.CreateDrawingPatriarch();
                        XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 200, 40, 0, 0, 1, 1);
                        anchor.AnchorType = AnchorType.MoveAndResize;
                        //把图片插到相应的位置
                        var pict = patriarch.CreatePicture(anchor, pictureIdx);

                        //标题
                        IRow row_t = sheet1.CreateRow(1);
                        ICell cell_t = row_t.CreateCell(0);
                        cell_t.SetCellValue("一汽丰田PCM经销店经营能力评估-待改善项报告");
                        CellRangeAddress region_t = new CellRangeAddress(1, 1, 0, 4);
                        cell_t.CellStyle = style_t;
                        sheet1.AddMergedRegion(region_t);

                        //插入标题右边的Logo
                        byte[] bytesRight = System.IO.File.ReadAllBytes(_environment.WebRootPath + "/images/pcm_logo.png");
                        int pictureIdxRight = workbook.AddPicture(bytesRight, PictureType.PNG);
                        var patriarchRight = sheet1.CreateDrawingPatriarch();
                        XSSFClientAnchor anchorRight = new XSSFClientAnchor(0, 0, 0, 0, 6, 1, 7, 3);
                        //把图片插到相应的位置
                        var pictRight = patriarch.CreatePicture(anchorRight, pictureIdxRight);
                        pictRight.Resize(2.01, 1.5);

                        IRow row = sheet1.CreateRow(2);
                        row.Height = 600;
                        ICell cell = row.CreateCell(0);
                        cell.SetCellValue("经销店代码:" + discode + "                经销店名称：" + disname + "                经销店负责人签字：________________");
                        CellRangeAddress region = new CellRangeAddress(2, 2, 0, 4);
                        cell.CellStyle = style2;
                        sheet1.AddMergedRegion(region);

                        IRow row1 = sheet1.CreateRow(3);
                        row1.Height = 600;
                        ICell cel2 = row1.CreateCell(0);
                        //cel2.CellStyle = style;
                        cel2.SetCellValue("日期：" + startTime + "              评估员：" + name);
                        cel2.CellStyle = style2;
                        CellRangeAddress region2 = new CellRangeAddress(3, 3, 0, 4);
                        sheet1.AddMergedRegion(region2);

                        var style_sub = (XSSFCellStyle)workbook.CreateCellStyle();
                        style_sub.Alignment = HorizontalAlignment.Center;
                        style_sub.SetFillForegroundColor(new XSSFColor(rgb));
                        style_sub.FillPattern = FillPattern.SolidForeground;
                        style_sub.SetFont(font_b);

                        IRow row_nouse = sheet1.CreateRow(4);
                        row_nouse.Height = 400;

                        IRow row_sub = sheet1.CreateRow(5);
                        ICell cell_sub = row_sub.CreateCell(0);
                        cell_sub.SetCellValue("待改善项汇总");
                        CellRangeAddress region_sub = new CellRangeAddress(5, 5, 0, 8);
                        cell_sub.CellStyle = style_sub;
                        sheet1.AddMergedRegion(region_sub);

                        int dataCount = 6;
                        IRow row3 = sheet1.CreateRow(dataCount);

                        //任务卡名称
                        ICell cell6 = row3.CreateCell(0);
                        cell6.SetCellValue("指标名称");
                        cell6.CellStyle = style1;
                        CellRangeAddress region6 = new CellRangeAddress(dataCount, dataCount, 0, 0);
                        sheet1.AddMergedRegion(region6);
                        sheet1.SetColumnWidth(0, 9 * 600);
                        //体系名称
                        ICell cell7 = row3.CreateCell(1);
                        cell7.SetCellValue("标准与要求");
                        cell7.CellStyle = style1;
                        CellRangeAddress region7 = new CellRangeAddress(dataCount, dataCount, 1, 1);
                        sheet1.AddMergedRegion(region7);
                        sheet1.SetColumnWidth(1, 8 * 600);
                        //检查标准
                        ICell cell8 = row3.CreateCell(2);
                        cell8.SetCellValue("过程与关键动作建议");
                        cell8.CellStyle = style1;
                        ICell cell9 = row3.CreateCell(3);
                        cell9.SetCellValue("");
                        cell9.CellStyle = style1;
                        ICell cell_9 = row3.CreateCell(4);
                        cell_9.SetCellValue("");
                        cell_9.CellStyle = style1;
                        CellRangeAddress region8 = new CellRangeAddress(dataCount, dataCount, 2, 4);
                        sheet1.AddMergedRegion(region8);
                        sheet1.SetColumnWidth(2, (15 * 600));
                        sheet1.SetColumnWidth(3, (3 * 600));
                        sheet1.SetColumnWidth(4, (10 * 600));
                        //结果
                        //ICell cell9 = row3.CreateCell(3);
                        // cell9.SetCellValue("结果");
                        // cell9.CellStyle = style1;
                        //CellRangeAddress region9 = new CellRangeAddress(dataCount, dataCount, 4, 4);
                        //sheet1.AddMergedRegion(region9);

                        //得分
                        ICell cell10 = row3.CreateCell(5);
                        cell10.SetCellValue("是否合格");
                        cell10.CellStyle = style1;
                        ICell cell101 = row3.CreateCell(6);
                        cell101.SetCellValue("");
                        cell101.CellStyle = style1;
                        CellRangeAddress region10 = new CellRangeAddress(dataCount, dataCount, 5, 6);
                        sheet1.AddMergedRegion(region10);
                        sheet1.SetColumnWidth(5, 4 * 300);
                        sheet1.SetColumnWidth(6, 4 * 300);

                        //备注
                        ICell cell11 = row3.CreateCell(7);
                        cell11.SetCellValue("备注");
                        cell11.CellStyle = style1;
                        ICell cell111 = row3.CreateCell(8);
                        cell111.SetCellValue("");
                        cell111.CellStyle = style1;
                        CellRangeAddress region11 = new CellRangeAddress(dataCount, dataCount, 7, 8);
                        sheet1.AddMergedRegion(region11);
                        sheet1.SetColumnWidth(7, 5 * 600);
                        sheet1.SetColumnWidth(8, 3 * 600);


                        int firstRow1 = dataCount + 1;
                        int lastRow1 = 0;
                        int sumCnt1 = 0;
                        int firstRow2 = dataCount + 1;
                        int lastRow2 = 0;
                        int sumCnt2 = 0;
                        int firstRow3 = dataCount + 1;
                        int lastRow3 = 0;
                        int sumCnt3 = 0;
                        //数据的动态加载
                        for (int i = 0; i < list.Count; i++)
                        {

                            object r = "r_" + i;
                            r = sheet1.CreateRow(dataCount + 1 + i);
                            int cnum = 0;
                            //计划任务名称

                            //任务卡名称
                            object c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].TCTitle);
                            (c as ICell).CellStyle = style_b2;
                            if (i == sumCnt1)
                            {
                                int cnt = (from l1 in list where l1.TCId == list[i].TCId select l1).Count();
                                sumCnt1 += cnt;
                                lastRow1 = firstRow1 + cnt - 1;
                                if (list[i].CSId != null)
                                {
                                    sheet1.AddMergedRegion(new CellRangeAddress(firstRow1, lastRow1, 0, 0));
                                }
                                firstRow1 = lastRow1 + 1;
                            }
                            cnum++;


                            //体系名称
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].TITitle);
                            (c as ICell).CellStyle = style_b2;
                            //sheet1.AddMergedRegion(new CellRangeAddress(5 + i, 5 + i, 7 + cnum, 7 + cnum));
                            if (i == sumCnt2)
                            {
                                int cnt = (from l1 in list where l1.TCId == list[i].TCId && l1.TIId == list[i].TIId select l1).Count();
                                sumCnt2 += cnt;
                                lastRow2 = firstRow2 + cnt - 1;
                                if (list[i].CSId != null)
                                {
                                    sheet1.AddMergedRegion(new CellRangeAddress(firstRow2, lastRow2, 1, 1));
                                }
                                firstRow2 = lastRow2 + 1;
                            }
                            cnum++;


                            //检查标准名称
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].CSTitle);
                            (c as ICell).CellStyle = style_Stand;
                            //sheet1.AddMergedRegion(new CellRangeAddress(dataCount + i, dataCount + i, 7 + cnum, 7 + cnum));
                            //sheet1.AddMergedRegion(new CellRangeAddress(dataCount + i, dataCount + i, 2, 3));
                            cnum++;

                            //检查标准名称
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue("");
                            (c as ICell).CellStyle = style_Stand;
                            cnum++;

                            //检查标准名称
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue("");
                            (c as ICell).CellStyle = style_Stand;
                            sheet1.AddMergedRegion(new CellRangeAddress(dataCount + 1 + i, dataCount + 1 + i, 2, 4));
                            cnum++;

                            //结果
                            /*c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].Result == "False" ? "否" : (list[i].Result == null ? "" : "是"));
                            (c as ICell).CellStyle = style4;
                            sheet1.AddMergedRegion(new CellRangeAddress(dataCount + i, dataCount + i, 7 + cnum, 7 + cnum));
                            cnum++;*/

                            //得分
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].Result == "true" ? "否" : "是");
                            (c as ICell).CellStyle = style4;
                            cnum++;

                            //得分
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue("");
                            (c as ICell).CellStyle = style4;
                            sheet1.AddMergedRegion(new CellRangeAddress(dataCount + 1 + i, dataCount + 1 + i, 5, 6));
                            cnum++;

                            //备注
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue(list[i].Remarks);
                            (c as ICell).CellStyle = style_b2;
                            cnum++;

                            //备注
                            c = "c_" + cnum;
                            c = (r as IRow).CreateCell(cnum);
                            (c as ICell).SetCellValue("");
                            (c as ICell).CellStyle = style_b2;
                            if (i == sumCnt3)
                            {
                                int cnt = (from l1 in list where l1.TCId == list[i].TCId && l1.TIId == list[i].TIId select l1).Count();
                                sumCnt3 += cnt;
                                lastRow3 = firstRow3 + cnt - 1;
                                //if (list[i].CSId != null)
                                //{
                                sheet1.AddMergedRegion(new CellRangeAddress(firstRow3, lastRow3, 7, 8));
                                //}
                                firstRow3 = lastRow3 + 1;
                            }
                            cnum++;

                        }

                        workbook.Write(fs);
                        return Json(newFile);
                    }
                }
                else
                {
                    return Json("没有查询结果.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
