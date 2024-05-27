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
 * Starnuto Di Topo & Jan Källman   Added stream constructors
 *                                  and Load method Save as
 *                                  stream                      2010-03-14
 * Jan Källman		License changed GPL-->LGPL 2011-12-27
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Compatibility;
using OfficeOpenXml.Encryption;
using OfficeOpenXml.Packaging;
using OfficeOpenXml.Utils;
using OfficeOpenXml.Utils.CompundDocument;

#pragma warning disable IDE0059

namespace OfficeOpenXml;

/// <summary>
///     Maps to DotNetZips CompressionLevel enum
/// </summary>
public enum CompressionLevel
{
    Level0 = 0,
    None = 0,
    Level1 = 1,
    BestSpeed = 1,
    Level2 = 2,
    Level3 = 3,
    Level4 = 4,
    Level5 = 5,
    Level6 = 6,
    Default = 6,
    Level7 = 7,
    Level8 = 8,
    BestCompression = 9,
    Level9 = 9
}

/// <summary>
///     Represents an Excel 2007/2010 XLSX file package.
///     This is the top-level object to access all parts of the document.
/// </summary>
/// <remarks>
///     <example>
///         <code>
///     FileInfo newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");
/// 	if (newFile.Exists)
/// 	{
/// 		newFile.Delete();  // ensures we create a new workbook
/// 		newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");
/// 	}
/// 	using (ExcelPackage package = new ExcelPackage(newFile))
///     {
///         // add a new worksheet to the empty workbook
///         ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Inventory");
///         //Add the headers
///         worksheet.Cells[1, 1].Value = "ID";
///         worksheet.Cells[1, 2].Value = "Product";
///         worksheet.Cells[1, 3].Value = "Quantity";
///         worksheet.Cells[1, 4].Value = "Price";
///         worksheet.Cells[1, 5].Value = "Value";
/// 
///         //Add some items...
///         worksheet.Cells["A2"].Value = "12001";
///         worksheet.Cells["B2"].Value = "Nails";
///         worksheet.Cells["C2"].Value = 37;
///         worksheet.Cells["D2"].Value = 3.99;
/// 
///         worksheet.Cells["A3"].Value = "12002";
///         worksheet.Cells["B3"].Value = "Hammer";
///         worksheet.Cells["C3"].Value = 5;
///         worksheet.Cells["D3"].Value = 12.10;
/// 
///         worksheet.Cells["A4"].Value = "12003";
///         worksheet.Cells["B4"].Value = "Saw";
///         worksheet.Cells["C4"].Value = 12;
///         worksheet.Cells["D4"].Value = 15.37;
/// 
///         //Add a formula for the value-column
///         worksheet.Cells["E2:E4"].Formula = "C2*D2";
/// 
///            //Ok now format the values;
///         using (var range = worksheet.Cells[1, 1, 1, 5]) 
///          {
///             range.Style.Font.Bold = true;
///             range.Style.Fill.PatternType = ExcelFillStyle.Solid;
///             range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
///             range.Style.Font.Color.SetColor(Color.White);
///         }
/// 
///         worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
///         worksheet.Cells["A5:E5"].Style.Font.Bold = true;
/// 
///         worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2,3,4,3).Address);
///         worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
///         worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";
/// 
///         //Create an autofilter for the range
///         worksheet.Cells["A1:E4"].AutoFilter = true;
/// 
///         worksheet.Cells["A1:E5"].AutoFitColumns(0);
/// 
///         // lets set the header text 
///         worksheet.HeaderFooter.oddHeader.CenteredText = "&amp;24&amp;U&amp;\"Arial,Regular Bold\" Inventory";
///         // add the page number to the footer plus the total number of pages
///         worksheet.HeaderFooter.oddFooter.RightAlignedText =
///         string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
///         // add the sheet name to the footer
///         worksheet.HeaderFooter.oddFooter.CenteredText = ExcelHeaderFooter.SheetName;
///         // add the file path to the footer
///         worksheet.HeaderFooter.oddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;
/// 
///         worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
///         worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];
/// 
///          // Change the sheet view to show it in page layout mode
///           worksheet.View.PageLayoutView = true;
/// 
///         // set some document properties
///         package.Workbook.Properties.Title = "Invertory";
///         package.Workbook.Properties.Author = "Fred Person";
///         package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";
/// 
///         // set some extended property values
///         package.Workbook.Properties.Company = "AdventureWorks Inc.";
/// 
///         // set some custom property values
///         package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Fred Person");
///         package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "FreePPlus");
/// 
///         // save our new workbook and we are done!
///         package.Save();
/// 
///       }
/// 
///       return newFile.FullName;
/// </code>
///         More samples can be found at
///         <a href="https://github.com/JanKallman/EPPlus/">https://github.com/JanKallman/EPPlus/</a>
///     </example>
/// </remarks>
public sealed class ExcelPackage : IDisposable
{
    internal const bool PreserveWhitespace = false;

    internal static int NewDocId = 1;

    private static readonly object Locker = new();

    private CompatibilitySettings _compatibility;

    private ExcelEncryption _encryption;

    // ReSharper disable once InconsistentNaming
    internal Dictionary<string, ImageInfo> _images = new();
    private bool _isExternalStream;

    private Stream _stream;
    internal int WorksheetAddId = 0;

    /// <summary>
    ///     Returns a reference to the package
    /// </summary>
    public ZipPackage Package { get; private set; }

    /// <summary>
    ///     Information how and if the package is encrypted
    /// </summary>
    public ExcelEncryption Encryption
    {
        get
        {
            if (_encryption == null) _encryption = new ExcelEncryption();
            return _encryption;
        }
    }

    /// <summary>
    ///     Returns a reference to the workbook component within the package.
    ///     All worksheets and cells can be accessed through the workbook.
    /// </summary>
    public ExcelWorkbook Workbook
    {
        get
        {
            if (_workbook == null)
            {
                var nsm = CreateDefaultNsm();

                _workbook = new ExcelWorkbook(this, nsm);

                _workbook.GetExternalReferences();
                _workbook.GetDefinedNames();
            }

            return _workbook;
        }
    }

    /// <summary>
    ///     Automatically adjust drawing size when column width/row height are adjusted, depending on the drawings editBy
    ///     property.
    ///     Default True
    /// </summary>
    public bool DoAdjustDrawings { get; set; }

    /// <summary>
    ///     Compression option for the package
    /// </summary>
    public CompressionLevel Compression
    {
        get => Package.Compression;
        set => Package.Compression = value;
    }

    /// <summary>
    ///     Compatibility settings for older versions of EPPlus.
    /// </summary>
    public CompatibilitySettings Compatibility
    {
        get
        {
            if (_compatibility == null) _compatibility = new CompatibilitySettings(this);
            return _compatibility;
        }
    }

    #region Dispose

    /// <summary>
    ///     Closes the package.
    /// </summary>
    public void Dispose()
    {
        if (Package != null)
        {
            if (_isExternalStream == false && _stream != null && (_stream.CanRead || _stream.CanWrite)) CloseStream();
            Package.Close();
            if (_workbook != null) _workbook.Dispose();
            Package = null;
            _images = null;
            File = null;
            _workbook = null;
            _stream = null;
            _workbook = null;
            GC.Collect();
        }
    }

    #endregion

    internal ImageInfo AddImage(byte[] image)
    {
        return AddImage(image, null, "");
    }

    internal ImageInfo AddImage(byte[] image, Uri uri, string contentType)
    {
        var hashProvider = SHA1.Create();
        var hash = BitConverter.ToString(hashProvider.ComputeHash(image)).Replace("-", "");
        lock (_images)
        {
            if (_images.ContainsKey(hash))
            {
                _images[hash].RefCount++;
            }
            else
            {
                ZipPackagePart imagePart;
                if (uri == null)
                {
                    uri = GetNewUri(Package, "/xl/media/image{0}.jpg");
                    imagePart = Package.CreatePart(uri, "image/jpeg", CompressionLevel.None);
                }
                else
                {
                    imagePart = Package.CreatePart(uri, contentType, CompressionLevel.None);
                }

                var stream = imagePart.GetStream(FileMode.Create, FileAccess.Write);
                stream.Write(image, 0, image.GetLength(0));

                _images.Add(hash, new ImageInfo { Uri = uri, RefCount = 1, Hash = hash, Part = imagePart });
            }
        }

        return _images[hash];
    }

    internal ImageInfo LoadImage(byte[] image, Uri uri, ZipPackagePart imagePart)
    {
        var hashProvider = SHA1.Create();
        var hash = BitConverter.ToString(hashProvider.ComputeHash(image)).Replace("-", "");
        if (_images.ContainsKey(hash))
            _images[hash].RefCount++;
        else
            _images.Add(hash, new ImageInfo { Uri = uri, RefCount = 1, Hash = hash, Part = imagePart });
        return _images[hash];
    }

    internal void RemoveImage(string hash)
    {
        lock (_images)
        {
            if (_images.ContainsKey(hash))
            {
                var ii = _images[hash];
                ii.RefCount--;
                if (ii.RefCount == 0)
                {
                    Package.DeletePart(ii.Uri);
                    _images.Remove(hash);
                }
            }
        }
    }

    internal ImageInfo GetImageInfo(byte[] image)
    {
        var hashProvider = SHA1.Create();
        var hash = BitConverter.ToString(hashProvider.ComputeHash(image)).Replace("-", "");

        if (_images.ContainsKey(hash))
            return _images[hash];
        return null;
    }

    private Uri GetNewUri(ZipPackage package, string sUri)
    {
        Uri uri;
        do
        {
            uri = new Uri(string.Format(sUri, NewDocId++), UriKind.Relative);
        } while (package.PartExists(uri));

        return uri;
    }

    /// <summary>
    ///     Init values here
    /// </summary>
    private void Init()
    {
        DoAdjustDrawings = true;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //Add Support for codepage 1252

        var build = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, false);
        var c = build.Build();

        var vNew = c["FreePPlus:ExcelPackage:Compatibility:IsWorksheets1Based"];
        var vOld = c["EPPlus:ExcelPackage:Compatibility:IsWorksheets1Based"];

        if (vNew != null && bool.TryParse(vNew.ToLowerInvariant(), out var valNew))
        {
            Compatibility.IsWorksheets1Based = valNew;
        }
        else if (vOld != null && bool.TryParse(vOld.ToLowerInvariant(), out var valOld))
        {
            Compatibility.IsWorksheets1Based = valOld;
        }
    }

    /// <summary>
    ///     Create a new file from a template
    /// </summary>
    /// <param name="template">An existing xlsx file to use as a template</param>
    /// <param name="password">The password to decrypt the package.</param>
    /// <returns></returns>
    private void CreateFromTemplate(FileInfo template, string password)
    {
        if (template != null) template.Refresh();
        if (template?.Exists ?? false)
        {
            if (_stream == null) _stream = new MemoryStream();
            var ms = new MemoryStream();
            if (password != null)
            {
                Encryption.IsEncrypted = true;
                Encryption.Password = password;
                var encrHandler = new EncryptedPackageHandler();
                ms = encrHandler.DecryptPackage(template, Encryption);
                // ReSharper disable once RedundantAssignment
                encrHandler = null;
            }
            else
            {
                WriteFileToStream(template.FullName, ms);
            }

            try
            {
                //_package = Package.Open(_stream, FileMode.Open, FileAccess.ReadWrite);
                Package = new ZipPackage(ms);
            }
            catch (Exception ex)
            {
                if (password == null && CompoundDocument.IsCompoundDocument(ms))
                    throw new Exception(
                        "Can not open the package. Package is an OLE compound document. If this is an encrypted package, please supply the password",
                        ex);
                throw;
            }
        }
        else
        {
            throw new Exception("Passed invalid TemplatePath to Excel Template");
        }
        //return newFile;
    }

    private void ConstructNewFile(string password)
    {
        var ms = new MemoryStream();
        if (_stream == null) _stream = new MemoryStream();
        if (File != null) File.Refresh();
        if (File != null && File.Exists)
        {
            if (password != null)
            {
                var encrHandler = new EncryptedPackageHandler();
                Encryption.IsEncrypted = true;
                Encryption.Password = password;
                ms = encrHandler.DecryptPackage(File, Encryption);
                // ReSharper disable once RedundantAssignment
                encrHandler = null;
            }
            else
            {
                WriteFileToStream(File.FullName, ms);
            }

            try
            {
                //_package = Package.Open(_stream, FileMode.Open, FileAccess.ReadWrite);
                Package = new ZipPackage(ms);
            }
            catch (Exception ex)
            {
                if (password == null && CompoundDocument.IsCompoundDocument(File))
                    throw new Exception(
                        "Can not open the package. Package is an OLE compound document. If this is an encrypted package, please supply the password",
                        ex);
                throw;
            }
        }
        else
        {
            //_package = Package.Open(_stream, FileMode.Create, FileAccess.ReadWrite);
            Package = new ZipPackage(ms);
            CreateBlankWb();
        }
    }

    /// <summary>
    ///     Pull request from  perkuypers to read open Excel workbooks
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="stream">Stream</param>
    private static void WriteFileToStream(string path, Stream stream)
    {
        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var buffer = new byte[4096];
            int read;
            while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0) stream.Write(buffer, 0, read);
        }
    }

    private void CreateBlankWb()
    {
        _ = Workbook.WorkbookXml; // this will create the workbook xml in the package
        // create the relationship to the main part
        Package.CreateRelationship(UriHelper.GetRelativeUri(new Uri("/xl", UriKind.Relative),
            Workbook.WorkbookUri), TargetMode.Internal, SchemaRelationships + "/officeDocument");
    }

    private XmlNamespaceManager CreateDefaultNsm()
    {
        //  Create a NamespaceManager to handle the default namespace, 
        //  and create a prefix for the default namespace:
        var nt = new NameTable();
        var ns = new XmlNamespaceManager(nt);
        ns.AddNamespace(string.Empty, SchemaMain);
        ns.AddNamespace("d", SchemaMain);
        ns.AddNamespace("r", SchemaRelationships);
        ns.AddNamespace("c", SchemaChart);
        ns.AddNamespace("vt", SchemaVt);
        // extended properties (app.xml)
        ns.AddNamespace("xp", SchemaExtended);
        // custom properties
        ns.AddNamespace("ctp", SchemaCustom);
        // core properties
        ns.AddNamespace("cp", SchemaCore);
        // core property namespaces 
        ns.AddNamespace("dc", SchemaDc);
        ns.AddNamespace("dcterms", SchemaDcTerms);
        ns.AddNamespace("dcmitype", SchemaDcmiType);
        ns.AddNamespace("xsi", SchemaXsi);
        ns.AddNamespace("x14", SchemaMainX14);
        ns.AddNamespace("xm", SchemaMainXm);
        ns.AddNamespace("xr2", SchemaXr2);

        return ns;
    }

    #region GetXmlFromUri

    /// <summary>
    ///     Get the XmlDocument from an URI
    /// </summary>
    /// <param name="uri">The Uri to the part</param>
    /// <returns>The XmlDocument</returns>
    internal XmlDocument GetXmlFromUri(Uri uri)
    {
        var xml = new XmlDocument();
        var part = Package.GetPart(uri);
        XmlHelper.LoadXmlSafe(xml, part.GetStream());
        return xml;
    }

    #endregion

    /// <summary>
    ///     Saves and returns the Excel files as a bytearray.
    ///     Note that the package is closed upon save
    /// </summary>
    /// <example>
    ///     Example how to return a document from a Webserver...
    ///     <code> 
    ///  ExcelPackage package=new ExcelPackage();
    ///  /**** ... Create the document ****/
    ///  Byte[] bin = package.GetAsByteArray();
    ///  Response.ContentType = "Application/vnd.ms-Excel";
    ///  Response.AddHeader("content-disposition", "attachment;  filename=TheFile.xlsx");
    ///  Response.BinaryWrite(bin);
    /// </code>
    /// </example>
    /// <returns></returns>
    public byte[] GetAsByteArray()
    {
        return GetAsByteArray(true);
    }

    /// <summary>
    ///     Saves and returns the Excel files as a bytearray
    ///     Note that the package is closed upon save
    /// </summary>
    /// <example>
    ///     Example how to return a document from a Webserver...
    ///     <code> 
    ///  ExcelPackage package=new ExcelPackage();
    ///  /**** ... Create the document ****/
    ///  Byte[] bin = package.GetAsByteArray();
    ///  Response.ContentType = "Application/vnd.ms-Excel";
    ///  Response.AddHeader("content-disposition", "attachment;  filename=TheFile.xlsx");
    ///  Response.BinaryWrite(bin);
    /// </code>
    /// </example>
    /// <param name="password">
    ///     The password to encrypt the workbook with.
    ///     This parameter overrides the Encryption.Password.
    /// </param>
    /// <returns></returns>
    public byte[] GetAsByteArray(string password)
    {
        if (password != null) Encryption.Password = password;
        return GetAsByteArray(true);
    }

    internal byte[] GetAsByteArray(bool save)
    {
        if (save)
        {
            Workbook.Save();
            Package.Close();
            Package.Save(_stream);
        }

        var byRet = new byte[Stream.Length];
        var pos = Stream.Position;
        Stream.Seek(0, SeekOrigin.Begin);
        _ = Stream.Read(byRet, 0, (int)Stream.Length);

        //Encrypt Workbook?
        if (Encryption.IsEncrypted)
        {
            var eph = new EncryptedPackageHandler();
            var ms = eph.EncryptPackage(byRet, Encryption);
            byRet = ms.ToArray();
        }

        Stream.Seek(pos, SeekOrigin.Begin);
        Stream.Close();
        return byRet;
    }

    /// <summary>
    ///     Loads the specified package data from a stream.
    /// </summary>
    /// <param name="input">The input.</param>
    public void Load(Stream input)
    {
        Load(input, new MemoryStream(), null);
    }

    /// <summary>
    ///     Loads the specified package data from a stream.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="password">The password to decrypt the document</param>
    public void Load(Stream input, string password)
    {
        Load(input, new MemoryStream(), password);
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="password"></param>
    private void Load(Stream input, Stream output, string password)
    {
        //Release some resources:
        if (Package != null)
        {
            Package.Close();
            Package = null;
        }

        if (_stream != null)
        {
            _stream.Close();
            _stream.Dispose();
            _stream = null;
        }

        _isExternalStream = true;
        if (input.Length == 0) // Template is blank, Construct new
        {
            _stream = output;
            ConstructNewFile(password);
        }
        else
        {
            Stream ms;
            _stream = output;
            if (password != null)
            {
                Stream encrStream = new MemoryStream();
                CopyStream(input, ref encrStream);
                var eph = new EncryptedPackageHandler();
                Encryption.Password = password;
                ms = eph.DecryptPackage((MemoryStream)encrStream, Encryption);
            }
            else
            {
                ms = new MemoryStream();
                CopyStream(input, ref ms);
            }

            try
            {
                //this._package = Package.Open(this._stream, FileMode.Open, FileAccess.ReadWrite);
                Package = new ZipPackage(ms);
            }
            catch (Exception ex)
            {
                _ = new EncryptedPackageHandler();
                if (password == null && CompoundDocument.IsCompoundDocument((MemoryStream)_stream))
                    throw new Exception(
                        "Can not open the package. Package is an OLE compound document. If this is an encrypted package, please supply the password",
                        ex);
                throw;
            }
        }

        //Clear the workbook so that it gets reinitialized next time
        _workbook = null;
    }

    /// <summary>
    ///     Copies the input stream to the output stream.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <param name="outputStream">The output stream.</param>
    internal static void CopyStream(Stream inputStream, ref Stream outputStream)
    {
        if (!inputStream.CanRead) throw new Exception("Can not read from inputstream");
        if (!outputStream.CanWrite) throw new Exception("Can not write to outputstream");
        if (inputStream.CanSeek) inputStream.Seek(0, SeekOrigin.Begin);

        const int bufferLength = 8096;
        var buffer = new byte[bufferLength];
        lock (Locker)
        {
            var bytesRead = inputStream.Read(buffer, 0, bufferLength);
            // write the required bytes
            while (bytesRead > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesRead = inputStream.Read(buffer, 0, bufferLength);
            }

            outputStream.Flush();
        }
    }

    internal class ImageInfo
    {
        internal string Hash { get; set; }
        internal Uri Uri { get; set; }
        internal int RefCount { get; set; }
        internal ZipPackagePart Part { get; set; }
    }

    #region Properties

    /// <summary>
    ///     Extension Schema types
    /// </summary>
    internal const string SchemaXmlExtension = "application/xml";

    internal const string SchemaRelsExtension = "application/vnd.openxmlformats-package.relationships+xml";

    /// <summary>
    ///     Main Xml schema name
    /// </summary>
    internal const string SchemaMain = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    /// <summary>
    ///     Relationship schema name
    /// </summary>
    internal const string SchemaRelationships = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    internal const string SchemaDrawings = @"http://schemas.openxmlformats.org/drawingml/2006/main";
    internal const string SchemaSheetDrawings = @"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
    internal const string SchemaMarkupCompatibility = @"http://schemas.openxmlformats.org/markup-compatibility/2006";

    internal const string SchemaMicrosoftVml = @"urn:schemas-microsoft-com:vml";
    internal const string SchemaMicrosoftOffice = "urn:schemas-microsoft-com:office:office";
    internal const string SchemaMicrosoftExcel = "urn:schemas-microsoft-com:office:excel";

    internal const string SchemaChart = @"http://schemas.openxmlformats.org/drawingml/2006/chart";

    internal const string SchemaHyperlink =
        @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";

    internal const string SchemaComment =
        @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";

    internal const string SchemaImage = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";

    //Office properties
    internal const string SchemaCore = @"http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
    internal const string SchemaExtended = @"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
    internal const string SchemaCustom = @"http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
    internal const string SchemaDc = @"http://purl.org/dc/elements/1.1/";
    internal const string SchemaDcTerms = @"http://purl.org/dc/terms/";
    internal const string SchemaDcmiType = @"http://purl.org/dc/dcmitype/";
    internal const string SchemaXsi = @"http://www.w3.org/2001/XMLSchema-instance";
    internal const string SchemaVt = @"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

    internal const string SchemaMainX14 = "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
    internal const string SchemaMainXm = "http://schemas.microsoft.com/office/excel/2006/main";
    internal const string SchemaXr = "http://schemas.microsoft.com/office/spreadsheetml/2014/revision";
    internal const string SchemaXr2 = "http://schemas.microsoft.com/office/spreadsheetml/2015/revision2";

    //Pivottables
    internal const string SchemaPivotTable =
        @"application/vnd.openxmlformats-officedocument.spreadsheetml.pivotTable+xml";

    internal const string SchemaPivotCacheDefinition =
        @"application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheDefinition+xml";

    internal const string SchemaPivotCacheRecords =
        @"application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheRecords+xml";

    //VBA
    // ReSharper disable InconsistentNaming
    internal const string SchemaVBA = @"application/vnd.ms-office.vbaProject";

    internal const string SchemaVBASignature = @"application/vnd.ms-office.vbaProjectSignature";
    // ReSharper restore InconsistentNaming

    internal const string ContentTypeWorkbookDefault =
        @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";

    internal const string ContentTypeWorkbookMacroEnabled = "application/vnd.ms-excel.sheet.macroEnabled.main+xml";

    internal const string ContentTypeSharedString =
        @"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";

    //Package reference

    // ReSharper disable once InconsistentNaming
    internal ExcelWorkbook _workbook;

    /// <summary>
    ///     Maximum number of columns in a worksheet (16384).
    /// </summary>
    public const int MaxColumns = 16384;

    /// <summary>
    ///     Maximum number of rows in a worksheet (1048576).
    /// </summary>
    public const int MaxRows = 1048576;

    #endregion

    #region ExcelPackage Constructors

    /// <summary>
    ///     Create a new instance of the ExcelPackage.
    ///     Output is accessed through the Stream property, using the <see cref="SaveAs(FileInfo)" /> method or later set the
    ///     <see cref="File" /> property.
    /// </summary>
    public ExcelPackage()
    {
        Init();
        ConstructNewFile(null);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on an existing file or creates a new file.
    /// </summary>
    /// <param name="newFile">If newFile exists, it is opened.  Otherwise, it is created from scratch.</param>
    public ExcelPackage(FileInfo newFile)
    {
        Init();
        File = newFile;
        ConstructNewFile(null);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on an existing file or creates a new file.
    /// </summary>
    /// <param name="newFile">If newFile exists, it is opened.  Otherwise, it is created from scratch.</param>
    /// <param name="password">Password for an encrypted package</param>
    public ExcelPackage(FileInfo newFile, string password)
    {
        Init();
        File = newFile;
        ConstructNewFile(password);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a existing template.
    ///     If newFile exists, it will be overwritten when the Save method is called
    /// </summary>
    /// <param name="newFile">The name of the Excel file to be created</param>
    /// <param name="template">The name of the Excel template to use as the basis of the new Excel file</param>
    public ExcelPackage(FileInfo newFile, FileInfo template)
    {
        Init();
        File = newFile;
        CreateFromTemplate(template, null);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a existing template.
    ///     If newFile exists, it will be overwritten when the Save method is called
    /// </summary>
    /// <param name="newFile">The name of the Excel file to be created</param>
    /// <param name="template">The name of the Excel template to use as the basis of the new Excel file</param>
    /// <param name="password">Password to decrypted the template</param>
    public ExcelPackage(FileInfo newFile, FileInfo template, string password)
    {
        Init();
        File = newFile;
        CreateFromTemplate(template, password);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a existing template.
    /// </summary>
    /// <param name="template">The name of the Excel template to use as the basis of the new Excel file</param>
    /// <param name="useStream">if true use a stream. If false create a file in the temp dir with a random name</param>
    public ExcelPackage(FileInfo template, bool useStream)
    {
        Init();
        CreateFromTemplate(template, null);
        if (useStream == false) File = new FileInfo(Path.GetTempPath() + Guid.NewGuid() + ".xlsx");
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a existing template.
    /// </summary>
    /// <param name="template">The name of the Excel template to use as the basis of the new Excel file</param>
    /// <param name="useStream">if true use a stream. If false create a file in the temp dir with a random name</param>
    /// <param name="password">Password to decrypted the template</param>
    public ExcelPackage(FileInfo template, bool useStream, string password)
    {
        Init();
        CreateFromTemplate(template, password);
        if (useStream == false) File = new FileInfo(Path.GetTempPath() + Guid.NewGuid() + ".xlsx");
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a stream
    /// </summary>
    /// <param name="newStream">The stream object can be empty or contain a package. The stream must be Read/Write</param>
    public ExcelPackage(Stream newStream)
    {
        Init();
        if (newStream.Length == 0)
        {
            _stream = newStream;
            _isExternalStream = true;
            ConstructNewFile(null);
        }
        else
        {
            Load(newStream);
        }
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a stream
    /// </summary>
    /// <param name="newStream">The stream object can be empty or contain a package. The stream must be Read/Write</param>
    /// <param name="password">The password to decrypt the document</param>
    public ExcelPackage(Stream newStream, string password)
    {
        if (!(newStream.CanRead && newStream.CanWrite)) throw new Exception("The stream must be read/write");

        Init();
        if (newStream.Length > 0)
        {
            Load(newStream, password);
        }
        else
        {
            _stream = newStream;
            _isExternalStream = true;
            Package = new ZipPackage(_stream);
            CreateBlankWb();
        }
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a stream
    /// </summary>
    /// <param name="newStream">The output stream. Must be an empty read/write stream.</param>
    /// <param name="templateStream">This stream is copied to the output stream at load</param>
    public ExcelPackage(Stream newStream, Stream templateStream)
    {
        if (newStream.Length > 0)
            throw new Exception("The output stream must be empty. Length > 0");
        if (!(newStream.CanRead && newStream.CanWrite)) throw new Exception("The stream must be read/write");
        Init();
        Load(templateStream, newStream, null);
    }

    /// <summary>
    ///     Create a new instance of the ExcelPackage class based on a stream
    /// </summary>
    /// <param name="newStream">The output stream. Must be an empty read/write stream.</param>
    /// <param name="templateStream">This stream is copied to the output stream at load</param>
    /// <param name="password">Password to decrypted the template</param>
    public ExcelPackage(Stream newStream, Stream templateStream, string password)
    {
        if (newStream.Length > 0)
            throw new Exception("The output stream must be empty. Length > 0");
        if (!(newStream.CanRead && newStream.CanWrite)) throw new Exception("The stream must be read/write");
        Init();
        Load(templateStream, newStream, password);
    }

    #endregion

    #region SavePart

    /// <summary>
    ///     Saves the XmlDocument into the package at the specified Uri.
    /// </summary>
    /// <param name="uri">The Uri of the component</param>
    /// <param name="xmlDoc">The XmlDocument to save</param>
    internal void SavePart(Uri uri, XmlDocument xmlDoc)
    {
        var part = Package.GetPart(uri);
        var stream = part.GetStream(FileMode.Create, FileAccess.Write);
        var xr = new XmlTextWriter(stream, Encoding.UTF8);
        xr.Formatting = Formatting.None;

        xmlDoc.Save(xr);
    }

    /// <summary>
    ///     Saves the XmlDocument into the package at the specified Uri.
    /// </summary>
    /// <param name="uri">The Uri of the component</param>
    /// <param name="xmlDoc">The XmlDocument to save</param>
    internal void SaveWorkbook(Uri uri, XmlDocument xmlDoc)
    {
        var part = Package.GetPart(uri);
        if (Workbook.VbaProject == null)
        {
            if (part.ContentType != ContentTypeWorkbookDefault)
                part = Package.CreatePart(uri, ContentTypeWorkbookDefault, Compression);
        }
        else
        {
            if (part.ContentType != ContentTypeWorkbookMacroEnabled)
            {
                var rels = part.GetRelationships();
                Package.DeletePart(uri);
                part = Package.CreatePart(uri, ContentTypeWorkbookMacroEnabled);
                foreach (var rel in rels)
                {
                    Package.DeleteRelationship(rel.Id);
                    part.CreateRelationship(rel.TargetUri, rel.TargetMode, rel.RelationshipType);
                }
            }
        }

        var stream = part.GetStream(FileMode.Create, FileAccess.Write);
        var xr = new XmlTextWriter(stream, Encoding.UTF8);
        xr.Formatting = Formatting.None;

        xmlDoc.Save(xr);
    }

    #endregion

    #region Save // ExcelPackage save

    /// <summary>
    ///     Saves all the components back into the package.
    ///     This method recursively calls the Save method on all sub-components.
    ///     We close the package after the save is done.
    /// </summary>
    public void Save()
    {
        try
        {
            if (_stream is MemoryStream && _stream.Length > 0)
                //Close any open memorystream and "renew" then. This can occure if the package is saved twice. 
                //The stream is left open on save to enable the user to read the stream-property.
                //Non-memorystream streams will leave the closing to the user before saving a second time.
                CloseStream();

            Workbook.Save();
            if (File == null)
            {
                if (Encryption.IsEncrypted)
                {
                    var ms = new MemoryStream();
                    Package.Save(ms);
                    var file = ms.ToArray();
                    var eph = new EncryptedPackageHandler();
                    var msEnc = eph.EncryptPackage(file, Encryption);
                    CopyStream(msEnc, ref _stream);
                }
                else
                {
                    Package.Save(_stream);
                }

                _stream.Flush();
                Package.Close();
            }
            else
            {
                if (System.IO.File.Exists(File.FullName))
                    try
                    {
                        System.IO.File.Delete(File.FullName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error overwriting file {0}", File.FullName), ex);
                    }

                Package.Save(_stream);
                Package.Close();
                if (Stream is MemoryStream)
                {
                    var fi = new FileStream(File.FullName, FileMode.Create);
                    //EncryptPackage
                    if (Encryption.IsEncrypted)
                    {
                        var file = ((MemoryStream)Stream).ToArray();
                        var eph = new EncryptedPackageHandler();
                        var ms = eph.EncryptPackage(file, Encryption);

                        fi.Write(ms.ToArray(), 0, (int)ms.Length);
                    }
                    else
                    {
                        fi.Write(((MemoryStream)Stream).ToArray(), 0, (int)Stream.Length);
                    }

                    fi.Close();
                    fi.Dispose();
                }
                else
                {
                    System.IO.File.WriteAllBytes(File.FullName, GetAsByteArray(false));
                }
            }
        }
        catch (Exception ex)
        {
            if (File == null)
                throw;
            throw new InvalidOperationException(string.Format("Error saving file {0}", File.FullName), ex);
        }
    }

    /// <summary>
    ///     Saves all the components back into the package.
    ///     This method recursively calls the Save method on all sub-components.
    ///     The package is closed after it has ben saved
    ///     d to encrypt the workbook with.
    /// </summary>
    /// <param name="password">This parameter overrides the Workbook.Encryption.Password.</param>
    public void Save(string password)
    {
        Encryption.Password = password;
        Save();
    }

    /// <summary>
    ///     Saves the workbook to a new file
    ///     The package is closed after it has been saved
    /// </summary>
    /// <param name="file">The file location</param>
    public void SaveAs(FileInfo file)
    {
        File = file;
        Save();
    }

    /// <summary>
    ///     Saves the workbook to a new file
    ///     The package is closed after it has been saved
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="password">
    ///     The password to encrypt the workbook with.
    ///     This parameter overrides the Encryption.Password.
    /// </param>
    public void SaveAs(FileInfo file, string password)
    {
        File = file;
        Encryption.Password = password;
        Save();
    }

    /// <summary>
    ///     Copies the Package to the Outstream
    ///     The package is closed after it has been saved
    /// </summary>
    /// <param name="outputStream">The stream to copy the package to</param>
    public void SaveAs(Stream outputStream)
    {
        File = null;
        Save();

        if (outputStream != _stream) CopyStream(_stream, ref outputStream);
    }

    /// <summary>
    ///     Copies the Package to the Outstream
    ///     The package is closed after it has been saved
    /// </summary>
    /// <param name="outputStream">The stream to copy the package to</param>
    /// <param name="password">
    ///     The password to encrypt the workbook with.
    ///     This parameter overrides the Encryption.Password.
    /// </param>
    public void SaveAs(Stream outputStream, string password)
    {
        Encryption.Password = password;
        SaveAs(outputStream);
    }

    /// <summary>
    ///     The output file. Null if no file is used
    /// </summary>
    public FileInfo File { get; set; }

    /// <summary>
    ///     Close the internal stream
    /// </summary>
    internal void CloseStream()
    {
        // Issue15252: Clear output buffer
        if (_stream != null)
        {
            _stream.Close();
            _stream.Dispose();
        }

        _stream = new MemoryStream();
    }

    /// <summary>
    ///     The output stream. This stream is the not the encrypted package.
    ///     To get the encrypted package use the SaveAs(stream) method.
    /// </summary>
    public Stream Stream => _stream;

    #endregion
}