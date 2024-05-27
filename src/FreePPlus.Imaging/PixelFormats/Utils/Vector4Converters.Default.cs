﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.PixelFormats.Utils;

//was previously: namespace SixLabors.ImageSharp.PixelFormats.Utils;

/// <summary>
///     Helper class for (bulk) conversion of <see cref="Vector4" /> buffers to/from other buffer types.
/// </summary>
internal static partial class Vector4Converters
{
    /// <summary>
    ///     Provides default implementations for batched to/from <see cref="Vector4" /> conversion.
    ///     WARNING: The methods prefixed with "Unsafe" are operating without bounds checking and input validation!
    ///     Input validation is the responsibility of the caller!
    /// </summary>
    public static class Default
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        public static void FromVector4<TPixel>(
            Span<Vector4> sourceVectors,
            Span<TPixel> destPixels,
            PixelConversionModifiers modifiers)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Guard.DestinationShouldNotBeTooShort(sourceVectors, destPixels, nameof(destPixels));

            UnsafeFromVector4(sourceVectors, destPixels, modifiers);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static void ToVector4<TPixel>(
            ReadOnlySpan<TPixel> sourcePixels,
            Span<Vector4> destVectors,
            PixelConversionModifiers modifiers)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Guard.DestinationShouldNotBeTooShort(sourcePixels, destVectors, nameof(destVectors));

            UnsafeToVector4(sourcePixels, destVectors, modifiers);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static void UnsafeFromVector4<TPixel>(
            Span<Vector4> sourceVectors,
            Span<TPixel> destPixels,
            PixelConversionModifiers modifiers)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ApplyBackwardConversionModifiers(sourceVectors, modifiers);

            if (modifiers.IsDefined(PixelConversionModifiers.Scale))
                UnsafeFromScaledVector4Core(sourceVectors, destPixels);
            else
                UnsafeFromVector4Core(sourceVectors, destPixels);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public static void UnsafeToVector4<TPixel>(
            ReadOnlySpan<TPixel> sourcePixels,
            Span<Vector4> destVectors,
            PixelConversionModifiers modifiers)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (modifiers.IsDefined(PixelConversionModifiers.Scale))
                UnsafeToScaledVector4Core(sourcePixels, destVectors);
            else
                UnsafeToVector4Core(sourcePixels, destVectors);

            ApplyForwardConversionModifiers(destVectors, modifiers);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        private static void UnsafeFromVector4Core<TPixel>(
            ReadOnlySpan<Vector4> sourceVectors,
            Span<TPixel> destPixels)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(sourceVectors);
            ref var destRef = ref MemoryMarshal.GetReference(destPixels);

            for (var i = 0; i < sourceVectors.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceRef, i);
                ref var dp = ref Unsafe.Add(ref destRef, i);
                dp.FromVector4(sp);
            }
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        private static void UnsafeToVector4Core<TPixel>(
            ReadOnlySpan<TPixel> sourcePixels,
            Span<Vector4> destVectors)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(sourcePixels);
            ref var destRef = ref MemoryMarshal.GetReference(destVectors);

            for (var i = 0; i < sourcePixels.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceRef, i);
                ref var dp = ref Unsafe.Add(ref destRef, i);
                dp = sp.ToVector4();
            }
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        private static void UnsafeFromScaledVector4Core<TPixel>(
            ReadOnlySpan<Vector4> sourceVectors,
            Span<TPixel> destinationColors)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(sourceVectors);
            ref var destRef = ref MemoryMarshal.GetReference(destinationColors);

            for (var i = 0; i < sourceVectors.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceRef, i);
                ref var dp = ref Unsafe.Add(ref destRef, i);
                dp.FromScaledVector4(sp);
            }
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        private static void UnsafeToScaledVector4Core<TPixel>(
            ReadOnlySpan<TPixel> sourceColors,
            Span<Vector4> destinationVectors)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(sourceColors);
            ref var destRef = ref MemoryMarshal.GetReference(destinationVectors);

            for (var i = 0; i < sourceColors.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceRef, i);
                ref var dp = ref Unsafe.Add(ref destRef, i);
                dp = sp.ToScaledVector4();
            }
        }
    }
}