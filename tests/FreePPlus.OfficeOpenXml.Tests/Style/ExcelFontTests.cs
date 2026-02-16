using CodeBrix.Imaging.Fonts;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests.Style;

public class ExcelFontTests
{
    /// <summary>
    ///     Font names to try when a system-installed font is needed.
    ///     Includes Windows-common fonts and Linux-common fonts.
    /// </summary>
    private static readonly string[] CommonFontNames =
    [
        "Arial", "Times New Roman", "Segoe UI", "Verdana", "Tahoma",
        "DejaVu Sans", "FreeSans"
    ];

    /// <summary>
    ///     Returns the name of the first available system font, or skips the test if none are found.
    /// </summary>
    private static string GetAvailableSystemFont()
    {
        foreach (var name in CommonFontNames)
        {
            if (SystemFonts.TryGet(name, out _))
                return name;
        }

        Assert.Skip("No common system font is available on this platform.");
        return null!; // unreachable
    }
    #region ExcelFont Property Tests

    [Fact]
    public void DefaultFontNameIsCalibri()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Equal("Calibri", ws.Cells["A1"].Style.Font.Name);
    }

    [Fact]
    public void DefaultFontSizeIs11()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Equal(11, ws.Cells["A1"].Style.Font.Size);
    }

    [Fact]
    public void FontNameCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Name = "Arial";

        Assert.Equal("Arial", ws.Cells["A1"].Style.Font.Name);
    }

    [Fact]
    public void FontSizeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Size = 14;

        Assert.Equal(14, ws.Cells["A1"].Style.Font.Size);
    }

    [Fact]
    public void FontBoldCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Bold = true;

        Assert.True(ws.Cells["A1"].Style.Font.Bold);
    }

    [Fact]
    public void FontItalicCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Italic = true;

        Assert.True(ws.Cells["A1"].Style.Font.Italic);
    }

    [Fact]
    public void FontStrikeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Strike = true;

        Assert.True(ws.Cells["A1"].Style.Font.Strike);
    }

    [Fact]
    public void FontUnderLineCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.UnderLine = true;

        Assert.True(ws.Cells["A1"].Style.Font.UnderLine);
        Assert.Equal(ExcelUnderLineType.Single, ws.Cells["A1"].Style.Font.UnderLineType);
    }

    [Fact]
    public void FontUnderLineCanBeCleared()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Font.UnderLine = true;

        ws.Cells["A1"].Style.Font.UnderLine = false;

        Assert.False(ws.Cells["A1"].Style.Font.UnderLine);
        Assert.Equal(ExcelUnderLineType.None, ws.Cells["A1"].Style.Font.UnderLineType);
    }

    [Theory]
    [InlineData(ExcelUnderLineType.Single)]
    [InlineData(ExcelUnderLineType.Double)]
    [InlineData(ExcelUnderLineType.SingleAccounting)]
    [InlineData(ExcelUnderLineType.DoubleAccounting)]
    public void FontUnderLineTypeCanBeSet(ExcelUnderLineType underLineType)
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.UnderLineType = underLineType;

        Assert.Equal(underLineType, ws.Cells["A1"].Style.Font.UnderLineType);
        Assert.True(ws.Cells["A1"].Style.Font.UnderLine);
    }

    [Fact]
    public void FontFamilyCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Family = 2;

        Assert.Equal(2, ws.Cells["A1"].Style.Font.Family);
    }

    [Theory]
    [InlineData(ExcelVerticalAlignmentFont.Subscript)]
    [InlineData(ExcelVerticalAlignmentFont.Superscript)]
    [InlineData(ExcelVerticalAlignmentFont.Baseline)]
    public void FontVerticalAlignCanBeSet(ExcelVerticalAlignmentFont alignment)
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.VerticalAlign = alignment;

        Assert.Equal(alignment, ws.Cells["A1"].Style.Font.VerticalAlign);
    }

    [Fact]
    public void FontVerticalAlignDefaultIsNone()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Equal(ExcelVerticalAlignmentFont.None, ws.Cells["A1"].Style.Font.VerticalAlign);
    }

    #endregion

    #region ExcelFont Style Isolation Tests

    [Fact]
    public void FontChangeOnOneCellDoesNotAffectAnother()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Font.Bold = true;

        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.False(ws.Cells["B1"].Style.Font.Bold);
    }

    [Fact]
    public void FontCanBeAppliedToRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1:C3"].Style.Font.Name = "Times New Roman";
        ws.Cells["A1:C3"].Style.Font.Size = 16;
        ws.Cells["A1:C3"].Style.Font.Bold = true;

        Assert.Equal("Times New Roman", ws.Cells["A1"].Style.Font.Name);
        Assert.Equal("Times New Roman", ws.Cells["C3"].Style.Font.Name);
        Assert.Equal(16, ws.Cells["B2"].Style.Font.Size);
        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.True(ws.Cells["C3"].Style.Font.Bold);
    }

    [Fact]
    public void MultipleFontPropertiesCanBeSetOnSameCell()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.Name = "Verdana";
        ws.Cells["A1"].Style.Font.Size = 12;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Italic = true;
        ws.Cells["A1"].Style.Font.Strike = true;
        ws.Cells["A1"].Style.Font.UnderLine = true;

        Assert.Equal("Verdana", ws.Cells["A1"].Style.Font.Name);
        Assert.Equal(12, ws.Cells["A1"].Style.Font.Size);
        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.True(ws.Cells["A1"].Style.Font.Italic);
        Assert.True(ws.Cells["A1"].Style.Font.Strike);
        Assert.True(ws.Cells["A1"].Style.Font.UnderLine);
    }

    #endregion

    #region ExcelFont.SetFromFont Tests

    [Fact]
    public void SetFromFontCopiesRegularFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var font = SystemFonts.CreateFont(fontName, 14f);

        ws.Cells["A1"].Style.Font.SetFromFont(font);

        Assert.Equal(fontName, ws.Cells["A1"].Style.Font.Name);
        Assert.Equal(14, ws.Cells["A1"].Style.Font.Size);
        Assert.False(ws.Cells["A1"].Style.Font.Bold);
        Assert.False(ws.Cells["A1"].Style.Font.Italic);
        Assert.False(ws.Cells["A1"].Style.Font.Strike);
        Assert.False(ws.Cells["A1"].Style.Font.UnderLine);
    }

    [Fact]
    public void SetFromFontCopiesBoldFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var font = SystemFonts.CreateFont(fontName, 12f, FontStyle.Bold);

        ws.Cells["A1"].Style.Font.SetFromFont(font);

        Assert.Equal(fontName, ws.Cells["A1"].Style.Font.Name);
        Assert.Equal(12, ws.Cells["A1"].Style.Font.Size);
        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.False(ws.Cells["A1"].Style.Font.Italic);
    }

    [Fact]
    public void SetFromFontCopiesItalicFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var font = SystemFonts.CreateFont(fontName, 10f, FontStyle.Italic);

        ws.Cells["A1"].Style.Font.SetFromFont(font);

        Assert.Equal(fontName, ws.Cells["A1"].Style.Font.Name);
        Assert.Equal(10, ws.Cells["A1"].Style.Font.Size);
        Assert.False(ws.Cells["A1"].Style.Font.Bold);
        Assert.True(ws.Cells["A1"].Style.Font.Italic);
    }

    [Fact]
    public void SetFromFontCopiesBoldItalicFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var font = SystemFonts.CreateFont(fontName, 18f, FontStyle.BoldItalic);

        ws.Cells["A1"].Style.Font.SetFromFont(font);

        Assert.Equal(fontName, ws.Cells["A1"].Style.Font.Name);
        Assert.Equal(18, ws.Cells["A1"].Style.Font.Size);
        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.True(ws.Cells["A1"].Style.Font.Italic);
    }

    #endregion

    #region ExcelFontXml Tests

    [Fact]
    public void ExcelFontXmlDefaultsAreCorrect()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");

        var fontXml = pck.Workbook.Styles.Fonts[0];

        Assert.False(fontXml.Bold);
        Assert.False(fontXml.Italic);
        Assert.False(fontXml.Strike);
        Assert.False(fontXml.UnderLine);
        Assert.Equal(ExcelUnderLineType.None, fontXml.UnderLineType);
    }

    [Fact]
    public void ExcelFontXmlSetFromFontCopiesRegularFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");
        var fontXml = pck.Workbook.Styles.Fonts[0];
        var font = SystemFonts.CreateFont(fontName, 14f);

        fontXml.SetFromFont(font);

        Assert.Equal(fontName, fontXml.Name);
        Assert.Equal(14, fontXml.Size);
        Assert.False(fontXml.Bold);
        Assert.False(fontXml.Italic);
        Assert.False(fontXml.Strike);
        Assert.False(fontXml.UnderLine);
    }

    [Fact]
    public void ExcelFontXmlSetFromFontCopiesBoldFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");
        var fontXml = pck.Workbook.Styles.Fonts[0];
        var font = SystemFonts.CreateFont(fontName, 12f, FontStyle.Bold);

        fontXml.SetFromFont(font);

        Assert.Equal(fontName, fontXml.Name);
        Assert.Equal(12, fontXml.Size);
        Assert.True(fontXml.Bold);
        Assert.False(fontXml.Italic);
    }

    [Fact]
    public void ExcelFontXmlSetFromFontCopiesItalicFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");
        var fontXml = pck.Workbook.Styles.Fonts[0];
        var font = SystemFonts.CreateFont(fontName, 10f, FontStyle.Italic);

        fontXml.SetFromFont(font);

        Assert.Equal(fontName, fontXml.Name);
        Assert.Equal(10, fontXml.Size);
        Assert.False(fontXml.Bold);
        Assert.True(fontXml.Italic);
    }

    [Fact]
    public void ExcelFontXmlSetFromFontCopiesBoldItalicFont()
    {
        var fontName = GetAvailableSystemFont();
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");
        var fontXml = pck.Workbook.Styles.Fonts[0];
        var font = SystemFonts.CreateFont(fontName, 18f, FontStyle.BoldItalic);

        fontXml.SetFromFont(font);

        Assert.Equal(fontName, fontXml.Name);
        Assert.Equal(18, fontXml.Size);
        Assert.True(fontXml.Bold);
        Assert.True(fontXml.Italic);
    }

    [Fact]
    public void ExcelFontXmlCopyCreatesIndependentCopy()
    {
        using var pck = new ExcelPackage();
        _ = pck.Workbook.Worksheets.Add("Sheet1");
        var original = pck.Workbook.Styles.Fonts[0];
        original.Bold = true;
        original.Name = "Arial";
        original.Size = 14;

        var copy = original.Copy();

        Assert.Equal("Arial", copy.Name);
        Assert.Equal(14, copy.Size);
        Assert.True(copy.Bold);

        copy.Bold = false;
        copy.Name = "Verdana";

        Assert.True(original.Bold);
        Assert.Equal("Arial", original.Name);
        Assert.False(copy.Bold);
        Assert.Equal("Verdana", copy.Name);
    }

    #endregion

    #region ExcelFontXml.GetFontHeight Tests

    [Fact]
    public void GetFontHeightReturnsPositiveValue()
    {
        var height = ExcelFontXml.GetFontHeight("Calibri", 11);

        Assert.True(height > 0);
    }

    [Fact]
    public void GetFontHeightFallsBackToCalibriForUnknownFont()
    {
        var calibriHeight = ExcelFontXml.GetFontHeight("Calibri", 11);
        var unknownHeight = ExcelFontXml.GetFontHeight("NonExistentFontName12345", 11);

        Assert.Equal(calibriHeight, unknownHeight);
    }

    [Fact]
    public void GetFontHeightStripsLeadingAtSign()
    {
        var normalHeight = ExcelFontXml.GetFontHeight("Calibri", 11);
        var atPrefixedHeight = ExcelFontXml.GetFontHeight("@Calibri", 11);

        Assert.Equal(normalHeight, atPrefixedHeight);
    }

    [Fact]
    public void GetFontHeightIncreasesWithSize()
    {
        var smallHeight = ExcelFontXml.GetFontHeight("Calibri", 8);
        var largeHeight = ExcelFontXml.GetFontHeight("Calibri", 20);

        Assert.True(largeHeight > smallHeight);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void FontPropertiesSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Style.Font.Name = "Arial";
            ws.Cells["A1"].Style.Font.Size = 16;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Italic = true;
            ws.Cells["A1"].Style.Font.Strike = true;
            ws.Cells["A1"].Style.Font.UnderLine = true;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new System.IO.MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var font = pck2.Workbook.Worksheets[0].Cells["A1"].Style.Font;

        Assert.Equal("Arial", font.Name);
        Assert.Equal(16, font.Size);
        Assert.True(font.Bold);
        Assert.True(font.Italic);
        Assert.True(font.Strike);
        Assert.True(font.UnderLine);
    }

    [Theory]
    [InlineData(ExcelUnderLineType.Single)]
    [InlineData(ExcelUnderLineType.Double)]
    [InlineData(ExcelUnderLineType.SingleAccounting)]
    [InlineData(ExcelUnderLineType.DoubleAccounting)]
    public void FontUnderLineTypeSurvivesRoundTrip(ExcelUnderLineType underLineType)
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Style.Font.UnderLineType = underLineType;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new System.IO.MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var font = pck2.Workbook.Worksheets[0].Cells["A1"].Style.Font;

        Assert.Equal(underLineType, font.UnderLineType);
    }

    [Fact]
    public void SetFromFontSurvivesRoundTrip()
    {
        var fontName = GetAvailableSystemFont();
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            var font = SystemFonts.CreateFont(fontName, 20f, FontStyle.BoldItalic);
            ws.Cells["A1"].Style.Font.SetFromFont(font);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new System.IO.MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var cellFont = pck2.Workbook.Worksheets[0].Cells["A1"].Style.Font;

        Assert.Equal(fontName, cellFont.Name);
        Assert.Equal(20, cellFont.Size);
        Assert.True(cellFont.Bold);
        Assert.True(cellFont.Italic);
    }

    #endregion
}
