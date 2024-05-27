// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Gets the closest color to the supplied color based upon the Euclidean distance.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal readonly struct EuclideanPixelMap<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Vector4[] vectorCache;
    private readonly ConcurrentDictionary<TPixel, int> distanceCache;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EuclideanPixelMap{TPixel}" /> struct.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="palette">The color palette to map from.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public EuclideanPixelMap(Configuration configuration, ReadOnlyMemory<TPixel> palette)
    {
        Palette = palette;
        vectorCache = new Vector4[palette.Length];

        // Use the same rules across all target frameworks.
        distanceCache = new ConcurrentDictionary<TPixel, int>(Environment.ProcessorCount, 31);
        PixelOperations<TPixel>.Instance.ToVector4(configuration, Palette.Span, vectorCache);
    }

    /// <summary>
    ///     Gets the color palette of this <see cref="EuclideanPixelMap{TPixel}" />.
    ///     The palette memory is owned by the palette source that created it.
    /// </summary>
    public ReadOnlyMemory<TPixel> Palette
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get;
    }

    /// <summary>
    ///     Returns the closest color in the palette and the index of that pixel.
    ///     The palette contents must match the one used in the constructor.
    /// </summary>
    /// <param name="color">The color to match.</param>
    /// <param name="match">The matched color.</param>
    /// <returns>The <see cref="int" /> index.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public int GetClosestColor(TPixel color, out TPixel match)
    {
        ref var paletteRef = ref MemoryMarshal.GetReference(Palette.Span);

        // Check if the color is in the lookup table
        if (!distanceCache.TryGetValue(color, out var index))
            return GetClosestColorSlow(color, ref paletteRef, out match);

        match = Unsafe.Add(ref paletteRef, index);
        return index;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private int GetClosestColorSlow(TPixel color, ref TPixel paletteRef, out TPixel match)
    {
        // Loop through the palette and find the nearest match.
        var index = 0;
        var leastDistance = float.MaxValue;
        var vector = color.ToVector4();
        ref var vectorCacheRef = ref MemoryMarshal.GetReference<Vector4>(vectorCache);
        for (var i = 0; i < Palette.Length; i++)
        {
            var candidate = Unsafe.Add(ref vectorCacheRef, i);
            var distance = Vector4.DistanceSquared(vector, candidate);

            // If it's an exact match, exit the loop
            if (distance == 0)
            {
                index = i;
                break;
            }

            if (distance < leastDistance)
            {
                // Less than... assign.
                index = i;
                leastDistance = distance;
            }
        }

        // Now I have the index, pop it into the cache for next time
        distanceCache[color] = index;
        match = Unsafe.Add(ref paletteRef, index);
        return index;
    }
}