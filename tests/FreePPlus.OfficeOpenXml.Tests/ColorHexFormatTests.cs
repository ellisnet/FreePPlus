using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Globalization;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Formatting of packed ARGB colours as hex strings.
/// </summary>
/// <remarks>
///     ToArgbInt32 packs alpha into the top byte and int.ToString("X") emits no leading zeros, so an alpha
///     below 16 produced a string shorter than eight characters. Downstream that threw where the code took a
///     fixed-offset slice, silently shifted the colour where it sliced with a range operator, and wrote a
///     malformed attribute where it wrote the string whole. Color.Transparent has alpha 0, so every one of
///     those was reachable from ordinary caller input.
/// </remarks>
public class ColorHexFormatTests
{
    /// <summary>Alphas below 16 are the ones that used to truncate; the rest guard against regressions.</summary>
    public static TheoryData<int> Alphas => new() { 0, 1, 9, 15, 16, 17, 128, 254, 255 };

    #region Sites that used to throw ArgumentOutOfRangeException on a fixed-offset slice

    [Theory]
    [MemberData(nameof(Alphas))]
    public void ChartTitleFontColorAcceptsEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var chart = ws.Drawings.AddChart("chart", eChartType.ColumnClustered);
        chart.Title.Text = "Title";

        //Act
        chart.Title.Font.Color = Color.FromArgb(alpha, 51, 102, 153);

        //Assert - the six RGB digits survive whatever the alpha was
        var doc = PackageXmlInspector.GetPart(pck, "xl/charts/chart1.xml");
        var written = doc.SelectSingleNode("//c:title//a:rPr/a:solidFill/a:srgbClr/@val",
            PackageXmlInspector.NamespaceManager(doc))?.Value;

        Assert.Equal("336699", written);
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void ShapeFontColorAcceptsEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var shape = ws.Drawings.AddShape("shape", eShapeStyle.Rect);

        //Act
        shape.Font.Color = Color.FromArgb(alpha, 51, 102, 153);
        shape.Font.UnderLineColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), shape.Font.Color.ToRgba32());
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void CommentBackgroundColorAcceptsEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var comment = ws.Cells["A1"].AddComment("text", "author");

        //Act
        comment.BackgroundColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        var vml = PackageXmlInspector.GetPartText(pck, "xl/drawings/vmlDrawing1.vml");
        Assert.Contains("#336699", vml);
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void CommentLineColorAcceptsEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var comment = ws.Cells["A1"].AddComment("text", "author");

        //Act
        comment.LineColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        var vml = PackageXmlInspector.GetPartText(pck, "xl/drawings/vmlDrawing1.vml");
        Assert.Contains("#336699", vml);
    }

    #endregion

    #region Sites that used to write a hex string shorter than the eight ARGB digits OOXML expects

    [Theory]
    [MemberData(nameof(Alphas))]
    public void TabColorWritesEightHexDigits(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        //Act
        ws.TabColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        var doc = PackageXmlInspector.GetPart(pck, "xl/worksheets/sheet1.xml");
        var written = doc.SelectSingleNode("//d:sheetPr/d:tabColor/@rgb",
            PackageXmlInspector.NamespaceManager(doc))?.Value;

        Assert.NotNull(written);
        Assert.Equal(8, written.Length);
        Assert.Equal(alpha.ToString("X2", CultureInfo.InvariantCulture) + "336699", written);
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void CellFontColorWritesEightHexDigits(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "text";

        //Act
        ws.Cells["A1"].Style.Font.Color.SetColor(Color.FromArgb(alpha, 51, 102, 153));

        //Assert
        var doc = PackageXmlInspector.GetPart(pck, "xl/styles.xml");
        var written = doc.SelectSingleNode("//d:fonts/d:font/d:color[@rgb]/@rgb",
            PackageXmlInspector.NamespaceManager(doc))?.Value;

        Assert.NotNull(written);
        Assert.Equal(8, written.Length);
        Assert.Equal(alpha.ToString("X2", CultureInfo.InvariantCulture) + "336699", written);
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void CellFillColorWritesEightHexDigits(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;

        //Act
        ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(alpha, 51, 102, 153));

        //Assert
        var doc = PackageXmlInspector.GetPart(pck, "xl/styles.xml");
        var written = doc.SelectSingleNode("//d:fills/d:fill/d:patternFill/d:fgColor/@rgb",
            PackageXmlInspector.NamespaceManager(doc))?.Value;

        Assert.NotNull(written);
        Assert.Equal(8, written.Length);
        Assert.Equal(alpha.ToString("X2", CultureInfo.InvariantCulture) + "336699", written);
    }

    [Theory]
    [MemberData(nameof(Alphas))]
    public void CellBorderColorWritesEightHexDigits(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;

        //Act
        ws.Cells["A1"].Style.Border.Top.Color.SetColor(Color.FromArgb(alpha, 51, 102, 153));

        //Assert
        var doc = PackageXmlInspector.GetPart(pck, "xl/styles.xml");
        var written = doc.SelectSingleNode("//d:borders/d:border/d:top/d:color/@rgb",
            PackageXmlInspector.NamespaceManager(doc))?.Value;

        Assert.NotNull(written);
        Assert.Equal(8, written.Length);
        Assert.Equal(alpha.ToString("X2", CultureInfo.InvariantCulture) + "336699", written);
    }

    #endregion

    #region Getters whose setter stores six RGB digits must not report the colour as transparent

    /// <remarks>
    ///     Same defect as the chart series getters: the setter strips the alpha byte and stores six hex
    ///     digits, and the getter fed those straight into the packed-ARGB overload, which read the missing
    ///     top byte as alpha 0. Any caller reading one of these colours back got a fully transparent colour,
    ///     including one it had just written.
    /// </remarks>
    [Fact]
    public void ShapeFontColorReadsBackOpaque()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var shape = ws.Drawings.AddShape("shape", eShapeStyle.Rect);

        //Act
        shape.Font.Color = Color.FromArgb(255, 51, 102, 153);
        shape.Font.UnderLineColor = Color.FromArgb(255, 51, 102, 153);

        //Assert
        Assert.Equal(255, shape.Font.Color.ToRgba32().A);
        Assert.Equal(255, shape.Font.UnderLineColor.ToRgba32().A);
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), shape.Font.Color.ToRgba32());
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), shape.Font.UnderLineColor.ToRgba32());
    }

    [Fact]
    public void CommentColorsReadBackOpaque()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var comment = ws.Cells["A1"].AddComment("text", "author");

        //Act
        comment.BackgroundColor = Color.FromArgb(255, 51, 102, 153);
        comment.LineColor = Color.FromArgb(255, 51, 102, 153);

        //Assert
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), comment.BackgroundColor.ToRgba32());
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), comment.LineColor.ToRgba32());
    }

    [Fact]
    public void DrawingFillColorReadsBackOpaque()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var shape = ws.Drawings.AddShape("shape", eShapeStyle.Rect);

        //Act
        shape.Fill.Color = Color.FromArgb(255, 51, 102, 153);

        //Assert
        Assert.Equal(255, shape.Fill.Color.ToRgba32().A);
        Assert.Equal(Color.FromArgb(255, 51, 102, 153).ToRgba32(), shape.Fill.Color.ToRgba32());
    }

    #endregion

    #region Color.Transparent - alpha 0 - is ordinary caller input and must not throw anywhere

    [Fact]
    public void TransparentColorIsAcceptedAcrossTheColourApis()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "text";
        ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        var comment = ws.Cells["B2"].AddComment("text", "author");
        var shape = ws.Drawings.AddShape("shape", eShapeStyle.Rect);

        //Act - none of these may throw
        ws.TabColor = Color.Transparent;
        ws.Cells["A1"].Style.Font.Color.SetColor(Color.Transparent);
        ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.Transparent);
        ws.Cells["A1"].Style.Border.Top.Color.SetColor(Color.Transparent);
        comment.BackgroundColor = Color.Transparent;
        comment.LineColor = Color.Transparent;
        shape.Font.Color = Color.Transparent;
        shape.Font.UnderLineColor = Color.Transparent;

        //Assert - and the package still saves
        var bytes = pck.GetAsByteArray();
        Assert.NotEmpty(bytes);
    }

    #endregion
}
