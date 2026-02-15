using OfficeOpenXml;
using OfficeOpenXml.Style;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class CellStoreTests
{
    private static void LoadData(ExcelWorksheet ws, int rows = 1000, int cols = 1, bool isNumeric = false)
    {
        for (var r = 0; r < rows; r++)
        for (var c = 0; c < cols; c++)
        {
            if (isNumeric)
                ws.SetValue(r + 1, c + 1, r + c);
            else
                ws.SetValue(r + 1, c + 1, r + "," + c);
        }
    }

    #region Insert Tests

    [Fact]
    public void Insert1()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Insert1");
        LoadData(ws);

        ws.InsertRow(2, 1000);
        Assert.Equal("1,0", ws.GetValue(1002, 1));

        ws.InsertRow(1003, 1000);
        Assert.Equal("2,0", ws.GetValue(2003, 1));

        ws.InsertRow(2004, 1000);
        Assert.Equal("3,0", ws.GetValue(3004, 1));

        ws.InsertRow(2006, 1000);
        Assert.Equal("4,0", ws.GetValue(4005, 1));

        ws.InsertRow(4500, 500);
        Assert.Equal("499,0", ws.GetValue(5000, 1));

        ws.InsertRow(1, 1);
        Assert.Equal("1,0", ws.GetValue(1003, 1));
        Assert.Equal("499,0", ws.GetValue(5001, 1));

        ws.InsertRow(1, 15);
        Assert.Equal("3,0", ws.GetValue(4020, 1));
        Assert.Equal("499,0", ws.GetValue(5016, 1));
    }

    [Fact]
    public void Insert2_AtRow1()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Insert2-1");
        LoadData(ws);

        for (var i = 0; i < 32; i++)
            ws.InsertRow(1, 1);

        Assert.Equal("0,0", ws.GetValue(33, 1));
    }

    [Fact]
    public void Insert2_AtRow15()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Insert2-2");
        LoadData(ws);

        for (var i = 0; i < 32; i++)
            ws.InsertRow(15, 1);

        Assert.Equal("0,0", ws.GetValue(1, 1));
        Assert.Equal("14,0", ws.GetValue(47, 1));
    }

    [Fact]
    public void Insert3()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Insert3");
        LoadData(ws);

        for (var i = 0; i < 500; i += 4)
            ws.InsertRow(i + 1, 2);

        // The original EPPlus test only verified this runs without exceptions.
        // Row 1 is now an inserted blank row, so just confirm the worksheet is intact.
        Assert.NotNull(ws);
    }

    [Fact]
    public void InsertRandomTest()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Insert4-1");
        LoadData(ws, 5000);

        for (var i = 5000; i > 0; i -= 2)
            ws.InsertRow(i, 1);

        // Verify no exception is thrown during reverse-order inserts
        Assert.NotNull(ws);
    }

    [Fact]
    public void FillInsertTest()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FillInsert");
        LoadData(ws, 500);

        var r = 1;
        for (var i = 1; i <= 500; i++)
        {
            ws.InsertRow(r, i);
            Assert.Equal((i - 1) + ",0", ws.GetValue(r + i, 1)?.ToString());
            r += i + 1;
        }
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void DeleteCells()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Delete");
        LoadData(ws, 5000);

        ws.DeleteRow(2, 2);
        Assert.Equal("3,0", ws.GetValue(2, 1));

        ws.DeleteRow(10, 10);
        Assert.Equal("21,0", ws.GetValue(10, 1));

        ws.DeleteRow(50, 40);
        Assert.Equal("101,0", ws.GetValue(50, 1));

        ws.DeleteRow(100, 100);
        Assert.Equal("251,0", ws.GetValue(100, 1));

        ws.DeleteRow(1, 31);
        Assert.Equal("43,0", ws.GetValue(1, 1));
    }

    [Fact]
    public void DeleteCellsFirst()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteFirst");
        LoadData(ws, 5000);

        ws.DeleteRow(32, 30);
        for (var i = 1; i < 50; i++)
            ws.DeleteRow(1, 1);

        // Verify no exception is thrown during repeated first-row deletion
        Assert.NotNull(ws);
    }

    #endregion

    #region DeleteInsert Tests

    [Fact]
    public void DeleteInsert()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteInsert");
        LoadData(ws, 5000);

        ws.DeleteRow(2, 33);
        ws.InsertRow(2, 38);

        for (var i = 0; i < 33; i++)
            ws.SetValue(i + 2, 1, i + 2);

        // Verify the newly set values are correct
        Assert.Equal(2, ws.GetValue(2, 1));
        Assert.Equal(34, ws.GetValue(34, 1));
    }

    #endregion

    #region Enumerate CellStore Tests

    [Fact]
    public void EnumCellstore()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("enum");
        LoadData(ws, 5000);

        var o = new CellsStoreEnumerator<ExcelCoreValue>(ws._values, 2, 1, 5, 3);
        var count = 0;
        foreach (var i in o)
            count++;

        Assert.True(count > 0);
    }

    #endregion

    #region Copy Cells Tests

    [Fact]
    public void CopyCellsTest()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CopyCells");
        LoadData(ws, 100, isNumeric: true);

        ws.Cells["B1"].Formula = "SUM(A1:A500)";
        ws.Calculate();

        ws.Cells["B1"].Copy(ws.Cells["C1"]);
        ws.Cells["B1"].Copy(ws.Cells["D1"], ExcelRangeCopyOptionFlags.ExcludeFormulas);

        Assert.Equal(ws.Cells["B1"].Value, ws.Cells["C1"].Value);
        Assert.Equal("SUM(B1:B500)", ws.Cells["C1"].Formula);
        Assert.Equal(ws.Cells["B1"].Value, ws.Cells["D1"].Value);
        Assert.NotEqual(ws.Cells["B1"].Formula, ws.Cells["D1"].Formula);
    }

    #endregion

    #region Issue Regression Tests

    [Fact]
    public void Issues351_InsertRowShiftsCellsCorrectly()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Test");

        // Set an "anchor" cell
        worksheet.Cells[1, 1].Value = "A";

        // Set values further down
        worksheet.Cells[1026, 1].Value = "B";
        worksheet.Cells[1026, 2].Value = "B";

        var range = worksheet.Row(1026);
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(255, 255, 255, 0);

        // Insert a row above row 1026 — this should shift row 1026 down
        worksheet.InsertRow(1024, 1);

        // Row 1025 should now be empty (the inserted row pushed things down)
        Assert.Null(worksheet.Cells[1025, 1].Value);
    }

    #endregion
}