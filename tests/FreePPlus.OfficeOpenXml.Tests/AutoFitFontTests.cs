using System;
using System.IO;
using System.Linq;
using CodeBrix.Imaging.Fonts;
using Xunit;

namespace OfficeOpenXml.Tests;

public class AutoFitFontTests
{
    private static FontFamily AvailableFamily()
    {
        var families = SystemFonts.Families.ToArray();
        Assert.SkipWhen(families.Length == 0, "Font measurement tests require an installed font.");
        return families[0];
    }

    [Fact]
    public void MissingFontFallsBackWithoutChangingSavedStyle()
    {
        _ = AvailableFamily();
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Test");
        var cell = sheet.Cells["A1"];
        cell.Value = "A long line of text that needs a wider column";
        cell.Style.Font.Name = "FreePPlus-Missing-Font-7D203C";
        cell.Style.Font.Bold = true;
        cell.AutoFitColumns(1, 25);
        Assert.InRange(sheet.Column(1).Width, 2, 25);
        Assert.True(package.DoAdjustDrawings);

        using var stream = new MemoryStream(package.GetAsByteArray());
        using var reopened = new ExcelPackage(stream);
        Assert.Equal("FreePPlus-Missing-Font-7D203C", reopened.Workbook.Worksheets[0].Cells["A1"].Style.Font.Name);
        Assert.True(reopened.Workbook.Worksheets[0].Cells["A1"].Style.Font.Bold);
    }

    [Fact]
    public void CustomResolverControlsMeasurementWithoutChangingStyle()
    {
        var family = AvailableFamily();
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Test");
        var cell = sheet.Cells["A1"];
        cell.Value = "Measure this text";
        cell.Style.Font.Name = "Workbook Font";
        cell.Style.Font.Size = 13;
        cell.Style.Font.Bold = true;
        cell.Style.Font.Italic = true;
        int calls = 0;
        package.AutoFitFontResolver = (name, size, style) =>
        {
            Assert.Equal("Workbook Font", name);
            Assert.Equal(13f, size);
            Assert.Equal(FontStyle.BoldItalic, style);
            calls++;
            return new Font(family, size, style);
        };
        cell.AutoFitColumns(0, 100);
        double firstWidth = sheet.Column(1).Width;
        Assert.Equal(1, calls);

        package.AutoFitFontResolver = (_, size, style) => new Font(family, size * 2, style);
        cell.AutoFitColumns(0, 100);
        Assert.True(sheet.Column(1).Width > firstWidth);
        Assert.Equal("Workbook Font", cell.Style.Font.Name);
        Assert.Equal(13f, cell.Style.Font.Size);
    }

    [Fact]
    public void NullResolverResultUsesSystemResolutionAndSettingsArePerPackage()
    {
        var family = AvailableFamily();
        using var package = new ExcelPackage();
        using var other = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Test");
        sheet.Cells["A1"].Value = "Use the installed font";
        sheet.Cells["A1"].Style.Font.Name = family.Name;
        sheet.Cells["A1"].AutoFitColumns(0);
        double expected = sheet.Column(1).Width;
        package.AutoFitFontResolver = (_, _, _) => null;
        sheet.Cells["A1"].AutoFitColumns(0);
        Assert.Equal(expected, sheet.Column(1).Width);
        Assert.Null(other.AutoFitFontResolver);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ResolverFailureRestoresDrawingAdjustmentSetting(bool adjustDrawings)
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Test");
        sheet.Cells["A1"].Value = "Text";
        package.DoAdjustDrawings = adjustDrawings;
        var failure = new InvalidOperationException("Custom resolver failure");
        package.AutoFitFontResolver = (_, _, _) => throw failure;
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => sheet.Cells["A1"].AutoFitColumns()));
        Assert.Equal(adjustDrawings, package.DoAdjustDrawings);
    }
}
