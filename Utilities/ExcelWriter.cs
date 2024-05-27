using NPOI.HSSF.UserModel;
using System.Data;
using System.IO;

namespace Utilities
{
    public class ExcelWriter
    {
        public static void GenerateXLS(string fileName, string sheetName, DataTable dt)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            //Create new Excel Sheet
            var sheet = workbook.CreateSheet(sheetName);

            // Add the header
            var headerRow = sheet.CreateRow(0);
            for (int i = 0;i < dt.Columns.Count;i++)
            {
                headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            
            //Populate the sheet with values from the grid data
            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dr = dt.Rows[rowIndex];
                var row = sheet.CreateRow(rowIndex + 1);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(dr[i].ToString());
                }
            }

            //Write the Workbook to a memory stream
            //MemoryStream output = new MemoryStream();
            FileStream fs = new FileStream(fileName, FileMode.Create);
            workbook.Write(fs);
            fs.Flush();
            fs.Close();
        }

    }
}
