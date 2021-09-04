using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPPlus.Core.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace ManageNotes.Utils
{
    public class ExcelWriter
    {
        public static async Task<byte[]> GetExcelBytesAsync<T>(List<T> list)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPakage = list.ToExcelPackage();
            excelPakage.Workbook.Worksheets.First().Cells.AutoFitColumns();
            excelPakage.Workbook.Worksheets.First().View.RightToLeft = true;
            excelPakage.Workbook.Worksheets.First().Tables[0].TableStyle = TableStyles.Light13;
            var bytes =await excelPakage.GetAsByteArrayAsync();
            return bytes;
        }
    }
}