// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Options for influencing the drawing functions.
/// </summary>
public class GraphicsOptions : IDeepCloneable<GraphicsOptions>
{
    private int antialiasSubpixelDepth = 16;
    private float blendPercentage = 1F;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GraphicsOptions" /> class.
    /// </summary>
    public GraphicsOptions() { }

    private GraphicsOptions(GraphicsOptions source)
    {
        AlphaCompositionMode = source.AlphaCompositionMode;
        Antialias = source.Antialias;
        AntialiasSubpixelDepth = source.AntialiasSubpixelDepth;
        BlendPercentage = source.BlendPercentage;
        ColorBlendingMode = source.ColorBlendingMode;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether antialiasing should be applied.
    ///     Defaults to true.
    /// </summary>
    public bool Antialias { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating the number of subpixels to use while rendering with antialiasing enabled.
    ///     Defaults to 16.
    /// </summary>
    public int AntialiasSubpixelDepth
    {
        get => antialiasSubpixelDepth;

        set
        {
            Guard.MustBeGreaterThanOrEqualTo(value, 0, nameof(AntialiasSubpixelDepth));
            antialiasSubpixelDepth = value;
        }
    }

    /// <summary>
    ///     Gets or sets a value between indicating the blending percentage to apply to the drawing operation.
    ///     Range 0..1; Defaults to 1.
    /// </summary>
    public float BlendPercentage
    {
        get => blendPercentage;

        set
        {
            Guard.MustBeBetweenOrEqualTo(value, 0, 1F, nameof(BlendPercentage));
            blendPercentage = value;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating the color blending mode to apply to the drawing operation.
    ///     Defaults to <see cref="PixelColorBlendingMode.Normal" />.
    /// </summary>
    public PixelColorBlendingMode ColorBlendingMode { get; set; } = PixelColorBlendingMode.Normal;

    /// <summary>
    ///     Gets or sets a value indicating the alpha composition mode to apply to the drawing operation
    ///     Defaults to <see cref="PixelAlphaCompositionMode.SrcOver" />.
    /// </summary>
    public PixelAlphaCompositionMode AlphaCompositionMode { get; set; } = PixelAlphaCompositionMode.SrcOver;

    /// <inheritdoc />
    public GraphicsOptions DeepClone()
    {
        return new GraphicsOptions(this);
    }
}