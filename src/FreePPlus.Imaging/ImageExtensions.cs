// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Text;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for the <see cref="Image" /> type.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    ///     Writes the image to the given stream using the currently loaded image format.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="path">The file path to save the image to.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    public static void Save(this Image source, string path)
    {
        Guard.NotNull(path, nameof(path));

        var ext = Path.GetExtension(path);
        var format = source.GetConfiguration().ImageFormatsManager.FindFormatByFileExtension(ext);
        if (format is null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"No encoder was found for extension '{ext}'. Registered encoders include:");
            foreach (var fmt in source.GetConfiguration().ImageFormats)
                sb.AppendFormat(" - {0} : {1}{2}", fmt.Name, string.Join(", ", fmt.FileExtensions),
                    Environment.NewLine);

            throw new NotSupportedException(sb.ToString());
        }

        var encoder = source.GetConfiguration().ImageFormatsManager.FindEncoder(format);

        if (encoder is null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"No encoder was found for extension '{ext}' using image format '{format.Name}'. Registered encoders include:");
            foreach (var enc in source.GetConfiguration().ImageFormatsManager.ImageEncoders)
                sb.AppendFormat(" - {0} : {1}{2}", enc.Key, enc.Value.GetType().Name, Environment.NewLine);

            throw new NotSupportedException(sb.ToString());
        }

        source.Save(path, encoder);
    }

    /// <summary>
    ///     Writes the image to the given stream using the currently loaded image format.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="path">The file path to save the image to.</param>
    /// <param name="encoder">The encoder to save the image with.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="ArgumentNullException">The encoder is null.</exception>
    public static void Save(this Image source, string path, IImageEncoder encoder)
    {
        Guard.NotNull(path, nameof(path));
        Guard.NotNull(encoder, nameof(encoder));
        using (var fs = source.GetConfiguration().FileSystem.Create(path))
        {
            source.Save(fs, encoder);
        }
    }

    /// <summary>
    ///     Writes the image to the given stream using the currently loaded image format.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <param name="format">The format to save the image in.</param>
    /// <exception cref="ArgumentNullException">The stream is null.</exception>
    /// <exception cref="ArgumentNullException">The format is null.</exception>
    /// <exception cref="NotSupportedException">The stream is not writable.</exception>
    /// <exception cref="NotSupportedException">No encoder available for provided format.</exception>
    public static void Save(this Image source, Stream stream, IImageFormat format)
    {
        Guard.NotNull(stream, nameof(stream));
        Guard.NotNull(format, nameof(format));

        if (!stream.CanWrite) throw new NotSupportedException("Cannot write to the stream.");

        var encoder = source.GetConfiguration().ImageFormatsManager.FindEncoder(format);

        if (encoder is null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("No encoder was found for the provided mime type. Registered encoders include:");

            foreach (var val in source.GetConfiguration().ImageFormatsManager.ImageEncoders)
                sb.AppendFormat(" - {0} : {1}{2}", val.Key.Name, val.Value.GetType().Name, Environment.NewLine);

            throw new NotSupportedException(sb.ToString());
        }

        source.Save(stream, encoder);
    }

    /// <summary>
    ///     Returns a Base64 encoded string from the given image.
    ///     The result is prepended with a Data URI <see href="https://en.wikipedia.org/wiki/Data_URI_scheme" />
    ///     <para>
    ///         <example>
    ///             For example:
    ///             <see href="data:image/gif;base64,R0lGODlhAQABAIABAEdJRgAAACwAAAAAAQABAAACAkQBAA==" />
    ///         </example>
    ///     </para>
    /// </summary>
    /// <param name="source">The source image</param>
    /// <param name="format">The format.</param>
    /// <exception cref="ArgumentNullException">The format is null.</exception>
    /// <returns>The <see cref="string" /></returns>
    public static string ToBase64String(this Image source, IImageFormat format)
    {
        var bytes = source.ToByteArray(format);
        return $"data:{format.DefaultMimeType};base64,{Convert.ToBase64String(bytes, 0, bytes.Length)}";
    }

    public static byte[] ToByteArray(this Image source, IImageFormat format)
    {
        Guard.NotNull(format, nameof(format));

        using var stream = new MemoryStream();
        source.Save(stream, format);

        // Always available.
        stream.TryGetBuffer(out var buffer);

        return buffer.ToArray();
    }
}