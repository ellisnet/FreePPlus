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
///     Xml access class for border top level
/// </summary>
public sealed class ExcelBorderXml : StyleXmlHelper
{
    private const string leftPath = "d:left";
    private const string rightPath = "d:right";
    private const string topPath = "d:top";
    private const string bottomPath = "d:bottom";
    private const string diagonalPath = "d:diagonal";
    private const string diagonalUpPath = "@diagonalUp";
    private const string diagonalDownPath = "@diagonalDown";

    internal ExcelBorderXml(XmlNamespaceManager nameSpaceManager)
        : base(nameSpaceManager) { }

    internal ExcelBorderXml(XmlNamespaceManager nsm, XmlNode topNode) :
        base(nsm, topNode)
    {
        Left = new ExcelBorderItemXml(nsm, topNode.SelectSingleNode(leftPath, nsm));
        Right = new ExcelBorderItemXml(nsm, topNode.SelectSingleNode(rightPath, nsm));
        Top = new ExcelBorderItemXml(nsm, topNode.SelectSingleNode(topPath, nsm));
        Bottom = new ExcelBorderItemXml(nsm, topNode.SelectSingleNode(bottomPath, nsm));
        Diagonal = new ExcelBorderItemXml(nsm, topNode.SelectSingleNode(diagonalPath, nsm));
        DiagonalUp = GetBoolValue(topNode, diagonalUpPath);
        DiagonalDown = GetBoolValue(topNode, diagonalDownPath);
    }

    internal override string Id => Left.Id + Right.Id + Top.Id + Bottom.Id + Diagonal.Id + DiagonalUp + DiagonalDown;

    /// <summary>
    ///     Left border style properties
    /// </summary>
    public ExcelBorderItemXml Left { get; internal set; }

    /// <summary>
    ///     Right border style properties
    /// </summary>
    public ExcelBorderItemXml Right { get; internal set; }

    /// <summary>
    ///     Top border style properties
    /// </summary>
    public ExcelBorderItemXml Top { get; internal set; }

    /// <summary>
    ///     Bottom border style properties
    /// </summary>
    public ExcelBorderItemXml Bottom { get; internal set; }

    /// <summary>
    ///     Diagonal border style properties
    /// </summary>
    public ExcelBorderItemXml Diagonal { get; internal set; }

    /// <summary>
    ///     Diagonal up border
    /// </summary>
    public bool DiagonalUp { get; internal set; }

    /// <summary>
    ///     Diagonal down border
    /// </summary>
    public bool DiagonalDown { get; internal set; }

    internal ExcelBorderXml Copy()
    {
        var newBorder = new ExcelBorderXml(NameSpaceManager);
        newBorder.Bottom = Bottom.Copy();
        newBorder.Diagonal = Diagonal.Copy();
        newBorder.Left = Left.Copy();
        newBorder.Right = Right.Copy();
        newBorder.Top = Top.Copy();
        newBorder.DiagonalUp = DiagonalUp;
        newBorder.DiagonalDown = DiagonalDown;

        return newBorder;
    }

    internal override XmlNode CreateXmlNode(XmlNode topNode)
    {
        TopNode = topNode;
        CreateNode(leftPath);
        topNode.AppendChild(Left.CreateXmlNode(TopNode.SelectSingleNode(leftPath, NameSpaceManager)));
        CreateNode(rightPath);
        topNode.AppendChild(Right.CreateXmlNode(TopNode.SelectSingleNode(rightPath, NameSpaceManager)));
        CreateNode(topPath);
        topNode.AppendChild(Top.CreateXmlNode(TopNode.SelectSingleNode(topPath, NameSpaceManager)));
        CreateNode(bottomPath);
        topNode.AppendChild(Bottom.CreateXmlNode(TopNode.SelectSingleNode(bottomPath, NameSpaceManager)));
        CreateNode(diagonalPath);
        topNode.AppendChild(Diagonal.CreateXmlNode(TopNode.SelectSingleNode(diagonalPath, NameSpaceManager)));
        if (DiagonalUp) SetXmlNodeString(diagonalUpPath, "1");
        if (DiagonalDown) SetXmlNodeString(diagonalDownPath, "1");
        return topNode;
    }
}