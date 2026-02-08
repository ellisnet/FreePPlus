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
 * Jan Källman		                Initial Release		        2009-10-01
 * Jan Källman		License changed GPL-->LGPL 2011-12-16
 *******************************************************************************/

using System;
using System.Globalization;
using System.Xml;
using CodeBrix.Imaging;
using CodeBrix.Imaging.Fonts;

namespace OfficeOpenXml.Style;

/// <summary>
///     Linestyle
/// </summary>
public enum eUnderLineType
{
    Dash,
    DashHeavy,
    DashLong,
    DashLongHeavy,
    Double,
    DotDash,
    DotDashHeavy,
    DotDotDash,
    DotDotDashHeavy,
    Dotted,
    DottedHeavy,
    Heavy,
    None,
    Single,
    Wavy,
    WavyDbl,
    WavyHeavy,
    Words
}

/// <summary>
///     Type of font strike
/// </summary>
public enum eStrikeType
{
    Double,
    No,
    Single
}

/// <summary>
///     Used by Rich-text and Paragraphs.
/// </summary>
public class ExcelTextFont : XmlHelper
{
    private readonly string _boldPath = "@b";

    private readonly string _colorPath = "a:solidFill/a:srgbClr/@val";

    private readonly string _fontCsPath = "a:cs/@typeface";

    private readonly string _fontLatinPath = "a:latin/@typeface";

    private readonly string _italicPath = "@i";
    private readonly string _path;
    private readonly XmlNode _rootNode;

    private readonly string _sizePath = "@sz";

    private readonly string _strikePath = "@strike";

    private readonly string _underLineColorPath = "a:uFill/a:solidFill/a:srgbClr/@val";

    private readonly string _underLinePath = "@u";

    internal ExcelTextFont(XmlNamespaceManager namespaceManager, XmlNode rootNode, string path,
        string[] schemaNodeOrder)
        : base(namespaceManager, rootNode)
    {
        SchemaNodeOrder = schemaNodeOrder;
        _rootNode = rootNode;
        if (path != "")
        {
            var node = rootNode.SelectSingleNode(path, namespaceManager);
            if (node != null) TopNode = node;
        }

        _path = path;
    }

    public string LatinFont
    {
        get => GetXmlNodeString(_fontLatinPath);
        set
        {
            CreateTopNode();
            SetXmlNodeString(_fontLatinPath, value);
        }
    }

    public string ComplexFont
    {
        get => GetXmlNodeString(_fontCsPath);
        set
        {
            CreateTopNode();
            SetXmlNodeString(_fontCsPath, value);
        }
    }

    public bool Bold
    {
        get => GetXmlNodeBool(_boldPath);
        set
        {
            CreateTopNode();
            SetXmlNodeString(_boldPath, value ? "1" : "0");
        }
    }

    public eUnderLineType UnderLine
    {
        get => TranslateUnderline(GetXmlNodeString(_underLinePath));
        set
        {
            CreateTopNode();
            SetXmlNodeString(_underLinePath, TranslateUnderlineText(value));
        }
    }

    public Color UnderLineColor
    {
        get
        {
            var col = GetXmlNodeString(_underLineColorPath);
            if (col == "")
                return Color.Empty;
            return Color.FromArgb(int.Parse(col, NumberStyles.AllowHexSpecifier));
        }
        set
        {
            CreateTopNode();
            SetXmlNodeString(_underLineColorPath, value.ToArgbInt32().ToString("X").Substring(2, 6));
        }
    }

    public bool Italic
    {
        get => GetXmlNodeBool(_italicPath);
        set
        {
            CreateTopNode();
            SetXmlNodeString(_italicPath, value ? "1" : "0");
        }
    }

    public eStrikeType Strike
    {
        get => TranslateStrike(GetXmlNodeString(_strikePath));
        set
        {
            CreateTopNode();
            SetXmlNodeString(_strikePath, TranslateStrikeText(value));
        }
    }

    public float Size
    {
        get => GetXmlNodeInt(_sizePath) / 100;
        set
        {
            CreateTopNode();
            SetXmlNodeString(_sizePath, ((int)(value * 100)).ToString());
        }
    }

    public Color Color
    {
        get
        {
            var col = GetXmlNodeString(_colorPath);
            if (col == "")
                return Color.Empty;
            return Color.FromArgb(int.Parse(col, NumberStyles.AllowHexSpecifier));
        }
        set
        {
            CreateTopNode();
            SetXmlNodeString(_colorPath, value.ToArgbInt32().ToString("X").Substring(2, 6));
        }
    }

    protected internal void CreateTopNode()
    {
        if (_path != "" && TopNode == _rootNode)
        {
            CreateNode(_path);
            TopNode = _rootNode.SelectSingleNode(_path, NameSpaceManager);
        }
    }

    /// <summary>
    ///     Set the font style from a font object
    /// </summary>
    /// <param name="font"></param>
    public void SetFromFont(Font font)
    {
        LatinFont = font.Name;
        ComplexFont = font.Name;
        Size = font.Size;
        if (font.IsBold) Bold = font.IsBold;
        if (font.IsItalic) Italic = font.IsItalic;
        if (font.IsUnderline) UnderLine = eUnderLineType.Single;
        if (font.IsStrikeout) Strike = eStrikeType.Single;
    }

    #region Translate Methods

    private eUnderLineType TranslateUnderline(string text)
    {
        switch (text)
        {
            case "sng":
                return eUnderLineType.Single;
            case "dbl":
                return eUnderLineType.Double;
            case "":
                return eUnderLineType.None;
            default:
                return (eUnderLineType)Enum.Parse(typeof(eUnderLineType), text);
        }
    }

    private string TranslateUnderlineText(eUnderLineType value)
    {
        switch (value)
        {
            case eUnderLineType.Single:
                return "sng";
            case eUnderLineType.Double:
                return "dbl";
            default:
                var ret = value.ToString();
                return ret[..1].ToLower(CultureInfo.InvariantCulture) + ret.Substring(1, ret.Length - 1);
        }
    }

    private eStrikeType TranslateStrike(string text)
    {
        switch (text)
        {
            case "dblStrike":
                return eStrikeType.Double;
            case "sngStrike":
                return eStrikeType.Single;
            default:
                return eStrikeType.No;
        }
    }

    private string TranslateStrikeText(eStrikeType value)
    {
        switch (value)
        {
            case eStrikeType.Single:
                return "sngStrike";
            case eStrikeType.Double:
                return "dblStrike";
            default:
                return "noStrike";
        }
    }

    #endregion
}