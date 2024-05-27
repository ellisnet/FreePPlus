// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

#pragma warning disable IDE0251

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     An implementation of Wu's color quantizer with alpha channel.
/// </summary>
/// <remarks>
///     <para>
///         Based on C Implementation of Xiaolin Wu's Color Quantizer (v. 2)
///         (see Graphics Gems volume II, pages 126-133)
///         (<see href="http://www.ece.mcmaster.ca/~xwu/cq.c" />).
///     </para>
///     <para>
///         This adaptation is based on the excellent JeremyAnsel.ColorQuant by Jérémy Ansel
///         <see href="https://github.com/JeremyAnsel/JeremyAnsel.ColorQuant" />
///     </para>
///     <para>
///         Algorithm: Greedy orthogonal bipartition of RGB space for variance minimization aided by inclusion-exclusion
///         tricks.
///         For speed no nearest neighbor search is done. Slightly better performance can be expected by more sophisticated
///         but more expensive versions.
///     </para>
/// </remarks>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal struct WuQuantizer<TPixel> : IQuantizer<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly MemoryAllocator memoryAllocator;

    // The following two variables determine the amount of bits to preserve when calculating the histogram.
    // Reducing the value of these numbers the granularity of the color maps produced, making it much faster
    // and using much less memory but potentially less accurate. Current results are very good though!

    /// <summary>
    ///     The index bits. 6 in original code.
    /// </summary>
    private const int IndexBits = 5;

    /// <summary>
    ///     The index alpha bits. 3 in original code.
    /// </summary>
    private const int IndexAlphaBits = 5;

    /// <summary>
    ///     The index count.
    /// </summary>
    private const int IndexCount = (1 << IndexBits) + 1;

    /// <summary>
    ///     The index alpha count.
    /// </summary>
    private const int IndexAlphaCount = (1 << IndexAlphaBits) + 1;

    /// <summary>
    ///     The table length. Now 1185921. originally 2471625.
    /// </summary>
    private const int TableLength = IndexCount * IndexCount * IndexCount * IndexAlphaCount;

    private IMemoryOwner<Moment> momentsOwner;
    private IMemoryOwner<byte> tagsOwner;
    private IMemoryOwner<TPixel> paletteOwner;
    private ReadOnlyMemory<TPixel> palette;
    private int maxColors;
    private readonly Box[] colorCube;
    private EuclideanPixelMap<TPixel> pixelMap;
    private readonly bool isDithering;
    private bool isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WuQuantizer{TPixel}" /> struct.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public WuQuantizer(Configuration configuration, QuantizerOptions options)
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(options, nameof(options));

        Configuration = configuration;
        Options = options;
        maxColors = Options.MaxColors;
        memoryAllocator = Configuration.MemoryAllocator;
        momentsOwner = memoryAllocator.Allocate<Moment>(TableLength, AllocationOptions.Clean);
        tagsOwner = memoryAllocator.Allocate<byte>(TableLength, AllocationOptions.Clean);
        paletteOwner = memoryAllocator.Allocate<TPixel>(maxColors, AllocationOptions.Clean);
        palette = default;
        colorCube = new Box[maxColors];
        isDisposed = false;
        pixelMap = default;
        isDithering = isDithering = Options.Dither is not null;
    }

    /// <inheritdoc />
    public Configuration Configuration { get; }

    /// <inheritdoc />
    public QuantizerOptions Options { get; }

    /// <inheritdoc />
    public ReadOnlyMemory<TPixel> Palette
    {
        get
        {
            QuantizerUtilities.CheckPaletteState(in palette);
            return palette;
        }
    }

    /// <inheritdoc />
    public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion)
    {
        var bounds = pixelRegion.Rectangle;
        var source = pixelRegion.Buffer;

        Build3DHistogram(source, bounds);
        Get3DMoments(memoryAllocator);
        BuildCube();

        ReadOnlySpan<Moment> momentsSpan = momentsOwner.GetSpan();
        var paletteSpan = paletteOwner.GetSpan();
        for (var k = 0; k < maxColors; k++)
        {
            Mark(ref colorCube[k], (byte)k);

            var moment = Volume(ref colorCube[k], momentsSpan);

            if (moment.Weight > 0)
            {
                ref var color = ref paletteSpan[k];
                color.FromScaledVector4(moment.Normalize());
            }
        }

        ReadOnlyMemory<TPixel> result = paletteOwner.Memory[..maxColors];
        pixelMap = new EuclideanPixelMap<TPixel>(Configuration, result);
        palette = result;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
    {
        return QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(in this), source, bounds);
    }

    /// <inheritdoc />
    public readonly byte GetQuantizedColor(TPixel color, out TPixel match)
    {
        if (isDithering) return (byte)pixelMap.GetClosestColor(color, out match);

        Rgba32 rgba = default;
        color.ToRgba32(ref rgba);

        var r = rgba.R >> (8 - IndexBits);
        var g = rgba.G >> (8 - IndexBits);
        var b = rgba.B >> (8 - IndexBits);
        var a = rgba.A >> (8 - IndexAlphaBits);

        ReadOnlySpan<byte> tagSpan = tagsOwner.GetSpan();
        var index = tagSpan[GetPaletteIndex(r + 1, g + 1, b + 1, a + 1)];
        ref var paletteRef = ref MemoryMarshal.GetReference(pixelMap.Palette.Span);
        match = Unsafe.Add(ref paletteRef, index);
        return index;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            momentsOwner?.Dispose();
            tagsOwner?.Dispose();
            paletteOwner?.Dispose();
            momentsOwner = null;
            tagsOwner = null;
            paletteOwner = null;
        }
    }

    /// <summary>
    ///     Gets the index of the given color in the palette.
    /// </summary>
    /// <param name="r">The red value.</param>
    /// <param name="g">The green value.</param>
    /// <param name="b">The blue value.</param>
    /// <param name="a">The alpha value.</param>
    /// <returns>The index.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private static int GetPaletteIndex(int r, int g, int b, int a)
    {
        return (r << (IndexBits * 2 + IndexAlphaBits))
               + (r << (IndexBits + IndexAlphaBits + 1))
               + (g << (IndexBits + IndexAlphaBits))
               + (r << (IndexBits * 2))
               + (r << (IndexBits + 1))
               + (g << IndexBits)
               + ((r + g + b) << IndexAlphaBits)
               + r + g + b + a;
    }

    /// <summary>
    ///     Computes sum over a box of any given statistic.
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="moments">The moment.</param>
    /// <returns>The result.</returns>
    private static Moment Volume(ref Box cube, ReadOnlySpan<Moment> moments) =>
        moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMax)]
        - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)]
        - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)]
        + moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)]
        - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)]
        + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)]
        + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)]
        - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)]
        - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)]
        + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)]
        + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)]
        - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)]
        + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)]
        - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)]
        - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)]
        + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)];

    /// <summary>
    ///     Computes part of Volume(cube, moment) that doesn't depend on RMax, GMax, BMax, or AMax (depending on direction).
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="moments">The moment.</param>
    /// <returns>The result.</returns>
    private static Moment Bottom(ref Box cube, int direction, ReadOnlySpan<Moment> moments) =>
        direction switch
        {
            // Red
            3 => -moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)],
            // Green
            2 => -moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)],
            // Blue
            1 => -moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)],
            // Alpha
            0 => -moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)],
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

    /// <summary>
    ///     Computes remainder of Volume(cube, moment), substituting position for RMax, GMax, BMax, or AMax (depending on
    ///     direction).
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="position">The position.</param>
    /// <param name="moments">The moment.</param>
    /// <returns>The result.</returns>
    private static Moment Top(ref Box cube, int direction, int position, ReadOnlySpan<Moment> moments) =>
        direction switch
        {
            // Red
            3 => moments[GetPaletteIndex(position, cube.GMax, cube.BMax, cube.AMax)] -
                 moments[GetPaletteIndex(position, cube.GMax, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(position, cube.GMax, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(position, cube.GMax, cube.BMin, cube.AMin)] -
                 moments[GetPaletteIndex(position, cube.GMin, cube.BMax, cube.AMax)] +
                 moments[GetPaletteIndex(position, cube.GMin, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(position, cube.GMin, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(position, cube.GMin, cube.BMin, cube.AMin)],
            // Green
            2 => moments[GetPaletteIndex(cube.RMax, position, cube.BMax, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMax, position, cube.BMax, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMax, position, cube.BMin, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMax, position, cube.BMin, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, position, cube.BMax, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, position, cube.BMax, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, position, cube.BMin, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, position, cube.BMin, cube.AMin)],
            // Blue
            1 => moments[GetPaletteIndex(cube.RMax, cube.GMax, position, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMax, position, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, position, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, position, cube.AMin)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, position, cube.AMax)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, position, cube.AMin)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, position, cube.AMax)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, position, cube.AMin)],
            // Alpha
            0 => moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, position)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, position)] -
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, position)] +
                 moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, position)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, position)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, position)] +
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, position)] -
                 moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, position)],
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

    /// <summary>
    ///     Builds a 3-D color histogram of <c>counts, r/g/b, c^2</c>.
    /// </summary>
    /// <param name="source">The source data.</param>
    /// <param name="bounds">The bounds within the source image to quantize.</param>
    private void Build3DHistogram(Buffer2D<TPixel> source, Rectangle bounds)
    {
        var momentSpan = momentsOwner.GetSpan();

        // Build up the 3-D color histogram
        using var buffer = memoryAllocator.Allocate<Rgba32>(bounds.Width);
        var bufferSpan = buffer.GetSpan();

        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            var row = source.GetRowSpan(y).Slice(bounds.Left, bounds.Width);
            PixelOperations<TPixel>.Instance.ToRgba32(Configuration, row, bufferSpan);

            for (var x = 0; x < bufferSpan.Length; x++)
            {
                var rgba = bufferSpan[x];

                var r = (rgba.R >> (8 - IndexBits)) + 1;
                var g = (rgba.G >> (8 - IndexBits)) + 1;
                var b = (rgba.B >> (8 - IndexBits)) + 1;
                var a = (rgba.A >> (8 - IndexAlphaBits)) + 1;

                momentSpan[GetPaletteIndex(r, g, b, a)] += rgba;
            }
        }
    }

    /// <summary>
    ///     Converts the histogram into moments so that we can rapidly calculate the sums of the above quantities over any
    ///     desired box.
    /// </summary>
    /// <param name="allocator">The memory allocator used for allocating buffers.</param>
    private void Get3DMoments(MemoryAllocator allocator)
    {
        using var volume = allocator.Allocate<Moment>(IndexCount * IndexAlphaCount);
        using var area = allocator.Allocate<Moment>(IndexAlphaCount);

        var momentSpan = momentsOwner.GetSpan();
        var volumeSpan = volume.GetSpan();
        var areaSpan = area.GetSpan();
        var baseIndex = GetPaletteIndex(1, 0, 0, 0);

        for (var r = 1; r < IndexCount; r++)
        {
            volumeSpan.Clear();

            for (var g = 1; g < IndexCount; g++)
            {
                areaSpan.Clear();

                for (var b = 1; b < IndexCount; b++)
                {
                    Moment line = default;

                    for (var a = 1; a < IndexAlphaCount; a++)
                    {
                        var ind1 = GetPaletteIndex(r, g, b, a);
                        line += momentSpan[ind1];

                        areaSpan[a] += line;

                        var inv = b * IndexAlphaCount + a;
                        volumeSpan[inv] += areaSpan[a];

                        var ind2 = ind1 - baseIndex;
                        momentSpan[ind1] = momentSpan[ind2] + volumeSpan[inv];
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Computes the weighted variance of a box cube.
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <returns>The <see cref="float" />.</returns>
    private double Variance(ref Box cube)
    {
        ReadOnlySpan<Moment> momentSpan = momentsOwner.GetSpan();

        var volume = Volume(ref cube, momentSpan);
        var variance =
            momentSpan[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMax)]
            - momentSpan[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)]
            - momentSpan[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)]
            + momentSpan[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)]
            - momentSpan[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)]
            + momentSpan[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)]
            + momentSpan[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)]
            - momentSpan[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)]
            - momentSpan[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)]
            + momentSpan[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)]
            + momentSpan[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)]
            - momentSpan[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)]
            + momentSpan[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)]
            - momentSpan[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)]
            - momentSpan[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)]
            + momentSpan[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)];

        var vector = new Vector4(volume.R, volume.G, volume.B, volume.A);
        return variance.Moment2 - Vector4.Dot(vector, vector) / volume.Weight;
    }

    /// <summary>
    ///     We want to minimize the sum of the variances of two sub-boxes.
    ///     The sum(c^2) terms can be ignored since their sum over both sub-boxes
    ///     is the same (the sum for the whole box) no matter where we split.
    ///     The remaining terms have a minus sign in the variance formula,
    ///     so we drop the minus sign and maximize the sum of the two terms.
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="first">The first position.</param>
    /// <param name="last">The last position.</param>
    /// <param name="cut">The cutting point.</param>
    /// <param name="whole">The whole moment.</param>
    /// <returns>The <see cref="float" />.</returns>
    private float Maximize(ref Box cube, int direction, int first, int last, out int cut, Moment whole)
    {
        ReadOnlySpan<Moment> momentSpan = momentsOwner.GetSpan();
        var bottom = Bottom(ref cube, direction, momentSpan);

        var max = 0F;
        cut = -1;

        for (var i = first; i < last; i++)
        {
            var half = bottom + Top(ref cube, direction, i, momentSpan);

            if (half.Weight == 0) continue;

            var vector = new Vector4(half.R, half.G, half.B, half.A);
            var temp = Vector4.Dot(vector, vector) / half.Weight;

            half = whole - half;

            if (half.Weight == 0) continue;

            vector = new Vector4(half.R, half.G, half.B, half.A);
            temp += Vector4.Dot(vector, vector) / half.Weight;

            if (temp > max)
            {
                max = temp;
                cut = i;
            }
        }

        return max;
    }

    /// <summary>
    ///     Cuts a box.
    /// </summary>
    /// <param name="set1">The first set.</param>
    /// <param name="set2">The second set.</param>
    /// <returns>Returns a value indicating whether the box has been split.</returns>
    private bool Cut(ref Box set1, ref Box set2)
    {
        ReadOnlySpan<Moment> momentSpan = momentsOwner.GetSpan();
        var whole = Volume(ref set1, momentSpan);

        var maxR = Maximize(ref set1, 3, set1.RMin + 1, set1.RMax, out var cutR, whole);
        var maxG = Maximize(ref set1, 2, set1.GMin + 1, set1.GMax, out var cutG, whole);
        var maxB = Maximize(ref set1, 1, set1.BMin + 1, set1.BMax, out var cutB, whole);
        var maxA = Maximize(ref set1, 0, set1.AMin + 1, set1.AMax, out var cutA, whole);

        int dir;

        if (maxR >= maxG && maxR >= maxB && maxR >= maxA)
        {
            dir = 3;

            if (cutR < 0) return false;
        }
        else if (maxG >= maxR && maxG >= maxB && maxG >= maxA)
        {
            dir = 2;
        }
        else if (maxB >= maxR && maxB >= maxG && maxB >= maxA)
        {
            dir = 1;
        }
        else
        {
            dir = 0;
        }

        set2.RMax = set1.RMax;
        set2.GMax = set1.GMax;
        set2.BMax = set1.BMax;
        set2.AMax = set1.AMax;

        switch (dir)
        {
            // Red
            case 3:
                set2.RMin = set1.RMax = cutR;
                set2.GMin = set1.GMin;
                set2.BMin = set1.BMin;
                set2.AMin = set1.AMin;
                break;

            // Green
            case 2:
                set2.GMin = set1.GMax = cutG;
                set2.RMin = set1.RMin;
                set2.BMin = set1.BMin;
                set2.AMin = set1.AMin;
                break;

            // Blue
            case 1:
                set2.BMin = set1.BMax = cutB;
                set2.RMin = set1.RMin;
                set2.GMin = set1.GMin;
                set2.AMin = set1.AMin;
                break;

            // Alpha
            case 0:
                set2.AMin = set1.AMax = cutA;
                set2.RMin = set1.RMin;
                set2.GMin = set1.GMin;
                set2.BMin = set1.BMin;
                break;
        }

        set1.Volume = (set1.RMax - set1.RMin) * (set1.GMax - set1.GMin) * (set1.BMax - set1.BMin) *
                      (set1.AMax - set1.AMin);
        set2.Volume = (set2.RMax - set2.RMin) * (set2.GMax - set2.GMin) * (set2.BMax - set2.BMin) *
                      (set2.AMax - set2.AMin);

        return true;
    }

    /// <summary>
    ///     Marks a color space tag.
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="label">A label.</param>
    private void Mark(ref Box cube, byte label)
    {
        var tagSpan = tagsOwner.GetSpan();

        for (var r = cube.RMin + 1; r <= cube.RMax; r++)
        for (var g = cube.GMin + 1; g <= cube.GMax; g++)
        for (var b = cube.BMin + 1; b <= cube.BMax; b++)
        for (var a = cube.AMin + 1; a <= cube.AMax; a++)
            tagSpan[GetPaletteIndex(r, g, b, a)] = label;
    }

    /// <summary>
    ///     Builds the cube.
    /// </summary>
    private void BuildCube()
    {
        // Store the volume variance.
        using var vvOwner = Configuration.MemoryAllocator.Allocate<double>(maxColors);
        var vv = vvOwner.GetSpan();

        ref var cube = ref colorCube[0];
        cube.RMin = cube.GMin = cube.BMin = cube.AMin = 0;
        cube.RMax = cube.GMax = cube.BMax = IndexCount - 1;
        cube.AMax = IndexAlphaCount - 1;

        var next = 0;

        for (var i = 1; i < maxColors; i++)
        {
            ref var nextCube = ref colorCube[next];
            ref var currentCube = ref colorCube[i];
            if (Cut(ref nextCube, ref currentCube))
            {
                vv[next] = nextCube.Volume > 1 ? Variance(ref nextCube) : 0D;
                vv[i] = currentCube.Volume > 1 ? Variance(ref currentCube) : 0D;
            }
            else
            {
                vv[next] = 0D;
                i--;
            }

            next = 0;

            var temp = vv[0];
            for (var k = 1; k <= i; k++)
                if (vv[k] > temp)
                {
                    temp = vv[k];
                    next = k;
                }

            if (temp <= 0D)
            {
                maxColors = i + 1;
                break;
            }
        }
    }

    private struct Moment
    {
        /// <summary>
        ///     Moment of <c>r*P(c)</c>.
        /// </summary>
        public long R;

        /// <summary>
        ///     Moment of <c>g*P(c)</c>.
        /// </summary>
        public long G;

        /// <summary>
        ///     Moment of <c>b*P(c)</c>.
        /// </summary>
        public long B;

        /// <summary>
        ///     Moment of <c>a*P(c)</c>.
        /// </summary>
        public long A;

        /// <summary>
        ///     Moment of <c>P(c)</c>.
        /// </summary>
        public long Weight;

        /// <summary>
        ///     Moment of <c>c^2*P(c)</c>.
        /// </summary>
        public double Moment2;

        [MethodImpl(InliningOptions.ShortMethod)]
        public static Moment operator +(Moment x, Moment y)
        {
            x.R += y.R;
            x.G += y.G;
            x.B += y.B;
            x.A += y.A;
            x.Weight += y.Weight;
            x.Moment2 += y.Moment2;
            return x;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static Moment operator -(Moment x, Moment y)
        {
            x.R -= y.R;
            x.G -= y.G;
            x.B -= y.B;
            x.A -= y.A;
            x.Weight -= y.Weight;
            x.Moment2 -= y.Moment2;
            return x;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static Moment operator -(Moment x)
        {
            x.R = -x.R;
            x.G = -x.G;
            x.B = -x.B;
            x.A = -x.A;
            x.Weight = -x.Weight;
            x.Moment2 = -x.Moment2;
            return x;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static Moment operator +(Moment x, Rgba32 y)
        {
            x.R += y.R;
            x.G += y.G;
            x.B += y.B;
            x.A += y.A;
            x.Weight++;

            var vector = new Vector4(y.R, y.G, y.B, y.A);
            x.Moment2 += Vector4.Dot(vector, vector);

            return x;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public readonly Vector4 Normalize()
        {
            return new Vector4(R, G, B, A) / Weight / 255F;
        }
    }

    /// <summary>
    ///     Represents a box color cube.
    /// </summary>
    private struct Box : IEquatable<Box>
    {
        /// <summary>
        ///     Gets or sets the min red value, exclusive.
        /// </summary>
        public int RMin;

        /// <summary>
        ///     Gets or sets the max red value, inclusive.
        /// </summary>
        public int RMax;

        /// <summary>
        ///     Gets or sets the min green value, exclusive.
        /// </summary>
        public int GMin;

        /// <summary>
        ///     Gets or sets the max green value, inclusive.
        /// </summary>
        public int GMax;

        /// <summary>
        ///     Gets or sets the min blue value, exclusive.
        /// </summary>
        public int BMin;

        /// <summary>
        ///     Gets or sets the max blue value, inclusive.
        /// </summary>
        public int BMax;

        /// <summary>
        ///     Gets or sets the min alpha value, exclusive.
        /// </summary>
        public int AMin;

        /// <summary>
        ///     Gets or sets the max alpha value, inclusive.
        /// </summary>
        public int AMax;

        /// <summary>
        ///     Gets or sets the volume.
        /// </summary>
        public int Volume;

        /// <inheritdoc />
        public readonly override bool Equals(object obj)
        {
            return obj is Box box
                   && Equals(box);
        }

        /// <inheritdoc />
        public readonly bool Equals(Box other)
        {
            return RMin == other.RMin
                   && RMax == other.RMax
                   && GMin == other.GMin
                   && GMax == other.GMax
                   && BMin == other.BMin
                   && BMax == other.BMax
                   && AMin == other.AMin
                   && AMax == other.AMax
                   && Volume == other.Volume;
        }

        /// <inheritdoc />
        public readonly override int GetHashCode()
        {
            HashCode hash = default;
            hash.Add(RMin);
            hash.Add(RMax);
            hash.Add(GMin);
            hash.Add(GMax);
            hash.Add(BMin);
            hash.Add(BMax);
            hash.Add(AMin);
            hash.Add(AMax);
            hash.Add(Volume);
            return hash.ToHashCode();
        }
    }
}