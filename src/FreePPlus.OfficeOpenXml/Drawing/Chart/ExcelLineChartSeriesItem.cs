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
using System.Globalization;
using System.Xml;
using FreePPlus.Imaging;

namespace OfficeOpenXml.Drawing.Chart;

/// <summary>
///     A series item for a line chart
/// </summary>
public sealed class ExcelLineChartSeriesItem : ExcelChartSeriesItem
{
    private const string MarkerPath = "c:marker/c:symbol/@val";

    private const string SmoothPath = "c:smooth/@val";

    //new properties for excel line charts: LineColor, MarkerSize, LineWidth and MarkerLineColor 
    //implemented according to https://epplus.codeplex.com/discussions/287917
    private const string LineColorPath = "c:spPr/a:ln/a:solidFill/a:srgbClr/@val";

    private const string MarkerSizePath = "c:marker/c:size/@val";

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
    internal ExcelLineChartSeriesItem(ExcelChartSeries chartSeries, XmlNamespaceManager ns, XmlNode node,
        bool isPivot) :
        base(chartSeries, ns, node, isPivot) { }

    /// <summary>
    ///     Datalabels
    /// </summary>
    public ExcelChartSeriesDataLabel DataLabel
    {
        get
        {
            if (_dataLabel == null) _dataLabel = new ExcelChartSeriesDataLabel(_ns, _node);
            return _dataLabel;
        }
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
        set => SetXmlNodeString(MarkerPath, value.ToString().ToLower(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Smooth lines
    /// </summary>
    public bool Smooth
    {
        get => GetXmlNodeBool(SmoothPath, false);
        set => SetXmlNodeBool(SmoothPath, value);
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
                return Color.Black;
            return Color.FromArgb(Convert.ToInt32(color, 16));
        }
        set => SetXmlNodeString(LineColorPath, value.ToArgbInt32().ToString("X")[2..], true);
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
                return Color.Black;
            return Color.FromArgb(Convert.ToInt32(color, 16));
        }
        set => SetXmlNodeString(MarkerLineColorPath, value.ToArgbInt32().ToString("X")[2..], true);
    }
}