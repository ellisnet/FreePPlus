// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Processing.Processors.Dithering;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

/// <summary>
///     Allows the consumption a palette to dither an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal sealed class PaletteDitherProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly IDither dither;
    private readonly DitherProcessor ditherProcessor;
    private bool isDisposed;
    private IMemoryOwner<TPixel> paletteOwner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PaletteDitherProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="PaletteDitherProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public PaletteDitherProcessor(Configuration configuration, PaletteDitherProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        dither = definition.Dither;

        var sourcePalette = definition.Palette.Span;
        paletteOwner = Configuration.MemoryAllocator.Allocate<TPixel>(sourcePalette.Length);

        Color.ToPixel(Configuration, sourcePalette, paletteOwner.Memory.Span);

        ditherProcessor = new DitherProcessor(
            Configuration,
            paletteOwner.Memory,
            definition.DitherScale);
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());
        dither.ApplyPaletteDither(in ditherProcessor, source, interest);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (isDisposed) return;

        isDisposed = true;
        if (disposing) paletteOwner.Dispose();

        paletteOwner = null;
        base.Dispose(disposing);
    }

    /// <summary>
    ///     Used to allow inlining of calls to
    ///     <see cref="IPaletteDitherImageProcessor{TPixel}.GetPaletteColor(TPixel)" />.
    /// </summary>
    private readonly struct DitherProcessor : IPaletteDitherImageProcessor<TPixel>
    {
        private readonly EuclideanPixelMap<TPixel> pixelMap;

        [MethodImpl(InliningOptions.ShortMethod)]
        public DitherProcessor(
            Configuration configuration,
            ReadOnlyMemory<TPixel> palette,
            float ditherScale)
        {
            Configuration = configuration;
            pixelMap = new EuclideanPixelMap<TPixel>(configuration, palette);
            Palette = palette;
            DitherScale = ditherScale;
        }

        public Configuration Configuration { get; }

        public ReadOnlyMemory<TPixel> Palette { get; }

        public float DitherScale { get; }

        [MethodImpl(InliningOptions.ShortMethod)]
        public TPixel GetPaletteColor(TPixel color)
        {
            pixelMap.GetClosestColor(color, out var match);
            return match;
        }
    }
}