using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RDL;
using ClosedXML.Excel;
using System.Collections;

namespace arkAS.BLL.Core
{
    public class ExportManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public ExportManager()
        {
            db = new CoreRepository();
            _disposed = false;
            //SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //dbContext.Configuration.ProxyCreationEnabled = false;
            //ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (db != null)
                        db.Dispose();
                }
                db = null;
                _disposed = true;
            }
        }
        #endregion

        public string ExportTableToExcel(string title, string subtitle, object table)
        {
            var res = "";

            try
            {
                var g = Guid.NewGuid();
                res = string.Format("/uploads/export/{0}.xlsx", g);
                var f = HttpContext.Current.Server.MapPath(res);
                var workbook = new XLWorkbook();

                //   var list = workbook.Worksheets.Add(qType.name);
                var list = workbook.Worksheets.Add(title != "" ? title : "Экспорт");

                int titleRow = 0;
                int subtitleRow = 0;
                int tableHeaderRow = 0;
                var colCount = 0;

                var iRow = 1;
                if (title != "") {
                    titleRow = iRow;
                    var titleCell = list.Cell(iRow, 1);
                    titleCell.Value = title;
                    titleCell.Style.Font.SetFontSize(24);
                    iRow++;
                }
                if (subtitle != "")
                {
                    subtitleRow = iRow;
                    var subtitleCell = list.Cell(iRow, 1);
                    subtitleCell.Value = subtitle;
                    subtitleCell.Style.Font.SetFontSize(16);
                    iRow++;
                }
                tableHeaderRow = iRow;
                var trs = table as ArrayList;

                foreach (var tr in trs) {
                    var row = list.Row(iRow);

                    var tds = tr as ArrayList;
                    colCount = tds.Count;
                    var iCol = 1;
                    foreach (var td in tds) {
                        row.Cell(iCol).Value = td;
                        iCol++;
                        row.Style.Alignment.SetShrinkToFit();
                    }
                    iRow++;
                }

                var r = list.Range(1, 1, 1, 1);
                if (title != "")
                {
                    r = list.Range(titleRow, 1, titleRow, colCount);
                    r.Merge();
                    r.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                if (subtitle != "")
                {
                    r = list.Range(subtitleRow, 1, subtitleRow, colCount);
                    r.Merge();
                    r.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                r = list.Range(tableHeaderRow, 1, tableHeaderRow, colCount);
                r.Style.Font.SetBold();
                r.Style.Fill.SetBackgroundColor(XLColor.Amber);

                
                workbook.SaveAs(f);
            }
            catch (Exception ex) {
                RDL.Debug.LogError(ex);
                res = "";
            }
            return res;
        }

        public string ExportTableToPDF(string title, string subtitle, object table)
        {
            var res = "";

            try
            {
                if (table == null)
                {
                    throw new Exception("Попытка выгрузить в PDF пустую таблицу");
                }

                var trs = table as ArrayList;

                if (trs.Count == 0)
                {
                    throw new Exception("Попытка выгрузить в PDF пустую таблицу");
                }

                var g = Guid.NewGuid();
                res = string.Format("/uploads/export/{0}.pdf", g);

                var newFileUrl = HttpContext.Current.Server.MapPath(res);

                //Create our document object
                Document Doc = new Document(PageSize.A4,2,1,30,1);

                //Create our file stream
                using (FileStream fs = new FileStream(newFileUrl, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    //Bind PDF writer to document and stream
                    PdfWriter writer = PdfWriter.GetInstance(Doc, fs);

                    //Open document for writing
                    Doc.Open();

                    //Add a page
                    Doc.NewPage();

                    //Full path to the Unicode Arial file
                    string ARIALUNI_TFF = Path.Combine(HttpContext.Current.Server.MapPath(String.Format("/fonts/")),
                        "ARIALUNI.TTF");

                    //Create a base font object making sure to specify IDENTITY-H
                    BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    //Create a specific font object
                    Font f = new Font(bf, 12, Font.NORMAL);

                    
                    int numCols = ((ArrayList)trs[0]).Count;

                    PdfPTable newTable = new PdfPTable(numCols);

                    if (title != "")
                    {
                        PdfPCell titleCell = new PdfPCell(new Phrase(title, f));
                        titleCell.Colspan = numCols;
                        titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        titleCell.BackgroundColor = BaseColor.GRAY;
                        newTable.AddCell(titleCell);
                    }

                    if (subtitle != "")
                    {
                        PdfPCell subTitleCell = new PdfPCell(new Phrase(subtitle, f));
                        subTitleCell.Colspan = numCols;
                        subTitleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        subTitleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        subTitleCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        newTable.AddCell(subTitleCell);
                    }

                    int curRow = 0;
                    PdfPCell rowCell = new PdfPCell(new Phrase("", f));
                    foreach (var tr in trs)
                    {
                        bool GrayRow = false;
                        if (curRow % 2 == 0)
                        {
                            GrayRow = true;
                        }

                        var tds = tr as ArrayList;
                        foreach (var td in tds)
                        {
                            rowCell = new PdfPCell(new Phrase(td.ToString(), f));
                            if (curRow == 0)
                            {
                                rowCell.BackgroundColor = BaseColor.GRAY;
                            }else if (GrayRow)
                            {
                                rowCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            }
                            newTable.AddCell(rowCell);
                        }

                        curRow++;
                    }

                    Doc.Add(newTable);

                    //Close the PDF
                    Doc.Close();

                }

            }
            catch (Exception ex) {
                RDL.Debug.LogError(ex);
                res = "";
            }
            return res;
        }
    }
}