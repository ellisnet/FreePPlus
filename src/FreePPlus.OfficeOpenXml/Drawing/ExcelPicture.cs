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

using CodeBrix.Imaging;
using CodeBrix.Imaging.Formats.Bmp;
using CodeBrix.Imaging.Formats.Gif;
using CodeBrix.Imaging.Formats.Jpeg;
using CodeBrix.Imaging.Formats.Pbm;
using CodeBrix.Imaging.Formats.Png;
using CodeBrix.Imaging.Formats.Tga;
using CodeBrix.Imaging.Formats.Tiff;
using CodeBrix.Imaging.Formats.Webp;
using CodeBrix.Imaging.PixelFormats;
using OfficeOpenXml.Compatibility;
using OfficeOpenXml.Packaging;
using OfficeOpenXml.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Formats = CodeBrix.Imaging.Formats;

namespace OfficeOpenXml.Drawing;

/// <summary>
///     An image object
/// </summary>
public sealed class ExcelPicture : ExcelDrawing
{
    private ExcelDrawingBorder _border;
    private ExcelDrawingFill _fill;

    private Image _image;

    internal ZipPackagePart Part;

    internal string ImageHash { get; set; }

    /// <summary>
    ///     The Image
    /// </summary>
    public Image Image
    {
        get => _image;
        set
        {
            if (value != null)
            {
                _image = value;
                try
                {
                    var relID = SavePicture(value);

                    //Create relationship
                    TopNode.SelectSingleNode("xdr:pic/xdr:blipFill/a:blip/@r:embed", NameSpaceManager).Value = relID;
                    //_image.Save(Part.GetStream(FileMode.Create, FileAccess.Write), _imageFormat);   //Always JPEG here at this point. 
                }
                catch (Exception ex)
                {
                    throw new Exception("Can't save image - " + ex.Message, ex);
                }
            }
        }
    }

    /// <summary>
    ///     Image format
    ///     If the picture is created from an Image this type is always Jpeg
    /// </summary>
    public Formats.IImageFormat ImageFormat { get; internal set; } = JpegFormat.Instance;

    internal string ContentType { get; set; }

    internal Uri UriPic { get; set; }
    internal ZipPackageRelationship RelPic { get; set; }
    internal ZipPackageRelationship HypRel { get; set; }

    internal new string Id => Name;

    public static Image CreateImage(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        //New images default to JPEG format.
        return new Image<Rgba32>(width, height, JpegFormat.Instance);
    }

    /// <summary>
    ///     Fill
    /// </summary>
    public ExcelDrawingFill Fill
    {
        get
        {
            if (_fill == null) _fill = new ExcelDrawingFill(NameSpaceManager, TopNode, "xdr:pic/xdr:spPr");
            return _fill;
        }
    }

    /// <summary>
    ///     Border
    /// </summary>
    public ExcelDrawingBorder Border
    {
        get
        {
            if (_border == null) _border = new ExcelDrawingBorder(NameSpaceManager, TopNode, "xdr:pic/xdr:spPr/a:ln");
            return _border;
        }
    }

    /// <summary>
    ///     Hyperlink
    /// </summary>
    public Uri Hyperlink { get; private set; }

    private string SavePicture(Image image)
    {
        var img = ImageCompat.GetImageAsByteArray(image);
        var ii = _drawings._package.AddImage(img);


        ImageHash = ii.Hash;
        if (_drawings._hashes.ContainsKey(ii.Hash))
        {
            var relID = _drawings._hashes[ii.Hash];
            var rel = _drawings.Part.GetRelationship(relID);
            UriPic = UriHelper.ResolvePartUri(rel.SourceUri, rel.TargetUri);
            return relID;
        }

        UriPic = ii.Uri;
        ImageHash = ii.Hash;

        //Set the Image and save it to the package.
        RelPic = _drawings.Part.CreateRelationship(UriHelper.GetRelativeUri(_drawings.UriDrawing, UriPic),
            TargetMode.Internal, ExcelPackage.SchemaRelationships + "/image");

        //AddNewPicture(img, picRelation.Id);
        _drawings._hashes.Add(ii.Hash, RelPic.Id);

        return RelPic.Id;
    }

    private void SetPosDefaults(Image image)
    {
        EditAs = eEditAs.OneCell;
        SetPixelWidth(image.Width, image.Metadata.HorizontalResolution);
        SetPixelHeight(image.Height, image.Metadata.VerticalResolution);
    }

    private string PicStartXml()
    {
        var xml = new StringBuilder();

        xml.Append("<xdr:nvPicPr>");

        if (Hyperlink == null)
        {
            xml.AppendFormat("<xdr:cNvPr id=\"{0}\" descr=\"\" />", _id);
        }
        else
        {
            HypRel = _drawings.Part.CreateRelationship(Hyperlink, TargetMode.External, ExcelPackage.SchemaHyperlink);
            xml.AppendFormat("<xdr:cNvPr id=\"{0}\" descr=\"\">", _id);
            if (HypRel != null)
            {
                if (Hyperlink is ExcelHyperLink)
                    xml.AppendFormat(
                        "<a:hlinkClick xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:id=\"{0}\" tooltip=\"{1}\"/>",
                        HypRel.Id, ((ExcelHyperLink)Hyperlink).ToolTip);
                else
                    xml.AppendFormat(
                        "<a:hlinkClick xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:id=\"{0}\" />",
                        HypRel.Id);
            }

            xml.Append("</xdr:cNvPr>");
        }

        xml.Append(
            "<xdr:cNvPicPr><a:picLocks noChangeAspect=\"1\" /></xdr:cNvPicPr></xdr:nvPicPr><xdr:blipFill><a:blip xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:embed=\"\" cstate=\"print\" /><a:stretch><a:fillRect /> </a:stretch> </xdr:blipFill> <xdr:spPr> <a:xfrm> <a:off x=\"0\" y=\"0\" />  <a:ext cx=\"0\" cy=\"0\" /> </a:xfrm> <a:prstGeom prst=\"rect\"> <a:avLst /> </a:prstGeom> </xdr:spPr>");

        return xml.ToString();
    }

    /// <summary>
    ///     Set the size of the image in percent from the orginal size
    ///     Note that resizing columns / rows after using this function will effect the size of the picture
    /// </summary>
    /// <param name="Percent">Percent</param>
    public override void SetSize(int Percent)
    {
        if (Image == null)
        {
            base.SetSize(Percent);
        }
        else
        {
            _width = Image.Width;
            _height = Image.Height;

            _width = (int)(_width * ((decimal)Percent / 100));
            _height = (int)(_height * ((decimal)Percent / 100));

            SetPixelWidth(_width, Image.HorizontalResolution);
            SetPixelHeight(_height, Image.VerticalResolution);
        }
    }

    internal override void DeleteMe()
    {
        _drawings._package.RemoveImage(ImageHash);
        base.DeleteMe();
    }

    public override void Dispose()
    {
        base.Dispose();
        Hyperlink = null;
        _image.Dispose();
        _image = null;
    }

    #region Constructors

    internal ExcelPicture(ExcelDrawings drawings, XmlNode node) :
        base(drawings, node, "xdr:pic/xdr:nvPicPr/xdr:cNvPr/@name")
    {
        var picNode = node.SelectSingleNode("xdr:pic/xdr:blipFill/a:blip", drawings.NameSpaceManager);
        if (picNode != null)
        {
            RelPic = drawings.Part.GetRelationship(picNode.Attributes["r:embed"].Value);
            UriPic = UriHelper.ResolvePartUri(drawings.UriDrawing, RelPic.TargetUri);

            Part = drawings.Part.Package.GetPart(UriPic);
            var f = new FileInfo(UriPic.OriginalString);
            ContentType = GetContentType(f.Extension);
            _image = Image.Load(Part.GetStream());

            var iby = ImageCompat.GetImageAsByteArray(_image);
            var ii = _drawings._package.LoadImage(iby, UriPic, Part);
            ImageHash = ii.Hash;

            //_height = _image.Height;
            //_width = _image.Width;
            var relID = GetXmlNodeString("xdr:pic/xdr:nvPicPr/xdr:cNvPr/a:hlinkClick/@r:id");
            if (!string.IsNullOrEmpty(relID))
            {
                HypRel = drawings.Part.GetRelationship(relID);
                if (HypRel.TargetUri.IsAbsoluteUri)
                    Hyperlink = new ExcelHyperLink(HypRel.TargetUri.AbsoluteUri);
                else
                    Hyperlink = new ExcelHyperLink(HypRel.TargetUri.OriginalString, UriKind.Relative);
                ((ExcelHyperLink)Hyperlink).ToolTip =
                    GetXmlNodeString("xdr:pic/xdr:nvPicPr/xdr:cNvPr/a:hlinkClick/@tooltip");
            }
        }
    }

    internal ExcelPicture(ExcelDrawings drawings, XmlNode node, Image image, Uri hyperlink) :
        base(drawings, node, "xdr:pic/xdr:nvPicPr/xdr:cNvPr/@name")
    {
        var picNode = node.OwnerDocument.CreateElement("xdr", "pic", ExcelPackage.SchemaSheetDrawings);
        node.InsertAfter(picNode, node.SelectSingleNode("xdr:to", NameSpaceManager));
        Hyperlink = hyperlink;
        picNode.InnerXml = PicStartXml();

        node.InsertAfter(node.OwnerDocument.CreateElement("xdr", "clientData", ExcelPackage.SchemaSheetDrawings),
            picNode);

        var package = drawings.Worksheet._package.Package;
        //Get the picture if it exists or save it if not.
        _image = image;
        var relID = SavePicture(image);

        //Create relationship
        node.SelectSingleNode("xdr:pic/xdr:blipFill/a:blip/@r:embed", NameSpaceManager).Value = relID;
        _height = image.Height;
        _width = image.Width;
        SetPosDefaults(image);
        package.Flush();
    }

    internal ExcelPicture(ExcelDrawings drawings, XmlNode node, FileInfo imageFile, Uri hyperlink) :
        base(drawings, node, "xdr:pic/xdr:nvPicPr/xdr:cNvPr/@name")
    {
        var picNode = node.OwnerDocument.CreateElement("xdr", "pic", ExcelPackage.SchemaSheetDrawings);
        node.InsertAfter(picNode, node.SelectSingleNode("xdr:to", NameSpaceManager));
        Hyperlink = hyperlink;
        picNode.InnerXml = PicStartXml();

        node.InsertAfter(node.OwnerDocument.CreateElement("xdr", "clientData", ExcelPackage.SchemaSheetDrawings),
            picNode);

        //Changed to stream 2/4-13 (issue 14834). Thnx SClause
        var package = drawings.Worksheet._package.Package;
        ContentType = GetContentType(imageFile.Extension);
        var imagestream = new FileStream(imageFile.FullName, FileMode.Open, FileAccess.Read);
        _image = Image.Load(imagestream);

        var img = ImageCompat.GetImageAsByteArray(_image);

        imagestream.Close();
        UriPic = GetNewUri(package, "/xl/media/{0}" + imageFile.Name);
        var ii = _drawings._package.AddImage(img, UriPic, ContentType);
        string relID;
        if (!drawings._hashes.ContainsKey(ii.Hash))
        {
            Part = ii.Part;
            RelPic = drawings.Part.CreateRelationship(UriHelper.GetRelativeUri(drawings.UriDrawing, ii.Uri),
                TargetMode.Internal, ExcelPackage.SchemaRelationships + "/image");
            relID = RelPic.Id;
            _drawings._hashes.Add(ii.Hash, relID);
            AddNewPicture(img, relID);
        }
        else
        {
            relID = drawings._hashes[ii.Hash];
            var rel = _drawings.Part.GetRelationship(relID);
            UriPic = UriHelper.ResolvePartUri(rel.SourceUri, rel.TargetUri);
        }

        ImageHash = ii.Hash;
        _height = Image.Height;
        _width = Image.Width;
        SetPosDefaults(Image);
        //Create relationship
        node.SelectSingleNode("xdr:pic/xdr:blipFill/a:blip/@r:embed", NameSpaceManager).Value = relID;
        package.Flush();
    }

    internal static string GetContentType(string extension)
    {
        switch (extension.ToLower(CultureInfo.InvariantCulture))
        {
            // ReSharper disable RedundantCaseLabel
            case BmpFormat.FormatDefaultExtension:
                return BmpFormat.FormatMimeType;

            case GifFormat.FormatDefaultExtension:
                return GifFormat.FormatMimeType;

            case PngFormat.FormatDefaultExtension:
                return PngFormat.FormatMimeType;

            case ".cgm":
                return "image/cgm";

            case ".emf":
                return "image/x-emf";

            case ".eps":
                return "image/x-eps";

            case ".pcx":
                return "image/x-pcx";

            case TgaFormat.FormatDefaultExtension:
                return TgaFormat.FormatMimeType;

            case TiffFormat.FormatDefaultExtension:
            case TiffFormat.FormatAltDefaultExtension:
                return TiffFormat.FormatMimeType;

            case ".wmf":
                return "image/x-wmf";

            case JpegFormat.FormatDefaultExtension:
            case JpegFormat.FormatAltDefaultExtension:
            default:
                return JpegFormat.FormatMimeType;
            // ReSharper restore RedundantCaseLabel
        }
    }

    internal static Formats.IImageEncoder GetImageEncoder(Formats.IImageFormat format)
    {
        if (format == null) throw new ArgumentNullException(nameof(format));

        switch (format.Name)
        {
            case BmpFormat.FormatName:
                return new BmpEncoder();

            case GifFormat.FormatName:
                return new GifEncoder();

            case JpegFormat.FormatName:
                return new JpegEncoder();

            case PngFormat.FormatName:
                return new PngEncoder();

            case TgaFormat.FormatName:
                return new TgaEncoder();

            default:
                throw new NotSupportedException(
                    $"Image format '{format.Name}' cannot be processed by this application.");
        }
    }

    internal static Formats.IImageEncoder GetImageEncoder(string contentType)
    {
        return GetImageEncoder(GetImageFormat(contentType));
    }

    internal static Formats.IImageFormat GetImageFormat(string contentType)
    {
        switch (contentType.ToLower(CultureInfo.InvariantCulture))
        {
            case BmpFormat.FormatMimeType:
                return BmpFormat.Instance;

            case JpegFormat.FormatMimeType:
                return JpegFormat.Instance;

            case GifFormat.FormatMimeType:
                return GifFormat.Instance;

            case PngFormat.FormatMimeType:
                return PngFormat.Instance;

            case PbmFormat.FormatMimeType:
            case TgaFormat.FormatMimeType:
            case TgaFormat.FormatAltMimeType:
            case TiffFormat.FormatMimeType:
            case TiffFormat.FormatAltMimeType:
            case WebpFormat.FormatMimeType:
            case "image/x-emf":
            case "image/x-wmf":
                throw new NotSupportedException(
                    $"Image format '{contentType.ToLower(CultureInfo.InvariantCulture)}' cannot be processed by this application.");

            default:
                return JpegFormat.Instance;
        }
    } //Add a new image to the compare collection

    private void AddNewPicture(byte[] img, string relID)
    {
        var newPic = new ExcelDrawings.ImageCompare();
        newPic.image = img;
        newPic.relID = relID;
        //_drawings._pics.Add(newPic);
    }

    #endregion
}