using CodeBrix.Imaging;
using CodeBrix.Imaging.Formats;
using CodeBrix.Imaging.Formats.Png;

namespace OfficeOpenXml.Compatibility;

internal class ImageCompat
{
    /// <summary>
    ///     Encodes an image to a byte array.
    /// </summary>
    /// <param name="image">The image to encode, which may be null.</param>
    /// <returns>The encoded bytes, or null when <paramref name="image" /> is null.</returns>
    /// <remarks>
    ///     An image only carries an expected format when it was loaded from encoded bytes or previously saved.
    ///     One the caller constructed in memory reports <see cref="UnknownImageFormat" />, which cannot be
    ///     encoded, so PNG is used instead - it is lossless, alpha-capable, and accepted by Excel everywhere
    ///     a picture or background image can appear.
    /// </remarks>
    internal static byte[] GetImageAsByteArray(Image image) =>
        image?.ToByteArray(image.Format is UnknownImageFormat or null
            ? PngFormat.Instance
            : image.Format);
}
