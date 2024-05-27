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
using FreePPlus.Imaging;

namespace OfficeOpenXml.Style.XmlAccess;

/// <summary>
///     Xml access class for color
/// </summary>
public sealed class ExcelColorXml : StyleXmlHelper
{
    private bool _auto;

    private int _indexed;
    private string _rgb;
    private decimal _tint;

    internal ExcelColorXml(XmlNamespaceManager nameSpaceManager)
        : base(nameSpaceManager)
    {
        _auto = false;
        Theme = "";
        _tint = 0;
        _rgb = "";
        _indexed = int.MinValue;
    }

    internal ExcelColorXml(XmlNamespaceManager nsm, XmlNode topNode) :
        base(nsm, topNode)
    {
        if (topNode == null)
        {
            Exists = false;
        }
        else
        {
            Exists = true;
            _auto = GetXmlNodeBool("@auto");
            Theme = GetXmlNodeString("@theme");
            _tint = GetXmlNodeDecimalNull("@tint") ?? decimal.MinValue;
            _rgb = GetXmlNodeString("@rgb");
            _indexed = GetXmlNodeIntNull("@indexed") ?? int.MinValue;
        }
    }

    internal override string Id => _auto + "|" + Theme + "|" + _tint + "|" + _rgb + "|" + _indexed;

    public bool Auto
    {
        get => _auto;
        set
        {
            _auto = value;
            Exists = true;
            Clear();
        }
    }

    /// <summary>
    ///     Theme color value
    /// </summary>
    public string Theme { get; private set; }

    /// <summary>
    ///     Tint
    /// </summary>
    public decimal Tint
    {
        get
        {
            if (_tint == decimal.MinValue)
                return 0;
            return _tint;
        }
        set
        {
            _tint = value;
            Exists = true;
        }
    }

    /// <summary>
    ///     RGB value
    /// </summary>
    public string Rgb
    {
        get => _rgb;
        set
        {
            _rgb = value;
            Exists = true;
            _indexed = int.MinValue;
            _auto = false;
        }
    }

    /// <summary>
    ///     Indexed color value
    /// </summary>
    public int Indexed
    {
        get => _indexed == int.MinValue ? 0 : _indexed;
        set
        {
            if (value < 0 || value > 65) throw new ArgumentOutOfRangeException("Index out of range");
            Clear();
            _indexed = value;
            Exists = true;
        }
    }

    internal bool Exists { get; private set; }

    internal void Clear()
    {
        Theme = "";
        _tint = decimal.MinValue;
        _indexed = int.MinValue;
        _rgb = "";
        _auto = false;
    }

    public void SetColor(Color color)
    {
        Clear();
        _rgb = color.ToArgbInt32().ToString("X");
    }

    internal ExcelColorXml Copy()
    {
        return new ExcelColorXml(NameSpaceManager)
            { _indexed = _indexed, _tint = _tint, _rgb = _rgb, Theme = Theme, _auto = _auto, Exists = Exists };
    }

    internal override XmlNode CreateXmlNode(XmlNode topNode)
    {
        TopNode = topNode;
        if (_rgb != "")
            SetXmlNodeString("@rgb", _rgb);
        else if (_indexed >= 0)
            SetXmlNodeString("@indexed", _indexed.ToString());
        else if (_auto)
            SetXmlNodeBool("@auto", _auto);
        else
            SetXmlNodeString("@theme", Theme);
        if (_tint != decimal.MinValue) SetXmlNodeString("@tint", _tint.ToString(CultureInfo.InvariantCulture));
        return TopNode;
    }
}