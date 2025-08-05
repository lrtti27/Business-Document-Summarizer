using OfficeOpenXml;

namespace BusinessSearchTool.Services;

public class ExcelParserService
{
    public List<List<string>>? ExtractExcelData(Stream stream)
    {
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            return null;
        }

        int rowCount = worksheet.Dimension.Rows;
        int colCount = worksheet.Dimension.Columns;

        var data = new List<List<string>>();
        for (int row = 1; row <= rowCount; row++)
        {
            var rowData = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                rowData.Add(worksheet.Cells[row, col].Text);
            }

            data.Add(rowData);
        }

        return data;
    }
}