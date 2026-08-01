using CodeBrix.Imaging;
using CodeBrix.Imaging.Formats;
using CodeBrix.Imaging.Formats.Jpeg;
using CodeBrix.Imaging.Formats.Png;
using CodeBrix.Imaging.PixelFormats;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Encoding of images the caller constructed in memory.
/// </summary>
/// <remarks>
///     An image only carries an expected format when it was loaded from encoded bytes or previously saved.
///     One constructed in memory reports UnknownImageFormat, and asking the encoder for that format threw
///     NotSupportedException - so AddPicture worked for an image loaded from a file and failed for the
///     otherwise identical image built in code. PNG is used as the fallback.
/// </remarks>
public class ImageFormatFallbackTests
{
    [Fact]
    public void ConstructedImageHasNoFormatOfItsOwn()
    {
        //Arrange & Act
        using var image = new Image<Rgba32>(10, 10);

        //Assert - this is the precondition the fallback exists for
        Assert.IsType<UnknownImageFormat>(image.Format);
    }

    [Fact]
    public void AddPictureAcceptsAnImageConstructedInMemory()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = new Image<Rgba32>(100, 100);

        //Act
        var picture = ws.Drawings.AddPicture("constructed", image);

        //Assert
        Assert.NotNull(picture);
        Assert.Equal(100, picture.Image.Width);
        Assert.Equal(100, picture.Image.Height);
    }

    [Fact]
    public void AddPictureWithHyperlinkAcceptsAnImageConstructedInMemory()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = new Image<Rgba32>(100, 100);

        //Act
        var picture = ws.Drawings.AddPicture("constructed", image, new Uri("https://example.com"));

        //Assert
        Assert.NotNull(picture);
    }

    [Fact]
    public void ConstructedPictureIsStoredAsPng()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = new Image<Rgba32>(100, 100);
        ws.Drawings.AddPicture("constructed", image);

        //Act
        var bytes = pck.GetAsByteArray();

        //Assert - PNG's eight-byte signature
        using var ms = new MemoryStream(bytes);
        using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
        var media = zip.Entries.Single(e => e.FullName.StartsWith("xl/media/", StringComparison.Ordinal));

        using var stream = media.Open();
        var signature = new byte[8];
        stream.ReadExactly(signature);

        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, signature);
    }

    [Fact]
    public void PictureWithAKnownFormatKeepsThatFormat()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(100, 100);
        Assert.IsType<JpegFormat>(image.Format);
        ws.Drawings.AddPicture("jpeg", image);

        //Act
        var bytes = pck.GetAsByteArray();

        //Assert - JPEG's SOI marker, so the fallback did not override a format the image already had
        using var ms = new MemoryStream(bytes);
        using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
        var media = zip.Entries.Single(e => e.FullName.StartsWith("xl/media/", StringComparison.Ordinal));

        using var stream = media.Open();
        var signature = new byte[2];
        stream.ReadExactly(signature);

        Assert.Equal(new byte[] { 0xFF, 0xD8 }, signature);
    }

    [Fact]
    public void BackgroundImageAcceptsAnImageConstructedInMemory()
    {
        //Arrange
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = new Image<Rgba32>(100, 100);

        //Act
        ws.BackgroundImage.Image = image;

        //Assert
        Assert.NotNull(ws.BackgroundImage.Image);
        Assert.Equal(100, ws.BackgroundImage.Image.Width);
    }

    [Fact]
    public void PngFormatIsTheDocumentedFallback()
    {
        //Arrange
        using var image = new Image<Rgba32>(10, 10);

        //Act - what ImageCompat does when the image has no format of its own
        var bytes = image.ToByteArray(PngFormat.Instance);

        //Assert
        Assert.NotEmpty(bytes);
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, bytes.Take(4).ToArray());
    }
}
