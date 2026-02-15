using OfficeOpenXml;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class ExcelStyleTests
{
    [Fact]
    public void QuotePrefixStyle()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("QuotePrefixTest");
        var cell = ws.Cells["B2"];
        cell.Style.QuotePrefix = true;

        Assert.True(cell.Style.QuotePrefix);

        p.Workbook.Styles.UpdateXml();

        var nodes = p.Workbook.StylesXml.SelectNodes("//d:cellXfs/d:xf", p.Workbook.NameSpaceManager);

        // Since the quotePrefix attribute is not part of the default style,
        // a new one should be created and referenced.
        Assert.NotEqual(0, cell.StyleID);
        Assert.Null(nodes[0].Attributes["quotePrefix"]);
        Assert.Equal("1", nodes[cell.StyleID].Attributes["quotePrefix"].Value);
    }
}
