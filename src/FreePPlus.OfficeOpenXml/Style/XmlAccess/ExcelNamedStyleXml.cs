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

using System.Xml;

namespace OfficeOpenXml.Style.XmlAccess;

/// <summary>
///     Xml access class for named styles
/// </summary>
public sealed class ExcelNamedStyleXml : StyleXmlHelper
{
    private const string idPath = "@xfId";
    private const string buildInIdPath = "@builtinId";
    private const string customBuiltinPath = "@customBuiltin";
    private const string namePath = "@name";
    private readonly ExcelStyles _styles;

    internal ExcelNamedStyleXml(XmlNamespaceManager nameSpaceManager, ExcelStyles styles)
        : base(nameSpaceManager)
    {
        _styles = styles;
        BuildInId = int.MinValue;
    }

    internal ExcelNamedStyleXml(XmlNamespaceManager NameSpaceManager, XmlNode topNode, ExcelStyles styles) :
        base(NameSpaceManager, topNode)
    {
        StyleXfId = GetXmlNodeInt(idPath);
        Name = GetXmlNodeString(namePath);
        BuildInId = GetXmlNodeInt(buildInIdPath);
        CustomBuildin = GetXmlNodeBool(customBuiltinPath);

        _styles = styles;
        Style = new ExcelStyle(styles, styles.NamedStylePropertyChange, -1, Name, StyleXfId);
    }

    internal override string Id => Name;

    /// <summary>
    ///     Named style index
    /// </summary>
    public int StyleXfId { get; set; }

    /// <summary>
    ///     Style index
    /// </summary>
    internal int XfId { get; set; } = int.MinValue;

    public int BuildInId { get; set; }
    public bool CustomBuildin { get; set; }

    /// <summary>
    ///     Name of the style
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     The style object
    /// </summary>
    public ExcelStyle Style { get; internal set; }

    internal override XmlNode CreateXmlNode(XmlNode topNode)
    {
        TopNode = topNode;
        SetXmlNodeString(namePath, Name);
        SetXmlNodeString("@xfId", _styles.CellStyleXfs[StyleXfId].newID.ToString());
        if (BuildInId >= 0) SetXmlNodeString("@builtinId", BuildInId.ToString());
        if (CustomBuildin) SetXmlNodeBool(customBuiltinPath, true);
        return TopNode;
    }
}