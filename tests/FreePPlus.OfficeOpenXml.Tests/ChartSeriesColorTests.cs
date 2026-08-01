using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Globalization;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Colour handling on line- and scatter-chart series.
/// </summary>
/// <remarks>
///     These cover three defects that had no coverage at all: the getters returned fully transparent colours
///     because six stored RGB digits were fed to the packed-ARGB overload; the OOXML alpha value was written
///     inverted, on the wrong scale, and could go negative; and line-chart series had no alpha support.
///     The read and write sides were inverse of each other, so a property round trip passed while the file
///     written was still wrong - hence the assertions on the saved XML.
/// </remarks>
public class ChartSeriesColorTests
{
    private const string ChartPart = "xl/charts/chart1.xml";

    private const string LineColorValuePath =
        "//c:ser/c:spPr/a:ln/a:solidFill/a:srgbClr/@val";

    private const string LineColorAlphaPath =
        "//c:ser/c:spPr/a:ln/a:solidFill/a:srgbClr/a:alpha/@val";

    private const string MarkerColorValuePath =
        "//c:ser/c:marker/c:spPr/a:solidFill/a:srgbClr/@val";

    private const string MarkerLineColorValuePath =
        "//c:ser/c:marker/c:spPr/a:ln/a:solidFill/a:srgbClr/@val";

    private static void AddTestData(ExcelWorksheet ws)
    {
        ws.Cells["A1"].Value = 1;
        ws.Cells["A2"].Value = 2;
        ws.Cells["A3"].Value = 3;
        ws.Cells["B1"].Value = 100;
        ws.Cells["B2"].Value = 102;
        ws.Cells["B3"].Value = 101;
    }

    private static ExcelScatterChartSeriesItem AddScatterSeries(ExcelPackage pck)
    {
        var ws = pck.Workbook.Worksheets.Add("Scatter");
        AddTestData(ws);
        var chart = ws.Drawings.AddChart("scatter", eChartType.XYScatterLines) as ExcelScatterChart;
        Assert.NotNull(chart);
        chart.Series.Add($"'{ws.Name}'!B1:B3", $"'{ws.Name}'!A1:A3");
        return (ExcelScatterChartSeriesItem)chart.Series[0];
    }

    private static ExcelLineChartSeriesItem AddLineSeries(ExcelPackage pck)
    {
        var ws = pck.Workbook.Worksheets.Add("Line");
        AddTestData(ws);
        var chart = ws.Drawings.AddChart("line", eChartType.LineMarkers) as ExcelLineChart;
        Assert.NotNull(chart);
        chart.Series.Add($"'{ws.Name}'!B1:B3", $"'{ws.Name}'!A1:A3");
        return (ExcelLineChartSeriesItem)chart.Series[0];
    }

    /// <summary>
    ///     Reads the single node the xPath selects from the saved chart part, or null when it is absent.
    /// </summary>
    private static string ReadChartValue(ExcelPackage pck, string xPath) => ReadChartValues(pck, xPath)[0];

    /// <summary>
    ///     Reads several nodes from the saved chart part. Saving closes the package, so every value a test
    ///     needs from the XML has to be pulled in one call.
    /// </summary>
    private static string[] ReadChartValues(ExcelPackage pck, params string[] xPaths)
    {
        var doc = PackageXmlInspector.GetPart(pck, ChartPart);
        var nsm = PackageXmlInspector.NamespaceManager(doc);

        var results = new string[xPaths.Length];
        for (var i = 0; i < xPaths.Length; i++) results[i] = doc.SelectSingleNode(xPaths[i], nsm)?.Value;
        return results;
    }

    #region Getters return the colour that was set (previously always fully transparent)

    [Fact]
    public void ScatterChartLineColorRoundTripsOpaqueColor()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);
        var expected = Color.FromArgb(255, 51, 102, 153);

        //Act
        series.LineColor = expected;
        var actual = series.LineColor;

        //Assert
        Assert.Equal(expected.ToRgba32(), actual.ToRgba32());
        Assert.Equal(255, actual.ToRgba32().A);
    }

    [Fact]
    public void ScatterChartMarkerColorRoundTripsOpaqueColor()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);
        var expected = Color.FromArgb(255, 51, 102, 153);

        //Act
        series.MarkerColor = expected;
        var actual = series.MarkerColor;

        //Assert
        Assert.Equal(expected.ToRgba32(), actual.ToRgba32());
        Assert.Equal(255, actual.ToRgba32().A);
    }

    [Fact]
    public void ScatterChartMarkerLineColorRoundTripsOpaqueColor()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);
        var expected = Color.FromArgb(255, 51, 102, 153);

        //Act
        series.MarkerLineColor = expected;
        var actual = series.MarkerLineColor;

        //Assert
        Assert.Equal(expected.ToRgba32(), actual.ToRgba32());
        Assert.Equal(255, actual.ToRgba32().A);
    }

    [Fact]
    public void LineChartLineColorRoundTripsOpaqueColor()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);
        var expected = Color.FromArgb(255, 51, 102, 153);

        //Act
        series.LineColor = expected;
        var actual = series.LineColor;

        //Assert
        Assert.Equal(expected.ToRgba32(), actual.ToRgba32());
        Assert.Equal(255, actual.ToRgba32().A);
    }

    [Fact]
    public void LineChartMarkerLineColorRoundTripsOpaqueColor()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);
        var expected = Color.FromArgb(255, 51, 102, 153);

        //Act
        series.MarkerLineColor = expected;
        var actual = series.MarkerLineColor;

        //Assert
        Assert.Equal(expected.ToRgba32(), actual.ToRgba32());
        Assert.Equal(255, actual.ToRgba32().A);
    }

    #endregion

    #region Alpha is written as OOXML opacity (previously inverted, mis-scaled and able to go negative)

    [Theory]
    [InlineData(0, "0")]
    [InlineData(50, "19608")]
    [InlineData(100, "39216")]
    [InlineData(128, "50196")]
    [InlineData(200, "78431")]
    [InlineData(254, "99608")]
    public void ScatterChartLineColorWritesOpacityOnTheOoxmlScale(int alpha, string expectedVal)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        Assert.Equal(expectedVal, ReadChartValue(pck, LineColorAlphaPath));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(128)]
    [InlineData(200)]
    [InlineData(254)]
    public void ScatterChartAlphaIsAlwaysInTheLegalOoxmlRange(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(alpha, 51, 102, 153);
        var written = ReadChartValue(pck, LineColorAlphaPath);

        //Assert
        Assert.NotNull(written);
        var value = int.Parse(written, CultureInfo.InvariantCulture);
        Assert.InRange(value, 0, 100000);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(50)]
    [InlineData(128)]
    [InlineData(200)]
    [InlineData(254)]
    [InlineData(255)]
    public void ScatterChartLineColorRoundTripsAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(alpha, 51, 102, 153);
        var actual = series.LineColor.ToRgba32();

        //Assert
        Assert.Equal(alpha, actual.A);
        Assert.Equal(51, actual.R);
        Assert.Equal(102, actual.G);
        Assert.Equal(153, actual.B);
    }

    [Fact]
    public void ScatterChartLineColorKeepsTheRgbDigitsWhenAlphaIsSet()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(128, 51, 102, 153);

        //Assert
        Assert.Equal("336699", ReadChartValue(pck, LineColorValuePath));
    }

    [Fact]
    public void ScatterChartOpaqueColorWritesNoAlphaNode()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(255, 51, 102, 153);

        //Assert
        Assert.Null(ReadChartValue(pck, LineColorAlphaPath));
    }

    [Fact]
    public void ScatterChartOpaqueColorRemovesAStaleAlphaNode()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);
        series.LineColor = Color.FromArgb(128, 51, 102, 153);

        //Act - changing back to opaque must not leave the old transparency behind
        series.LineColor = Color.FromArgb(255, 51, 102, 153);

        //Assert
        Assert.Equal(255, series.LineColor.ToRgba32().A);
        Assert.Null(ReadChartValue(pck, LineColorAlphaPath));
    }

    [Fact]
    public void ScatterChartMarkerColorRoundTripsAlpha()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.MarkerColor = Color.FromArgb(128, 51, 102, 153);

        //Assert
        Assert.Equal(128, series.MarkerColor.ToRgba32().A);
        Assert.Equal("336699", ReadChartValue(pck, MarkerColorValuePath));
    }

    [Fact]
    public void ScatterChartMarkerLineColorRoundTripsAlpha()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.MarkerLineColor = Color.FromArgb(128, 51, 102, 153);

        //Assert
        Assert.Equal(128, series.MarkerLineColor.ToRgba32().A);
        Assert.Equal("336699", ReadChartValue(pck, MarkerLineColorValuePath));
    }

    #endregion

    #region Line-chart series support alpha the same way scatter-chart series do

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(128)]
    [InlineData(254)]
    [InlineData(255)]
    public void LineChartLineColorRoundTripsAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(alpha, 51, 102, 153);
        var actual = series.LineColor.ToRgba32();

        //Assert
        Assert.Equal(alpha, actual.A);
        Assert.Equal(51, actual.R);
        Assert.Equal(102, actual.G);
        Assert.Equal(153, actual.B);
    }

    [Fact]
    public void LineChartLineColorWritesOpacityOnTheOoxmlScale()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);

        //Act
        series.LineColor = Color.FromArgb(128, 51, 102, 153);
        var written = ReadChartValues(pck, LineColorValuePath, LineColorAlphaPath);

        //Assert
        Assert.Equal("336699", written[0]);
        Assert.Equal("50196", written[1]);
    }

    [Fact]
    public void LineChartMarkerLineColorRoundTripsAlpha()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);

        //Act
        series.MarkerLineColor = Color.FromArgb(128, 51, 102, 153);

        //Assert
        Assert.Equal(128, series.MarkerLineColor.ToRgba32().A);
        Assert.Equal("336699", ReadChartValue(pck, MarkerLineColorValuePath));
    }

    #endregion

    #region Low alpha no longer truncates the hex string

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(128)]
    [InlineData(255)]
    public void LineChartLineColorWritesSixRgbDigitsForEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddLineSeries(pck);

        //Act - alpha below 16 used to shorten the hex string and silently shift the stored colour
        series.LineColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        Assert.Equal("336699", ReadChartValue(pck, LineColorValuePath));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(128)]
    [InlineData(255)]
    public void ScatterChartMarkerColorWritesSixRgbDigitsForEveryAlpha(int alpha)
    {
        //Arrange
        using var pck = new ExcelPackage();
        var series = AddScatterSeries(pck);

        //Act
        series.MarkerColor = Color.FromArgb(alpha, 51, 102, 153);

        //Assert
        Assert.Equal("336699", ReadChartValue(pck, MarkerColorValuePath));
    }

    #endregion
}
