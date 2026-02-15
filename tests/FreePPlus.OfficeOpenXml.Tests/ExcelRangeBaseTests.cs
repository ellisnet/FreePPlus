using OfficeOpenXml;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class ExcelRangeBaseTests
{
    #region Copy Comment Tests

    [Fact]
    public void CopyCopiesCommentsFromSingleCellRanges()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var sourceCell = ws.Cells["C3"];
        sourceCell.AddComment("Comment text", "Author");

        ws.Cells["C3"].Copy(ws.Cells["E5"]);

        Assert.NotNull(ws.Cells["C3"].Comment);
        Assert.Equal("Comment text", ws.Cells["C3"].Comment.Text);
        Assert.NotNull(ws.Cells["E5"].Comment);
        Assert.Equal("Comment text", ws.Cells["E5"].Comment.Text);
    }

    [Fact]
    public void CopyCopiesCommentsFromMultiCellRanges()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["C3"].AddComment("C3 comment", "Author");
        ws.Cells["D3"].AddComment("D3 comment", "Author");
        ws.Cells["E3"].AddComment("E3 comment", "Author");

        ws.Cells["C3:E3"].Copy(ws.Cells["C5"]);

        Assert.NotNull(ws.Cells["C3"].Comment);
        Assert.Equal("C3 comment", ws.Cells["C3"].Comment.Text);
        Assert.NotNull(ws.Cells["D3"].Comment);
        Assert.Equal("D3 comment", ws.Cells["D3"].Comment.Text);
        Assert.NotNull(ws.Cells["E3"].Comment);
        Assert.Equal("E3 comment", ws.Cells["E3"].Comment.Text);

        Assert.NotNull(ws.Cells["C5"].Comment);
        Assert.Equal("C3 comment", ws.Cells["C5"].Comment.Text);
        Assert.NotNull(ws.Cells["D5"].Comment);
        Assert.Equal("D3 comment", ws.Cells["D5"].Comment.Text);
        Assert.NotNull(ws.Cells["E5"].Comment);
        Assert.Equal("E3 comment", ws.Cells["E5"].Comment.Text);
    }

    #endregion

    #region Named Range Address Tests

    [Fact]
    public void SettingAddressHandlesMultiAddresses()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var name = ws.Names.Add("TestName", ws.Cells["A1"]);

        // Single address — Addresses should be null
        name.Address = "Sheet1!C3";
        Assert.Null(name.Addresses);

        // Multi address — Addresses should be populated
        name.Address = "Sheet1!C3:D3,Sheet1!E3:F3";
        Assert.NotNull(name.Addresses);

        // Back to single address — Addresses should be null again
        name.Address = "Sheet1!C3";
        Assert.Null(name.Addresses);
    }

    #endregion
}
