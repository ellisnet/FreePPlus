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
 * Jan Källman		    Added		10-AUG-2010
 * Jan Källman		    License changed GPL-->LGPL 2011-12-27
 *******************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace OfficeOpenXml;

/// <summary>
///     Sets protection on the workbook level
///     <seealso cref="ExcelEncryption" />
///     <seealso cref="ExcelSheetProtection" />
/// </summary>
public class ExcelProtection : XmlHelper
{
    private const string workbookPasswordPath = "d:workbookProtection/@workbookPassword";
    private const string workbookAlgorithmNamePath = "d:workbookProtection/@workbookAlgorithmName";
    private const string workbookHashValuePath = "d:workbookProtection/@workbookHashValue";
    private const string workbookSaltValuePath = "d:workbookProtection/@workbookSaltValue";
    private const string workbookSpinCountPath = "d:workbookProtection/@workbookSpinCount";
    private const string lockStructurePath = "d:workbookProtection/@lockStructure";
    private const string lockWindowsPath = "d:workbookProtection/@lockWindows";
    private const string lockRevisionPath = "d:workbookProtection/@lockRevision";

    internal ExcelProtection(XmlNamespaceManager ns, XmlNode topNode, ExcelWorkbook wb) :
        base(ns, topNode)
    {
        SchemaNodeOrder = wb.SchemaNodeOrder;
    }

    /// <summary>
    ///     Locks the structure,which prevents users from adding or deleting worksheets or from displaying hidden worksheets.
    /// </summary>
    public bool LockStructure
    {
        get => GetXmlNodeBool(lockStructurePath, false);
        set => SetXmlNodeBool(lockStructurePath, value, false);
    }

    /// <summary>
    ///     Locks the position of the workbook window.
    /// </summary>
    public bool LockWindows
    {
        get => GetXmlNodeBool(lockWindowsPath, false);
        set => SetXmlNodeBool(lockWindowsPath, value, false);
    }

    /// <summary>
    ///     Lock the workbook for revision
    /// </summary>
    public bool LockRevision
    {
        get => GetXmlNodeBool(lockRevisionPath, false);
        set => SetXmlNodeBool(lockRevisionPath, value, false);
    }

    /// <summary>
    ///     Sets a password for the workbook. This does not encrypt the workbook.
    /// </summary>
    /// <param name="Password">The password. </param>
    public void SetPassword(string Password)
    {
        if (string.IsNullOrEmpty(Password))
        {
            DeleteNode(workbookPasswordPath);
            DeleteNode(workbookAlgorithmNamePath);
            DeleteNode(workbookHashValuePath);
            DeleteNode(workbookSaltValuePath);
            DeleteNode(workbookSpinCountPath);
        }
        else
        {
            // Remove legacy password attribute if present
            DeleteNode(workbookPasswordPath);

            // Use SHA-512 with salt and spin count (OOXML standard)
            var byPwd = Encoding.Unicode.GetBytes(Password);
            var bySalt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bySalt);

            const int spinCount = 100000;

            using var hp = SHA512.Create();
            var buffer = new byte[bySalt.Length + byPwd.Length];
            Array.Copy(bySalt, buffer, bySalt.Length);
            Array.Copy(byPwd, 0, buffer, bySalt.Length, byPwd.Length);
            var hash = hp.ComputeHash(buffer);

            for (var i = 0; i < spinCount; i++)
            {
                buffer = new byte[hash.Length + 4];
                Array.Copy(hash, buffer, hash.Length);
                Array.Copy(BitConverter.GetBytes(i), 0, buffer, hash.Length, 4);
                hash = hp.ComputeHash(buffer);
            }

            SetXmlNodeString(workbookAlgorithmNamePath, "SHA-512");
            SetXmlNodeString(workbookSaltValuePath, Convert.ToBase64String(bySalt));
            SetXmlNodeString(workbookHashValuePath, Convert.ToBase64String(hash));
            SetXmlNodeString(workbookSpinCountPath, spinCount.ToString());
        }
    }
}