/*******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * EPPlus provides server-side generation of Excel 2007/2010 spreadsheets.
 * See https://github.com/JanKallman/EPPlus for details.
 *
 * Copyright (C) 2011  Jan Källman
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "as is" with no warranty either express or implied.
 * The author accepts no liability for any damage or loss of business that this product may cause.
 *
 * Code change notes:
 *
 * Author							Change						Date
 *******************************************************************************
 * Jan Källman		Added		2009-12-30
 * Jan Källman		License changed GPL-->LGPL 2011-12-16
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CodeBrix.Imaging;

namespace OfficeOpenXml.Drawing.Chart;

/// <summary>
///     A chart series item
/// </summary>
public class ExcelChartSeriesItem : XmlHelper
{
    private const string HeaderPath = "c:tx/c:v";

    private const string HeaderAddressPath = "c:tx/c:strRef/c:f";
    private readonly string _seriesPath = "{0}/c:numRef/c:f";

    private readonly string _seriesTopPath;
    private readonly string _xSeriesPath = "{0}/{1}/c:f";

    private readonly string _xSeriesTopPath;

    private ExcelDrawingBorder _border;

    private ExcelDrawingFill _fill;

    private ExcelChartTrendlineCollection _trendLines;

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <param name="chartSeries">Parent collection</param>
    /// <param name="ns">Namespacemanager</param>
    /// <param name="node">Topnode</param>
    /// <param name="isPivot">Is pivotchart</param>
    internal ExcelChartSeriesItem(ExcelChartSeries chartSeries, XmlNamespaceManager ns, XmlNode node, bool isPivot)
        : base(ns, node)
    {
        _chartSeries = chartSeries;
        _node = node;
        _ns = ns;
        SchemaNodeOrder = new[]
        {
            "idx", "order", "spPr", "tx", "marker", "trendline", "explosion", "invertIfNegative", "dLbls", "cat", "val",
            "xVal", "yVal", "bubbleSize", "bubble3D", "smooth"
        };

        if (chartSeries.Chart.ChartType == eChartType.XYScatter ||
            chartSeries.Chart.ChartType == eChartType.XYScatterLines ||
            chartSeries.Chart.ChartType == eChartType.XYScatterLinesNoMarkers ||
            chartSeries.Chart.ChartType == eChartType.XYScatterSmooth ||
            chartSeries.Chart.ChartType == eChartType.XYScatterSmoothNoMarkers ||
            chartSeries.Chart.ChartType == eChartType.Bubble ||
            chartSeries.Chart.ChartType == eChartType.Bubble3DEffect)
        {
            _seriesTopPath = "c:yVal";
            _xSeriesTopPath = "c:xVal";
        }
        else
        {
            _seriesTopPath = "c:val";
            _xSeriesTopPath = "c:cat";
        }

        _seriesPath = string.Format(_seriesPath, _seriesTopPath);

        var np = string.Format(_xSeriesPath, _xSeriesTopPath, isPivot ? "c:multiLvlStrRef" : "c:numRef");
        var sp = string.Format(_xSeriesPath, _xSeriesTopPath, isPivot ? "c:multiLvlStrRef" : "c:strRef");
        if (ExistNode(sp))
            _xSeriesPath = sp;
        else
            _xSeriesPath = np;
    }

    /// <summary>
    ///     Header for the serie.
    /// </summary>
    public string Header
    {
        get => GetXmlNodeString(HeaderPath);
        set
        {
            Cleartx();
            SetXmlNodeString(HeaderPath, value);
        }
    }

    /// <summary>
    ///     Header address for the series item.
    /// </summary>
    public ExcelAddressBase HeaderAddress
    {
        get
        {
            var address = GetXmlNodeString(HeaderAddressPath);
            if (address == "")
                return null;
            return new ExcelAddressBase(address);
        }
        set
        {
            if ((value._fromCol != value._toCol && value._fromRow != value._toRow) ||
                value.Addresses != null) //Single cell removed, allow row & column --> issue 15102. 
                throw new ArgumentException("Address must be a row, column or single cell");

            Cleartx();
            SetXmlNodeString(HeaderAddressPath, ExcelCellBase.GetFullAddress(value.WorkSheet, value.Address));
            SetXmlNodeString("c:tx/c:strRef/c:strCache/c:ptCount/@val", "0");
        }
    }

    /// <summary>
    ///     Set this to a valid address or the drawing will be invalid.
    /// </summary>
    public virtual string Series
    {
        get => GetXmlNodeString(_seriesPath);
        set
        {
            CreateNode(_seriesPath, true);
            SetXmlNodeString(_seriesPath, ExcelCellBase.GetFullAddress(_chartSeries.Chart.WorkSheet.Name, value));

            if (_chartSeries.Chart.PivotTableSource != null)
            {
                var cache = TopNode.SelectSingleNode(string.Format("{0}/c:numRef/c:numCache", _seriesTopPath), _ns);
                if (cache?.ParentNode != null) cache.ParentNode.RemoveChild(cache);
                SetXmlNodeString(string.Format("{0}/c:numRef/c:numCache", _seriesTopPath), "General");
            }

            var lit = TopNode.SelectSingleNode(string.Format("{0}/c:numLit", _seriesTopPath), _ns);
            if (lit?.ParentNode != null) lit.ParentNode.RemoveChild(lit);
        }
    }

    /// <summary>
    ///     Set an address for the horizontal labels
    /// </summary>
    public virtual string XSeries
    {
        get => GetXmlNodeString(_xSeriesPath);
        set
        {
            CreateNode(_xSeriesPath, true);
            SetXmlNodeString(_xSeriesPath, ExcelCellBase.GetFullAddress(_chartSeries.Chart.WorkSheet.Name, value));

            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            if (_xSeriesPath.IndexOf("c:numRef") > 0)
            {
                var cache = TopNode.SelectSingleNode(string.Format("{0}/c:numRef/c:numCache", _xSeriesTopPath), _ns);
                if (cache?.ParentNode != null) cache.ParentNode.RemoveChild(cache);

                var lit = TopNode.SelectSingleNode(string.Format("{0}/c:numLit", _xSeriesTopPath), _ns);
                if (lit?.ParentNode != null) lit.ParentNode.RemoveChild(lit);
            }
            else
            {
                var cache = TopNode.SelectSingleNode(string.Format("{0}/c:strRef/c:strCache", _xSeriesTopPath), _ns);
                if (cache?.ParentNode != null) cache.ParentNode.RemoveChild(cache);

                var lit = TopNode.SelectSingleNode(string.Format("{0}/c:strLit", _xSeriesTopPath), _ns);
                if (lit?.ParentNode != null) lit.ParentNode.RemoveChild(lit);
            }
        }
    }

    /// <summary>
    ///     Access to the trendline collection
    /// </summary>
    public ExcelChartTrendlineCollection TrendLines => _trendLines ??= new ExcelChartTrendlineCollection(this);

    public ExcelDrawingFill Fill => _fill ??= new ExcelDrawingFill(NameSpaceManager, TopNode, "c:spPr");
    public ExcelDrawingBorder Border => _border ??= new ExcelDrawingBorder(NameSpaceManager, TopNode, "c:spPr/a:ln");

    internal void SetId(string id)
    {
        SetXmlNodeString("c:idx/@val", id);
        SetXmlNodeString("c:order/@val", id);
    }

    private void Cleartx()
    {
        var n = TopNode.SelectSingleNode("c:tx", NameSpaceManager);
        if (n != null) n.InnerXml = "";
    }

    /// <summary>
    ///     Reads the six RGB hex digits stored at a color path and returns them as an opaque color.
    /// </summary>
    /// <param name="hex">The stored value, which carries no alpha byte.</param>
    /// <returns>An opaque color.</returns>
    private protected static Color ColorFromRgbHex(string hex)
    {
        var argb = Convert.ToInt32(hex, 16);
        return Color.FromArgb((argb >> 16) & 0xFF, (argb >> 8) & 0xFF, argb & 0xFF);
    }

    /// <summary>
    ///     Writes the alpha value of a color as an OOXML opacity value.
    /// </summary>
    /// <param name="c">Color</param>
    /// <param name="xPath">where to write</param>
    /// <remarks>
    ///     alpha-values may only be written to color-nodes
    ///     eg: a:prstClr (preset), a:hslClr (hsl), a:schemeClr (schema), a:sysClr (system), a:scrgbClr (rgb percent) or
    ///     a:srgbClr (rgb hex)
    ///     .../a:prstClr/a:alpha/@val
    ///     OOXML a:alpha expresses OPACITY in 1000ths of a percent, so the legal range is 0 (fully transparent)
    ///     to 100000 (fully opaque). A fully opaque color needs no node at all, and any stale node left over
    ///     from a previously transparent color is removed so the old transparency does not survive in the file.
    /// </remarks>
    private protected void SetAlphaChannel(Color c, string xPath)
    {
        var rgba = c.ToRgba32();

        var s = GetXPath4Alpha(xPath);
        if (s.Length == 0) return;

        if (rgba.A == 255)
        {
            //opaque color => drop any alpha node a previous, partly transparent color left behind
            DeleteNode(s[..s.LastIndexOf('/')]);
            return;
        }

        var alpha = ((int)Math.Round(rgba.A * 100000.0 / 255.0)).ToString(CultureInfo.InvariantCulture);
        SetXmlNodeString(s, alpha, true);
    }

    /// <summary>
    ///     Reads the alpha channel from a color node.
    /// </summary>
    /// <param name="xPath">xPath to the color node</param>
    /// <returns>alpha as a 0-255 byte, or 255 (fully opaque) if there is no such node</returns>
    private protected int GetAlphaChannel(string xPath)
    {
        var r = 255;
        var s = GetXPath4Alpha(xPath);
        if (s.Length > 0)
            if (int.TryParse(GetXmlNodeString(s), NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
                r = (int)Math.Round(Math.Clamp(i, 0, 100000) * 255.0 / 100000.0);
        return r;
    }

    /// <summary>
    ///     Builds the xPath to the alpha attribute for a color.
    ///     eg: a:prstClr/a:alpha/@val
    /// </summary>
    /// <param name="xPath">xPath to color node</param>
    /// <returns>The xPath to the alpha attribute.</returns>
    /// <exception cref="InvalidOperationException">The path does not point at a color node.</exception>
    private protected static string GetXPath4Alpha(string xPath)
    {
        if (xPath.EndsWith("@val", StringComparison.Ordinal))
            xPath = xPath[..xPath.IndexOf("@val", StringComparison.Ordinal)];
        if (xPath.EndsWith("/", StringComparison.Ordinal))
            //cut tailing slash
            xPath = xPath[..^1];
        //parent node must be a color node/definition
        var colorDefs = new List<string>
            { "a:prstClr", "a:hslClr", "a:schemeClr", "a:sysClr", "a:scrgbClr", "a:srgbClr" };
        if (colorDefs.Find(cd => xPath.EndsWith(cd, StringComparison.Ordinal)) == null)
            throw new InvalidOperationException("alpha-values can only set to Colors");

        return xPath + "/a:alpha/@val";
    }

    // ReSharper disable InconsistentNaming
    internal ExcelChartSeries _chartSeries;
    protected XmlNode _node;

    protected XmlNamespaceManager _ns;
    // ReSharper restore InconsistentNaming
}