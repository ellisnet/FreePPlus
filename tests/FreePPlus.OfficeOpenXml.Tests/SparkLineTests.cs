using System;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Sparkline;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class SparkLineTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    #region Test Helpers

    private static ExcelWorksheet CreateSparklineWorksheet(ExcelPackage pck)
    {
        var ws = pck.Workbook.Worksheets.Add("Sparklines");
        ws.Cells["B1"].Value = 15;
        ws.Cells["B2"].Value = 30;
        ws.Cells["B3"].Value = 35;
        ws.Cells["B4"].Value = 28;
        ws.Cells["C1"].Value = 7;
        ws.Cells["C2"].Value = 33;
        ws.Cells["C3"].Value = 12;
        ws.Cells["C4"].Value = -1;
        return ws;
    }

    private static void AddAllSparklineGroups(ExcelWorksheet ws)
    {
        // Column<->Row - Line sparkline
        var sg1 = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg1.DisplayEmptyCellsAs = eDispBlanksAs.Gap;

        // Column<->Column - Column sparkline
        ws.SparklineGroups.Add(eSparklineType.Column, ws.Cells["D1:D2"], ws.Cells["B1:C4"]);

        // Row<->Column - Stacked sparkline
        var sg3 = ws.SparklineGroups.Add(eSparklineType.Stacked, ws.Cells["A10:B10"], ws.Cells["B1:C4"]);
        sg3.RightToLeft = true;

        // Row<->Row - Line sparkline with date axis
        var sg4 = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["D10:G10"], ws.Cells["B1:C4"]);
        ws.Cells["A20"].Value = new DateTime(2016, 12, 30);
        ws.Cells["A21"].Value = new DateTime(2017, 1, 31);
        ws.Cells["A22"].Value = new DateTime(2017, 2, 28);
        ws.Cells["A23"].Value = new DateTime(2017, 3, 31);
        sg4.DateAxisRange = ws.Cells["A20:A23"];
        sg4.ManualMax = 5;
        sg4.ManualMin = 3;
    }

    #endregion

    #region Write Sparkline Tests

    [Fact]
    public void CanAddLineSparklineGroup()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.Equal(eSparklineType.Line, sg.Type);
        Assert.Equal("A1:A4", sg.LocationRange.Address);
        Assert.Equal("B1:C4", sg.DataRange.Address);
    }

    [Fact]
    public void CanAddColumnSparklineGroup()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Column, ws.Cells["D1:D2"], ws.Cells["B1:C4"]);

        Assert.Equal(eSparklineType.Column, sg.Type);
        Assert.Equal("D1:D2", sg.LocationRange.Address);
    }

    [Fact]
    public void CanAddStackedSparklineGroup()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Stacked, ws.Cells["A10:B10"], ws.Cells["B1:C4"]);

        Assert.Equal(eSparklineType.Stacked, sg.Type);
        Assert.Equal("A10:B10", sg.LocationRange.Address);
    }

    [Fact]
    public void CanAddMultipleSparklineGroups()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(4, ws2.SparklineGroups.Count);
    }

    #endregion

    #region Sparkline Property Tests

    [Fact]
    public void DisplayEmptyCellsAsCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.DisplayEmptyCellsAs = eDispBlanksAs.Gap;

        Assert.Equal(eDispBlanksAs.Gap, sg.DisplayEmptyCellsAs);
    }

    [Theory]
    [InlineData(eDispBlanksAs.Gap)]
    [InlineData(eDispBlanksAs.Span)]
    [InlineData(eDispBlanksAs.Zero)]
    public void AllDisplayEmptyCellsAsValuesCanBeSet(eDispBlanksAs value)
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.DisplayEmptyCellsAs = value;

        Assert.Equal(value, sg.DisplayEmptyCellsAs);
    }

    [Fact]
    public void RightToLeftCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Stacked, ws.Cells["A10:B10"], ws.Cells["B1:C4"]);
        sg.RightToLeft = true;

        Assert.True(sg.RightToLeft);
    }

    [Fact]
    public void DateAxisRangeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);
        ws.Cells["A20"].Value = new DateTime(2016, 12, 30);
        ws.Cells["A21"].Value = new DateTime(2017, 1, 31);
        ws.Cells["A22"].Value = new DateTime(2017, 2, 28);
        ws.Cells["A23"].Value = new DateTime(2017, 3, 31);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["D10:G10"], ws.Cells["B1:C4"]);
        sg.DateAxisRange = ws.Cells["A20:A23"];

        Assert.NotNull(sg.DateAxisRange);
        Assert.Equal("'Sparklines'!A20:A23", sg.DateAxisRange.FullAddress);
    }

    [Fact]
    public void DateAxisRangeCanBeCleared()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);
        ws.Cells["A20"].Value = new DateTime(2016, 12, 30);
        ws.Cells["A21"].Value = new DateTime(2017, 1, 31);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["D10:G10"], ws.Cells["B1:C4"]);
        sg.DateAxisRange = ws.Cells["A20:A21"];
        Assert.NotNull(sg.DateAxisRange);

        sg.DateAxisRange = null;

        Assert.Null(sg.DateAxisRange);
    }

    [Fact]
    public void ManualMinMaxCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.ManualMax = 5;
        sg.ManualMin = 3;

        Assert.Equal(5, sg.ManualMax);
        Assert.Equal(3, sg.ManualMin);
    }

    [Fact]
    public void LineWidthCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.LineWidth = 2.5;

        Assert.Equal(2.5, sg.LineWidth);
    }

    [Fact]
    public void DefaultLineWidthIs075()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.Equal(0.75, sg.LineWidth);
    }

    #endregion

    #region Boolean Property Tests

    [Fact]
    public void MarkersCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.Markers = true;

        Assert.True(sg.Markers);
    }

    [Fact]
    public void HighLowCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.High = true;
        sg.Low = true;

        Assert.True(sg.High);
        Assert.True(sg.Low);
    }

    [Fact]
    public void FirstLastCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.First = true;
        sg.Last = true;

        Assert.True(sg.First);
        Assert.True(sg.Last);
    }

    [Fact]
    public void NegativeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.Negative = true;

        Assert.True(sg.Negative);
    }

    [Fact]
    public void DisplayXAxisCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.DisplayXAxis = true;

        Assert.True(sg.DisplayXAxis);
    }

    [Fact]
    public void DisplayHiddenCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.DisplayHidden = true;

        Assert.True(sg.DisplayHidden);
    }

    [Fact]
    public void BooleanPropertiesDefaultToFalse()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.False(sg.Markers);
        Assert.False(sg.High);
        Assert.False(sg.Low);
        Assert.False(sg.First);
        Assert.False(sg.Last);
        Assert.False(sg.Negative);
        Assert.False(sg.DisplayXAxis);
        Assert.False(sg.DisplayHidden);
        Assert.False(sg.RightToLeft);
    }

    #endregion

    #region Color Tests

    [Fact]
    public void DefaultColorsAreSet()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.Equal("FF376092", sg.ColorSeries.Rgb);
        Assert.Equal("FFD00000", sg.ColorNegative.Rgb);
        Assert.Equal("FF000000", sg.ColorAxis.Rgb);
        Assert.Equal("FFD00000", sg.ColorMarkers.Rgb);
        Assert.Equal("FFD00000", sg.ColorFirst.Rgb);
        Assert.Equal("FFD00000", sg.ColorLast.Rgb);
        Assert.Equal("FFD00000", sg.ColorHigh.Rgb);
        Assert.Equal("FFD00000", sg.ColorLow.Rgb);
    }

    [Fact]
    public void ColorMarkersHasDefaultRgb()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.Equal("FFD00000", sg.ColorMarkers.Rgb);
    }

    #endregion

    #region Axis Type Tests

    [Fact]
    public void DefaultAxisTypeIsIndividual()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);

        Assert.Equal(eSparklineAxisMinMax.Individual, sg.MinAxisType);
        Assert.Equal(eSparklineAxisMinMax.Individual, sg.MaxAxisType);
    }

    [Fact]
    public void SettingManualMinSetsAxisTypeToCustom()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.ManualMin = 3;

        Assert.Equal(eSparklineAxisMinMax.Custom, sg.MinAxisType);
    }

    [Fact]
    public void SettingManualMaxSetsAxisTypeToCustom()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.ManualMax = 5;

        Assert.Equal(eSparklineAxisMinMax.Custom, sg.MaxAxisType);
    }

    [Fact]
    public void AxisTypeCanBeSetToGroup()
    {
        using var pck = new ExcelPackage();
        var ws = CreateSparklineWorksheet(pck);

        var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
        sg.MinAxisType = eSparklineAxisMinMax.Group;
        sg.MaxAxisType = eSparklineAxisMinMax.Group;

        Assert.Equal(eSparklineAxisMinMax.Group, sg.MinAxisType);
        Assert.Equal(eSparklineAxisMinMax.Group, sg.MaxAxisType);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void CanRemoveSparklineGroupByIndex()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        Assert.Equal(4, ws2.SparklineGroups.Count);

        ws2.SparklineGroups.RemoveAt(0);

        Assert.Equal(3, ws2.SparklineGroups.Count);
    }

    [Fact]
    public void CanRemoveSparklineGroupByReference()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        var sg = ws2.SparklineGroups[1];

        ws2.SparklineGroups.Remove(sg);

        Assert.Equal(3, ws2.SparklineGroups.Count);
    }

    #endregion

    #region Write and Read Round-Trip Tests

    [Fact]
    public void SparklineGroupsSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(4, ws2.SparklineGroups.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SparklineGroupsSurviveRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void SparklineGroup1PropertiesSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        var sg1 = ws2.SparklineGroups[0];
        Assert.Equal("A1:A4", sg1.LocationRange.Address);
        Assert.Equal("B1:C4", sg1.DataRange.Address);
        Assert.Null(sg1.DateAxisRange);
        Assert.Equal(eDispBlanksAs.Gap, sg1.DisplayEmptyCellsAs);
        Assert.Equal(eSparklineType.Line, sg1.Type);
        Assert.Equal("FFD00000", sg1.ColorMarkers.Rgb);
    }

    [Fact]
    public void SparklineGroup2PropertiesSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        var sg2 = ws2.SparklineGroups[1];
        Assert.Equal("D1:D2", sg2.LocationRange.Address);
        Assert.Equal("B1:C4", sg2.DataRange.Address);
        Assert.Equal(eSparklineType.Column, sg2.Type);
    }

    [Fact]
    public void SparklineGroup3PropertiesSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        var sg3 = ws2.SparklineGroups[2];
        Assert.Equal("A10:B10", sg3.LocationRange.Address);
        Assert.Equal("B1:C4", sg3.DataRange.Address);
        Assert.Equal(eSparklineType.Stacked, sg3.Type);
    }

    [Fact]
    public void SparklineGroup4WithDateAxisSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            AddAllSparklineGroups(ws);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        var sg4 = ws2.SparklineGroups[3];
        Assert.Equal("D10:G10", sg4.LocationRange.Address);
        Assert.Equal("B1:C4", sg4.DataRange.Address);
        Assert.NotNull(sg4.DateAxisRange);
        Assert.Equal("'Sparklines'!A20:A23", sg4.DateAxisRange.FullAddress);
    }

    [Fact]
    public void BooleanPropertiesSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            sg.Markers = true;
            sg.High = true;
            sg.Low = true;
            sg.First = true;
            sg.Last = true;
            sg.Negative = true;
            sg.DisplayXAxis = true;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        var sg2 = ws2.SparklineGroups[0];

        Assert.True(sg2.Markers);
        Assert.True(sg2.High);
        Assert.True(sg2.Low);
        Assert.True(sg2.First);
        Assert.True(sg2.Last);
        Assert.True(sg2.Negative);
        Assert.True(sg2.DisplayXAxis);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BooleanPropertiesSurviveRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void ManualMinMaxSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            sg.ManualMax = 5;
            sg.ManualMin = 3;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        var sg2 = ws2.SparklineGroups[0];

        Assert.Equal(5, sg2.ManualMax);
        Assert.Equal(3, sg2.ManualMin);
        Assert.Equal(eSparklineAxisMinMax.Custom, sg2.MaxAxisType);
        Assert.Equal(eSparklineAxisMinMax.Custom, sg2.MinAxisType);
    }

    [Fact]
    public void LineWidthSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            sg.LineWidth = 2.25;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        var sg2 = ws2.SparklineGroups[0];

        Assert.Equal(2.25, sg2.LineWidth);
    }

    [Fact]
    public void ColorsSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];
        var sg2 = ws2.SparklineGroups[0];

        Assert.Equal("FF376092", sg2.ColorSeries.Rgb);
        Assert.Equal("FFD00000", sg2.ColorNegative.Rgb);
        Assert.Equal("FF000000", sg2.ColorAxis.Rgb);
        Assert.Equal("FFD00000", sg2.ColorMarkers.Rgb);
        Assert.Equal("FFD00000", sg2.ColorFirst.Rgb);
        Assert.Equal("FFD00000", sg2.ColorLast.Rgb);
        Assert.Equal("FFD00000", sg2.ColorHigh.Rgb);
        Assert.Equal("FFD00000", sg2.ColorLow.Rgb);
    }

    [Theory]
    [InlineData(eSparklineType.Line)]
    [InlineData(eSparklineType.Column)]
    [InlineData(eSparklineType.Stacked)]
    public void SparklineTypeSurvivesRoundTrip(eSparklineType type)
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            ws.SparklineGroups.Add(type, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(type, ws2.SparklineGroups[0].Type);
    }

    [Theory]
    [InlineData(eDispBlanksAs.Gap)]
    [InlineData(eDispBlanksAs.Span)]
    [InlineData(eDispBlanksAs.Zero)]
    public void DisplayEmptyCellsAsSurvivesRoundTrip(eDispBlanksAs value)
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            sg.DisplayEmptyCellsAs = value;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(value, ws2.SparklineGroups[0].DisplayEmptyCellsAs);
    }

    [Fact]
    public void RightToLeftSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            var sg = ws.SparklineGroups.Add(eSparklineType.Stacked, ws.Cells["A10:B10"], ws.Cells["B1:C4"]);
            sg.RightToLeft = true;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.True(ws2.SparklineGroups[0].RightToLeft);
    }

    #endregion

    #region Data Value Tests

    [Fact]
    public void SparklineDataValuesArePreserved()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = CreateSparklineWorksheet(pck);
            ws.SparklineGroups.Add(eSparklineType.Line, ws.Cells["A1:A4"], ws.Cells["B1:C4"]);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(15d, ws2.Cells["B1"].Value);
        Assert.Equal(30d, ws2.Cells["B2"].Value);
        Assert.Equal(35d, ws2.Cells["B3"].Value);
        Assert.Equal(28d, ws2.Cells["B4"].Value);
        Assert.Equal(7d, ws2.Cells["C1"].Value);
        Assert.Equal(33d, ws2.Cells["C2"].Value);
        Assert.Equal(12d, ws2.Cells["C3"].Value);
        Assert.Equal(-1d, ws2.Cells["C4"].Value);
    }

    #endregion
}
