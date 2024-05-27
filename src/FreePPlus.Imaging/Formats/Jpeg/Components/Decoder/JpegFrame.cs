﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Represent a single jpeg frame
/// </summary>
internal sealed class JpegFrame : IDisposable
{
    /// <summary>
    ///     Gets or sets a value indicating whether the frame uses the extended specification.
    /// </summary>
    public bool Extended { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the frame uses the progressive specification.
    /// </summary>
    public bool Progressive { get; set; }

    /// <summary>
    ///     Gets or sets the precision.
    /// </summary>
    public byte Precision { get; set; }

    /// <summary>
    ///     Gets or sets the number of scanlines within the frame.
    /// </summary>
    public short Scanlines { get; set; }

    /// <summary>
    ///     Gets or sets the number of samples per scanline.
    /// </summary>
    public short SamplesPerLine { get; set; }

    /// <summary>
    ///     Gets or sets the number of components within a frame. In progressive frames this value can range from only 1 to 4.
    /// </summary>
    public byte ComponentCount { get; set; }

    /// <summary>
    ///     Gets or sets the component id collection.
    /// </summary>
    public byte[] ComponentIds { get; set; }

    /// <summary>
    ///     Gets or sets the order in which to process the components.
    ///     in interleaved mode.
    /// </summary>
    public byte[] ComponentOrder { get; set; }

    /// <summary>
    ///     Gets or sets the frame component collection.
    /// </summary>
    public JpegComponent[] Components { get; set; }

    /// <summary>
    ///     Gets or sets the maximum horizontal sampling factor.
    /// </summary>
    public int MaxHorizontalFactor { get; set; }

    /// <summary>
    ///     Gets or sets the maximum vertical sampling factor.
    /// </summary>
    public int MaxVerticalFactor { get; set; }

    /// <summary>
    ///     Gets or sets the number of MCU's per line.
    /// </summary>
    public int McusPerLine { get; set; }

    /// <summary>
    ///     Gets or sets the number of MCU's per column.
    /// </summary>
    public int McusPerColumn { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Components != null)
        {
            for (var i = 0; i < Components.Length; i++) Components[i]?.Dispose();

            Components = null;
        }
    }

    /// <summary>
    ///     Allocates the frame component blocks.
    /// </summary>
    public void InitComponents()
    {
        McusPerLine = (int)MathF.Ceiling(SamplesPerLine / 8F / MaxHorizontalFactor);
        McusPerColumn = (int)MathF.Ceiling(Scanlines / 8F / MaxVerticalFactor);

        for (var i = 0; i < ComponentCount; i++)
        {
            var component = Components[i];
            component.Init();
        }
    }
}