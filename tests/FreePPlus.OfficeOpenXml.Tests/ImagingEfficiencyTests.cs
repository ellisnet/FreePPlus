using System;
using System.IO;
using CodeBrix.Imaging;
using CodeBrix.Imaging.Formats.Jpeg;
using CodeBrix.Imaging.Formats.Png;
using CodeBrix.Imaging.PixelFormats;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Tests for all code paths that use CodeBrix.Imaging for image I/O.
///     These cover the ExcelPicture constructors (Image, FileInfo, package-load),
///     ExcelHeaderFooter picture insertion, and ExcelBackgroundImage — ensuring
///     the imaging efficiency changes do not break consumer-visible behavior.
/// </summary>
public class ImagingEfficiencyTests
{
    /// <summary>
    ///     Creates a temporary JPEG image file and returns its path.
    ///     Caller is responsible for deleting the file.
    /// </summary>
    private static string CreateTempJpegFile(int width = 80, int height = 40)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
        using var image = new Image<Rgba32>(width, height, JpegFormat.Instance);
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        image.Save(fs, new JpegEncoder());
        return path;
    }

    /// <summary>
    ///     Creates a temporary PNG image file and returns its path.
    ///     Caller is responsible for deleting the file.
    /// </summary>
    private static string CreateTempPngFile(int width = 80, int height = 40)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
        using var image = new Image<Rgba32>(width, height, PngFormat.Instance);
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        image.Save(fs, new PngEncoder());
        return path;
    }

    #region ExcelPicture — FileInfo constructor

    [Fact]
    public void AddPictureFromFileInfo_CreatesDrawing()
    {
        var tempFile = CreateTempJpegFile();
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var pic = ws.Drawings.AddPicture("Pic1", new FileInfo(tempFile));

            Assert.NotNull(pic);
            Assert.Equal("Pic1", pic.Name);
            Assert.Equal(1, ws.Drawings.Count);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddPictureFromFileInfo_PreservesImageDimensions()
    {
        var tempFile = CreateTempJpegFile(120, 60);
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var pic = ws.Drawings.AddPicture("Pic1", new FileInfo(tempFile));

            Assert.Equal(120, pic.Image.Width);
            Assert.Equal(60, pic.Image.Height);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddPictureFromFileInfo_PngFormat_Works()
    {
        var tempFile = CreateTempPngFile(100, 50);
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var pic = ws.Drawings.AddPicture("PngPic", new FileInfo(tempFile));

            Assert.NotNull(pic);
            Assert.Equal(100, pic.Image.Width);
            Assert.Equal(50, pic.Image.Height);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddPictureFromFileInfo_RoundTrips_WithImageData()
    {
        var tempFile = CreateTempJpegFile(90, 45);
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("PicSheet");
                ws.Drawings.AddPicture("RtPic", new FileInfo(tempFile));
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["PicSheet"];
                Assert.Equal(1, ws.Drawings.Count);

                var pic = (ExcelPicture)ws.Drawings[0];
                Assert.Equal("RtPic", pic.Name);
                Assert.NotNull(pic.Image);
                // Dimensions should survive the round-trip
                Assert.Equal(90, pic.Image.Width);
                Assert.Equal(45, pic.Image.Height);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddPictureFromFileInfo_DuplicateImage_SharesHash()
    {
        var tempFile = CreateTempJpegFile();
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var pic1 = ws.Drawings.AddPicture("Pic1", new FileInfo(tempFile));
            var pic2 = ws.Drawings.AddPicture("Pic2", new FileInfo(tempFile));

            Assert.Equal(2, ws.Drawings.Count);
            // Same image file should produce the same internal hash
            Assert.Equal(pic1.ImageHash, pic2.ImageHash);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    #endregion

    #region ExcelPicture — Image constructor

    [Fact]
    public void AddPictureFromImage_CreatesDrawing()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(100, 50);

        var pic = ws.Drawings.AddPicture("Pic1", image);

        Assert.NotNull(pic);
        Assert.Equal("Pic1", pic.Name);
        Assert.Equal(1, ws.Drawings.Count);
    }

    [Fact]
    public void AddPictureFromImage_RoundTrips_WithImageData()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("ImgSheet");
            using var image = ExcelPicture.CreateImage(75, 40);
            ws.Drawings.AddPicture("ImgPic", image);
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["ImgSheet"];
            Assert.Equal(1, ws.Drawings.Count);

            var pic = (ExcelPicture)ws.Drawings[0];
            Assert.Equal("ImgPic", pic.Name);
            Assert.NotNull(pic.Image);
            Assert.Equal(75, pic.Image.Width);
            Assert.Equal(40, pic.Image.Height);
        }
    }

    #endregion

    #region ExcelPicture — Package constructor (loading from existing package)

    [Fact]
    public void PictureLoadedFromPackage_HasCorrectProperties()
    {
        var tempFile = CreateTempPngFile(200, 100);
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Drawings.AddPicture("LoadTest", new FileInfo(tempFile));
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["Sheet1"];
                var pic = (ExcelPicture)ws.Drawings[0];

                Assert.Equal("LoadTest", pic.Name);
                Assert.NotNull(pic.Image);
                Assert.NotNull(pic.Part);
                Assert.False(string.IsNullOrEmpty(pic.ImageHash));
                Assert.Equal(200, pic.Image.Width);
                Assert.Equal(100, pic.Image.Height);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void PictureLoadedFromPackage_MultiplePictures_AllIntact()
    {
        var tempFile1 = CreateTempJpegFile(60, 30);
        // ReSharper disable RedundantArgumentDefaultValue
        var tempFile2 = CreateTempPngFile(80, 40);
        // ReSharper restore RedundantArgumentDefaultValue
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Drawings.AddPicture("Jpeg1", new FileInfo(tempFile1));
                ws.Drawings.AddPicture("Png1", new FileInfo(tempFile2));
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["Sheet1"];
                Assert.Equal(2, ws.Drawings.Count);

                var pic1 = (ExcelPicture)ws.Drawings["Jpeg1"];
                var pic2 = (ExcelPicture)ws.Drawings["Png1"];

                Assert.NotNull(pic1.Image);
                Assert.NotNull(pic2.Image);
                Assert.Equal(60, pic1.Image.Width);
                Assert.Equal(80, pic2.Image.Width);
            }
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    [Fact]
    public void PictureLoadedFromPackage_DoubleRoundTrip_Survives()
    {
        var tempFile = CreateTempJpegFile(50, 25);
        try
        {
            // Round-trip 1: create → save
            using var ms1 = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Drawings.AddPicture("DblRt", new FileInfo(tempFile));
                pck.SaveAs(ms1);
            }

            // Round-trip 2: load → save again
            ms1.Position = 0;
            using var ms2 = new MemoryStream();
            using (var pck = new ExcelPackage(ms1))
            {
                pck.SaveAs(ms2);
            }

            // Verify after double round-trip
            ms2.Position = 0;
            using (var pck = new ExcelPackage(ms2))
            {
                var ws = pck.Workbook.Worksheets["Sheet1"];
                Assert.Equal(1, ws.Drawings.Count);

                var pic = (ExcelPicture)ws.Drawings[0];
                Assert.Equal("DblRt", pic.Name);
                Assert.NotNull(pic.Image);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    #endregion

    #region ExcelPicture — Image property setter

    [Fact]
    public void PictureImageProperty_CanBeReplaced()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image1 = ExcelPicture.CreateImage(100, 50);

        var pic = ws.Drawings.AddPicture("Pic1", image1);
        Assert.Equal(100, pic.Image.Width);

        // Replace the image
        using var image2 = ExcelPicture.CreateImage(200, 100);
        pic.Image = image2;

        Assert.Equal(200, pic.Image.Width);
        Assert.Equal(100, pic.Image.Height);
    }

    #endregion

    #region ExcelHeaderFooter — InsertPicture(Image)

    [Fact]
    public void HeaderFooter_InsertPictureFromImage_Works()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(100, 50);

        var vmlPic = ws.HeaderFooter.OddHeader.InsertPicture(image, PictureAlignment.Centered);

        Assert.NotNull(vmlPic);
        Assert.True(ws.HeaderFooter.Pictures.Count > 0);
    }

    [Fact]
    public void HeaderFooter_InsertPictureFromImage_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("HFSheet");
            using var image = ExcelPicture.CreateImage(80, 40);
            ws.HeaderFooter.OddHeader.InsertPicture(image, PictureAlignment.Left);
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["HFSheet"];
            Assert.True(ws.HeaderFooter.Pictures.Count > 0);
        }
    }

    [Fact]
    public void HeaderFooter_InsertPictureFromImage_AppendsPlaceholder()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.HeaderFooter.OddHeader.RightAlignedText = "Report";
        using var image = ExcelPicture.CreateImage(80, 40);

        ws.HeaderFooter.OddHeader.InsertPicture(image, PictureAlignment.Right);

        // The &G placeholder should be appended to the text
        Assert.Contains("&G", ws.HeaderFooter.OddHeader.RightAlignedText);
    }

    #endregion

    #region ExcelHeaderFooter — InsertPicture(FileInfo)

    [Fact]
    public void HeaderFooter_InsertPictureFromFileInfo_Works()
    {
        var tempFile = CreateTempJpegFile(60, 30);
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var vmlPic = ws.HeaderFooter.OddHeader.InsertPicture(
                new FileInfo(tempFile), PictureAlignment.Centered);

            Assert.NotNull(vmlPic);
            Assert.True(ws.HeaderFooter.Pictures.Count > 0);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void HeaderFooter_InsertPictureFromFileInfo_RoundTrips()
    {
        var tempFile = CreateTempJpegFile(60, 30);
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("HFSheet");
                ws.HeaderFooter.OddFooter.InsertPicture(
                    new FileInfo(tempFile), PictureAlignment.Left);
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["HFSheet"];
                Assert.True(ws.HeaderFooter.Pictures.Count > 0);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void HeaderFooter_InsertPictureFromFileInfo_ThrowsForMissingFile()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var bogusFile = new FileInfo(Path.Combine(Path.GetTempPath(), "nonexistent_12345.jpg"));

        Assert.Throws<InvalidDataException>(() =>
            ws.HeaderFooter.OddHeader.InsertPicture(bogusFile, PictureAlignment.Left));
    }

    [Fact]
    public void HeaderFooter_InsertPictureFromFileInfo_ThrowsForInvalidImage()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
        try
        {
            // Write non-image data
            File.WriteAllText(tempFile, "This is not an image file.");

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            Assert.Throws<InvalidDataException>(() =>
                ws.HeaderFooter.OddHeader.InsertPicture(new FileInfo(tempFile), PictureAlignment.Left));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void HeaderFooter_InsertPicture_DuplicateAlignmentThrows()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(80, 40);

        ws.HeaderFooter.OddHeader.InsertPicture(image, PictureAlignment.Left);

        Assert.Throws<InvalidOperationException>(() =>
            ws.HeaderFooter.OddHeader.InsertPicture(image, PictureAlignment.Left));
    }

    #endregion

    #region ExcelBackgroundImage — Image property setter

    [Fact]
    public void BackgroundImage_SetFromImageProperty_Works()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(100, 50);

        ws.BackgroundImage.Image = image;

        Assert.NotNull(ws.BackgroundImage.Image);
    }

    [Fact]
    public void BackgroundImage_SetFromImageProperty_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("BgSheet");
            using var image = ExcelPicture.CreateImage(100, 50);
            ws.BackgroundImage.Image = image;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["BgSheet"];
            var bgImage = ws.BackgroundImage.Image;
            Assert.NotNull(bgImage);
        }
    }

    [Fact]
    public void BackgroundImage_SetToNull_ClearsImage()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image = ExcelPicture.CreateImage(100, 50);

        ws.BackgroundImage.Image = image;
        Assert.NotNull(ws.BackgroundImage.Image);

        ws.BackgroundImage.Image = null;
        Assert.Null(ws.BackgroundImage.Image);
    }

    [Fact]
    public void BackgroundImage_ReplaceImage_Works()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        using var image1 = ExcelPicture.CreateImage(100, 50);
        ws.BackgroundImage.Image = image1;

        using var image2 = ExcelPicture.CreateImage(200, 100);
        ws.BackgroundImage.Image = image2;

        var bgImage = ws.BackgroundImage.Image;
        Assert.NotNull(bgImage);
    }

    #endregion

    #region ExcelBackgroundImage — SetFromFile

    [Fact]
    public void BackgroundImage_SetFromFile_Works()
    {
        var tempFile = CreateTempJpegFile(100, 50);
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            ws.BackgroundImage.SetFromFile(new FileInfo(tempFile));

            Assert.NotNull(ws.BackgroundImage.Image);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void BackgroundImage_SetFromFile_PngFormat_Works()
    {
        var tempFile = CreateTempPngFile(100, 50);
        try
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            ws.BackgroundImage.SetFromFile(new FileInfo(tempFile));

            Assert.NotNull(ws.BackgroundImage.Image);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void BackgroundImage_SetFromFile_RoundTrips()
    {
        var tempFile = CreateTempJpegFile(100, 50);
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("BgSheet");
                ws.BackgroundImage.SetFromFile(new FileInfo(tempFile));
                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["BgSheet"];
                Assert.NotNull(ws.BackgroundImage.Image);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void BackgroundImage_SetFromFile_ThrowsForInvalidFile()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
        try
        {
            File.WriteAllText(tempFile, "Not an image.");

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            Assert.Throws<InvalidDataException>(() =>
                ws.BackgroundImage.SetFromFile(new FileInfo(tempFile)));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    #endregion

    #region ImageCompat — exercised indirectly via Image property setter

    /// <summary>
    ///     The Image property setter calls SavePicture → ImageCompat.GetImageAsByteArray.
    ///     Setting the Image property multiple times exercises the internal byte conversion.
    /// </summary>
    [Fact]
    public void ImagePropertySetter_InternalByteConversion_Works()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        using var image1 = ExcelPicture.CreateImage(50, 25);
        var pic = ws.Drawings.AddPicture("Pic1", image1);

        // Replace image — exercises ImageCompat.GetImageAsByteArray internally
        using var image2 = ExcelPicture.CreateImage(70, 35);
        pic.Image = image2;

        Assert.Equal(70, pic.Image.Width);
        Assert.Equal(35, pic.Image.Height);
    }

    /// <summary>
    ///     Replacing an image and saving verifies the internal byte conversion
    ///     produces valid data that can be stored in the package.
    /// </summary>
    [Fact]
    public void ImagePropertySetter_ReplacedImage_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            using var image1 = ExcelPicture.CreateImage(50, 25);
            var pic = ws.Drawings.AddPicture("Pic1", image1);

            using var image2 = ExcelPicture.CreateImage(150, 75);
            pic.Image = image2;

            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            var pic = (ExcelPicture)ws.Drawings[0];
            Assert.NotNull(pic.Image);
            Assert.Equal(150, pic.Image.Width);
            Assert.Equal(75, pic.Image.Height);
        }
    }

    #endregion

    #region Cross-feature: Pictures + Background on same worksheet

    [Fact]
    public void PictureAndBackgroundImage_CanCoexist()
    {
        // ReSharper disable RedundantArgumentDefaultValue
        var tempFile = CreateTempJpegFile(80, 40);
        // ReSharper restore RedundantArgumentDefaultValue
        try
        {
            using var ms = new MemoryStream();
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Sheet1");

                // Add a drawing picture
                ws.Drawings.AddPicture("DrawingPic", new FileInfo(tempFile));

                // Also set a background image
                using var bgImage = ExcelPicture.CreateImage(200, 100);
                ws.BackgroundImage.Image = bgImage;

                pck.SaveAs(ms);
            }

            ms.Position = 0;
            using (var pck = new ExcelPackage(ms))
            {
                var ws = pck.Workbook.Worksheets["Sheet1"];
                Assert.Equal(1, ws.Drawings.Count);
                Assert.NotNull(ws.BackgroundImage.Image);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    #endregion
}
