using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Util
{
   public class ExportCommon
    {
        //单元格赋值方法
        public void SetCell(ISheet sheet, List<AuditColumn> auditColumns, int rowNum, XSSFCellStyle style1)
        {
            if (auditColumns.Count > 0)
            {
                rowNum++;
                IRow titleRow = sheet.CreateRow(rowNum);
                for (int c = 0; c < auditColumns.Count; c++)
                {
                    if (!auditColumns[c].IsEnd)
                    {
                        ICell cell = titleRow.CreateCell(auditColumns[c].ColumnIndex, NPOI.SS.UserModel.CellType.String);
                        cell.SetCellValue(auditColumns[c].Caption);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(auditColumns[c].Region[0] + rowNum, auditColumns[c].Region[1] + rowNum, auditColumns[c].Region[2], auditColumns[c].Region[3]));
                        cell.CellStyle = style1;
                        SetCell(sheet, auditColumns[c].Columns, rowNum, style1);
                    }
                    else
                    {
                        ICell cell = titleRow.CreateCell(auditColumns[c].ColumnIndex, NPOI.SS.UserModel.CellType.String);
                        cell.SetCellValue(auditColumns[c].Caption);
                        //sheet.SetColumnWidth(auditColumns[c].ColumnIndex, auditColumns[c].Caption.Length * 256);
                        cell.CellStyle = style1;
                    }
                }
            }
        }
    }
    public class AuditColumn
    {
        public string Caption { get; set; }
        public string DataField { get; set; }
        public List<AuditColumn> Columns { get; set; }
        public int[] Region { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsEnd { get; set; }
    }
}
