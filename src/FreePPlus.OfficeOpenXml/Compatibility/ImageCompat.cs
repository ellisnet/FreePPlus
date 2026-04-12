using CodeBrix.Imaging;

namespace OfficeOpenXml.Compatibility;

internal class ImageCompat
{
    internal static byte[] GetImageAsByteArray(Image image) => image?.ToByteArray(image.Format);
}
