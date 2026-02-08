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
 * ******************************************************************************
 * Jan Källman		Initial Release		        2009-10-01
 * Jan Källman		License changed GPL-->LGPL 2011-12-16
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using CodeBrix.Imaging;

namespace OfficeOpenXml.Drawing.Chart;

/// <summary>
///     A serie for a scatter chart
/// </summary>
public sealed class ExcelScatterChartSeriesItem : ExcelChartSeriesItem
{
    private const string SmoothPath = "c:smooth/@val";

    private const string MarkerPath = "c:marker/c:symbol/@val";

    //new properties for excel scatter-plots: LineColor, MarkerSize, MarkerColor, LineWidth and MarkerLineColor
    //implemented according to https://epplus.codeplex.com/discussions/287917
    private const string LineColorPath = "c:spPr/a:ln/a:solidFill/a:srgbClr/@val";

    private const string MarkerSizePath = "c:marker/c:size/@val";

    private const string MarkerColorPath = "c:marker/c:spPr/a:solidFill/a:srgbClr/@val";

    private const string LineWidthPath = "c:spPr/a:ln/@w";

    //marker line color
    private const string MarkerLineColorPath = "c:marker/c:spPr/a:ln/a:solidFill/a:srgbClr/@val";

    private ExcelChartSeriesDataLabel _dataLabel;

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <param name="chartSeries">Parent collection</param>
    /// <param name="ns">Namespacemanager</param>
    /// <param name="node">Topnode</param>
    /// <param name="isPivot">Is pivotchart</param>
    internal ExcelScatterChartSeriesItem(ExcelChartSeries chartSeries, XmlNamespaceManager ns, XmlNode node,
        bool isPivot) :
        base(chartSeries, ns, node, isPivot)
    {
        if (chartSeries.Chart.ChartType == eChartType.XYScatterLines ||
            chartSeries.Chart.ChartType == eChartType.XYScatterSmooth)
            Marker = eMarkerStyle.Square;

        if (chartSeries.Chart.ChartType == eChartType.XYScatterSmooth ||
            chartSeries.Chart.ChartType == eChartType.XYScatterSmoothNoMarkers)
            Smooth = 1;
        else if (chartSeries.Chart.ChartType == eChartType.XYScatterLines ||
                 chartSeries.Chart.ChartType == eChartType.XYScatterLinesNoMarkers ||
                 chartSeries.Chart.ChartType == eChartType.XYScatter) Smooth = 0;
    }

    /// <summary>
    ///     Datalabel
    /// </summary>
    public ExcelChartSeriesDataLabel DataLabel => _dataLabel ??= new ExcelChartSeriesDataLabel(_ns, _node);

    /// <summary>
    ///     Smooth for scattercharts
    /// </summary>
    public int Smooth
    {
        get => GetXmlNodeInt(SmoothPath);
        internal set => SetXmlNodeString(SmoothPath, value.ToString());
    }

    /// <summary>
    ///     Marker symbol
    /// </summary>
    public eMarkerStyle Marker
    {
        get
        {
            var marker = GetXmlNodeString(MarkerPath);
            if (marker == "")
                return eMarkerStyle.None;
            return (eMarkerStyle)Enum.Parse(typeof(eMarkerStyle), marker, true);
        }
        set =>
            /* setting MarkerStyle seems to be working, so no need to throw an exception in this case

             f (_chartSeries.Chart.ChartType == eChartType.XYScatterLinesNoMarkers ||

                chartSeries.Chart.ChartType == eChartType.XYScatterSmoothNoMarkers)



                hrow (new InvalidOperationException("Can't set marker style for this charttype."));

            */
            SetXmlNodeString(MarkerPath, value.ToString().ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Line color.
    /// </summary>
    /// <value>
    ///     The color of the line.
    /// </value>
    public Color LineColor
    {
        get
        {
            var color = GetXmlNodeString(LineColorPath);
            if (color == "")
            {
                return Color.Black;
            }

            var c = Color.FromArgb(Convert.ToInt32(color, 16));
            var a = GetAlphaChannel(LineColorPath);
            if (a != 255) c = Color.FromArgb(a, c);
            return c;
        }
        set
        {
            SetXmlNodeString(LineColorPath, value.ToArgbInt32().ToString("X8")[2..], true);
            SetAlphaChannel(value, LineColorPath);
        }
    }

    /// <summary>
    ///     Gets or sets the size of the marker.
    /// </summary>
    /// <remarks>
    ///     value between 2 and 72.
    /// </remarks>
    /// <value>
    ///     The size of the marker.
    /// </value>
    public int MarkerSize
    {
        get
        {
            var size = GetXmlNodeString(MarkerSizePath);
            if (size == "")
                return 5;
            return int.Parse(GetXmlNodeString(MarkerSizePath));
        }
        set
        {
            var size = value;
            size = Math.Max(2, size);
            size = Math.Min(72, size);
            SetXmlNodeString(MarkerSizePath, size.ToString(), true);
        }
    }

    /// <summary>
    ///     Marker color.
    /// </summary>
    /// <value>
    ///     The color of the Marker.
    /// </value>
    public Color MarkerColor
    {
        get
        {
            var color = GetXmlNodeString(MarkerColorPath);
            if (color == "")
            {
                return Color.Black;
            }

            var c = Color.FromArgb(Convert.ToInt32(color, 16));
            var a = GetAlphaChannel(MarkerColorPath);
            if (a != 255) c = Color.FromArgb(a, c);
            return c;
        }
        set
        {
            SetXmlNodeString(MarkerColorPath, value.ToArgbInt32().ToString("X8")[2..],
                true); //.Substring(2) => cut alpha value
            SetAlphaChannel(value, MarkerColorPath);
        }
    }

    /// <summary>
    ///     Gets or sets the width of the line in pt.
    /// </summary>
    /// <value>
    ///     The width of the line.
    /// </value>
    public double LineWidth
    {
        get
        {
            var size = GetXmlNodeString(LineWidthPath);
            if (size == "")
                return 2.25;
            return double.Parse(GetXmlNodeString(LineWidthPath)) / 12700;
        }
        set => SetXmlNodeString(LineWidthPath, ((int)(12700 * value)).ToString(), true);
    }

    /// <summary>
    ///     Marker Line color.
    ///     (not to be confused with LineColor)
    /// </summary>
    /// <value>
    ///     The color of the Marker line.
    /// </value>
    public Color MarkerLineColor
    {
        get
        {
            var color = GetXmlNodeString(MarkerLineColorPath);
            if (color == "")
            {
                return Color.Black;
            }

            var c = Color.FromArgb(Convert.ToInt32(color, 16));
            var a = GetAlphaChannel(MarkerLineColorPath);
            if (a != 255) c = Color.FromArgb(a, c);
            return c;
        }
        set
        {
            SetXmlNodeString(MarkerLineColorPath, value.ToArgbInt32().ToString("X8")[2..], true);
            SetAlphaChannel(value, MarkerLineColorPath);
        }
    }

    /// <summary>
    ///     write alpha value (if Color.A != 255)
    /// </summary>
    /// <param name="c">Color</param>
    /// <param name="xPath">where to write</param>
    /// <remarks>
    ///     alpha-values may only written to color-nodes
    ///     eg: a:prstClr (preset), a:hslClr (hsl), a:schemeClr (schema), a:sysClr (system), a:scrgbClr (rgb percent) or
    ///     a:srgbClr (rgb hex)
    ///     .../a:prstClr/a:alpha/@val
    /// </remarks>
    private void SetAlphaChannel(Color c, string xPath)
    {
        var rgba = c.ToRgba32();

        //check 4 Alpha-values
        if (rgba.A != 255)
        {
            //opaque color => alpha == 255 //source: https://msdn.microsoft.com/en-us/library/1hstcth9%28v=vs.110%29.aspx
            //check path
            var s = SetXPath4Alpha(xPath);
            if (s.Length > 0)
            {
                var alpha = (rgba.A == 0 ? 0 : (100 - rgba.A) * 1000)
                    .ToString(); //note: excel writes 100% transparency (alpha=0) as "0" and not as "100000"
                SetXmlNodeString(s, alpha, true);
            }
        }
    }

    /// <summary>
    ///     read AlphaChannel from a:solidFill
    /// </summary>
    /// <param name="xPath"></param>
    /// <returns>alpha or 255 if their is no such node</returns>
    private int GetAlphaChannel(string xPath)
    {
        var r = 255;
        var s = SetXPath4Alpha(xPath);
        if (s.Length > 0)
            if (int.TryParse(GetXmlNodeString(s), NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
                r = i == 0 ? 0 : 100 - i / 1000;
        return r;
    }

    /// <summary>
    ///     creates xPath to alpha attribute for a color
    ///     eg: a:prstClr/a:alpha/@val
    /// </summary>
    /// <param name="xPath">xPath to color node</param>
    /// <returns></returns>
    private string SetXPath4Alpha(string xPath)
    {
        // ReSharper disable once RedundantAssignment
        var s = string.Empty;
        if (xPath.EndsWith("@val"))
            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            xPath = xPath[..xPath.IndexOf("@val")];
        if (xPath.EndsWith("/"))
            //cut tailing slash
            xPath = xPath[..^1];
        //parent node must be a color node/definition
        var colorDefs = new List<string>
            { "a:prstClr", "a:hslClr", "a:schemeClr", "a:sysClr", "a:scrgbClr", "a:srgbClr" };
        if (colorDefs.Find(cd => xPath.EndsWith(cd, StringComparison.Ordinal)) != null)
        {
            s = xPath + "/a:alpha/@val";
        }
        else
        {
            Debug.Assert(false);
            throw new InvalidOperationException("alpha-values can only set to Colors");
        }

        return s;
    }
}