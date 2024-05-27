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

namespace OfficeOpenXml.Style.XmlAccess;

/// <summary>
///     Xml access class for fonts
/// </summary>
public sealed class ExcelFontXml : StyleXmlHelper
{
    internal ExcelFontXml(XmlNamespaceManager nameSpaceManager)
        : base(nameSpaceManager)
    {
        _name = "";
        Size = 0;
        _family = int.MinValue;
        Scheme = "";
        Color = Color = new ExcelColorXml(NameSpaceManager);
        Bold = false;
        Italic = false;
        Strike = false;
        UnderLineType = ExcelUnderLineType.None;
        VerticalAlign = "";
    }

    internal ExcelFontXml(XmlNamespaceManager nsm, XmlNode topNode) :
        base(nsm, topNode)
    {
        _name = GetXmlNodeString(namePath);
        Size = (float)GetXmlNodeDecimal(sizePath);
        _family = GetXmlNodeIntNull(familyPath) ?? int.MinValue;
        Scheme = GetXmlNodeString(schemePath);
        Color = new ExcelColorXml(nsm, topNode.SelectSingleNode(_colorPath, nsm));
        Bold = GetBoolValue(topNode, boldPath);
        Italic = GetBoolValue(topNode, italicPath);
        Strike = GetBoolValue(topNode, strikePath);
        VerticalAlign = GetXmlNodeString(verticalAlignPath);
        if (topNode.SelectSingleNode(underLinedPath, NameSpaceManager) != null)
        {
            var ut = GetXmlNodeString(underLinedPath + "/@val");
            if (ut == "")
                UnderLineType = ExcelUnderLineType.Single;
            else
                UnderLineType = (ExcelUnderLineType)Enum.Parse(typeof(ExcelUnderLineType), ut, true);
        }
        else
        {
            UnderLineType = ExcelUnderLineType.None;
        }
    }

    internal override string Id => Name + "|" + Size + "|" + Family + "|" + Color.Id + "|" + Scheme + "|" + Bold + "|" +
                                   Italic + "|" + Strike + "|" + VerticalAlign + "|" + UnderLineType;

    private const string namePath = "d:name/@val";
    private string _name;

    /// <summary>
    ///     The name of the font
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            Scheme = ""; //Reset schema to avoid corrupt file if unsupported font is selected.
            _name = value;
        }
    }

    private const string sizePath = "d:sz/@val";

    /// <summary>
    ///     Font size
    /// </summary>
    public float Size { get; set; }

    private const string familyPath = "d:family/@val";
    private int _family;

    /// <summary>
    ///     Font family
    /// </summary>
    public int Family
    {
        get => _family == int.MinValue ? 0 : _family;
        set => _family = value;
    }

    private const string _colorPath = "d:color";

    /// <summary>
    ///     Text color
    /// </summary>
    public ExcelColorXml Color { get; internal set; }

    private const string schemePath = "d:scheme/@val";

    /// <summary>
    ///     Font Scheme
    /// </summary>
    public string Scheme { get; private set; } = "";

    private const string boldPath = "d:b";

    /// <summary>
    ///     If the font is bold
    /// </summary>
    public bool Bold { get; set; }

    private const string italicPath = "d:i";

    /// <summary>
    ///     If the font is italic
    /// </summary>
    public bool Italic { get; set; }

    private const string strikePath = "d:strike";

    /// <summary>
    ///     If the font is striked out
    /// </summary>
    public bool Strike { get; set; }

    private const string underLinedPath = "d:u";

    /// <summary>
    ///     If the font is underlined.
    ///     When set to true a the text is underlined with a single line
    /// </summary>
    public bool UnderLine
    {
        get => UnderLineType != ExcelUnderLineType.None;
        set => UnderLineType = value ? ExcelUnderLineType.Single : ExcelUnderLineType.None;
    }

    /// <summary>
    ///     If the font is underlined
    /// </summary>
    public ExcelUnderLineType UnderLineType { get; set; }

    private const string verticalAlignPath = "d:vertAlign/@val";

    /// <summary>
    ///     Vertical aligned
    /// </summary>
    public string VerticalAlign { get; set; }

#if ENABLE_SET_FROM_FONT
        public void SetFromFont(System.Drawing.Font Font)
        {
            Name = Font.Name;
            //Family=fnt.FontFamily.;
            Size = (int)Font.Size;
            Strike = Font.Strikeout;
            Bold = Font.Bold;
            UnderLine = Font.Underline;
            Italic = Font.Italic;
        }
#endif

    public static float GetFontHeight(string name, float size)
    {
        name = name.StartsWith("@") ? name[1..] : name;
        if (FontSize.FontHeights.ContainsKey(name))
            return GetHeightByName(name, size);
        return GetHeightByName("Calibri", size);
    }

    private static float GetHeightByName(string name, float size)
    {
        if (FontSize.FontHeights[name].ContainsKey(size)) return FontSize.FontHeights[name][size].Height;

        float min = -1, max = float.MaxValue;
        foreach (var h in FontSize.FontHeights[name])
        {
            if (min < h.Key && h.Key < size) min = h.Key;
            if (max > h.Key && h.Key > size) max = h.Key;
        }

        if (min == max || max == float.MaxValue)
            return Convert.ToSingle(FontSize.FontHeights[name][min].Height);
        if (min == -1)
            return Convert.ToSingle(FontSize.FontHeights[name][max].Height);
        return Convert.ToSingle(FontSize.FontHeights[name][min].Height +
                                (FontSize.FontHeights[name][max].Height - FontSize.FontHeights[name][min].Height) *
                                ((size - min) / (max - min)));
    }

    internal ExcelFontXml Copy()
    {
        var newFont = new ExcelFontXml(NameSpaceManager);
        newFont.Name = _name;
        newFont.Size = Size;
        newFont.Family = _family;
        newFont.Scheme = Scheme;
        newFont.Bold = Bold;
        newFont.Italic = Italic;
        newFont.UnderLineType = UnderLineType;
        newFont.Strike = Strike;
        newFont.VerticalAlign = VerticalAlign;
        newFont.Color = Color.Copy();
        return newFont;
    }

    internal override XmlNode CreateXmlNode(XmlNode topElement)
    {
        TopNode = topElement;

        if (Bold) CreateNode(boldPath);
        else DeleteAllNode(boldPath);
        if (Italic) CreateNode(italicPath);
        else DeleteAllNode(italicPath);
        if (Strike) CreateNode(strikePath);
        else DeleteAllNode(strikePath);

        if (UnderLineType == ExcelUnderLineType.None)
        {
            DeleteAllNode(underLinedPath);
        }
        else if (UnderLineType == ExcelUnderLineType.Single)
        {
            CreateNode(underLinedPath);
        }
        else
        {
            var v = UnderLineType.ToString();
            SetXmlNodeString(underLinedPath + "/@val", v[..1].ToLower(CultureInfo.InvariantCulture) + v[1..]);
        }

        if (VerticalAlign != "") SetXmlNodeString(verticalAlignPath, VerticalAlign);
        if (Size > 0) SetXmlNodeString(sizePath, Size.ToString(CultureInfo.InvariantCulture));
        if (Color.Exists)
        {
            CreateNode(_colorPath);
            TopNode.AppendChild(Color.CreateXmlNode(TopNode.SelectSingleNode(_colorPath, NameSpaceManager)));
        }

        if (!string.IsNullOrEmpty(_name)) SetXmlNodeString(namePath, _name);
        if (_family > int.MinValue) SetXmlNodeString(familyPath, _family.ToString());
        if (Scheme != "") SetXmlNodeString(schemePath, Scheme);

        return TopNode;
    }
}