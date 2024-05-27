// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.PixelFormats.Utils;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <content>
///     Provides optimized overrides for bulk operations.
/// </content>
public partial struct RgbaVector
{
    /// <summary>
    ///     <see cref="PixelOperations{TPixel}" /> implementation optimized for <see cref="RgbaVector" />.
    /// </summary>
    internal class PixelOperations : PixelOperations<RgbaVector>
    {
        /// <inheritdoc />
        public override void FromVector4Destructive(
            Configuration configuration,
            Span<Vector4> sourceVectors,
            Span<RgbaVector> destinationPixels,
            PixelConversionModifiers modifiers)
        {
            Guard.DestinationShouldNotBeTooShort(sourceVectors, destinationPixels, nameof(destinationPixels));

            Vector4Converters.ApplyBackwardConversionModifiers(sourceVectors, modifiers);
            MemoryMarshal.Cast<Vector4, RgbaVector>(sourceVectors).CopyTo(destinationPixels);
        }

        /// <inheritdoc />
        public override void ToVector4(
            Configuration configuration,
            ReadOnlySpan<RgbaVector> sourcePixels,
            Span<Vector4> destinationVectors,
            PixelConversionModifiers modifiers)
        {
            Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationVectors, nameof(destinationVectors));

            MemoryMarshal.Cast<RgbaVector, Vector4>(sourcePixels).CopyTo(destinationVectors);
            Vector4Converters.ApplyForwardConversionModifiers(destinationVectors, modifiers);
        }

        public override void ToL8(Configuration configuration, ReadOnlySpan<RgbaVector> sourcePixels,
            Span<L8> destinationPixels)
        {
            Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, nameof(destinationPixels));

            ref var sourceBaseRef = ref Unsafe.As<RgbaVector, Vector4>(ref MemoryMarshal.GetReference(sourcePixels));
            ref var destBaseRef = ref MemoryMarshal.GetReference(destinationPixels);

            for (var i = 0; i < sourcePixels.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceBaseRef, i);
                ref var dp = ref Unsafe.Add(ref destBaseRef, i);

                dp.ConvertFromRgbaScaledVector4(sp);
            }
        }

        public override void ToL16(Configuration configuration, ReadOnlySpan<RgbaVector> sourcePixels,
            Span<L16> destinationPixels)
        {
            Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, nameof(destinationPixels));

            ref var sourceBaseRef = ref Unsafe.As<RgbaVector, Vector4>(ref MemoryMarshal.GetReference(sourcePixels));
            ref var destBaseRef = ref MemoryMarshal.GetReference(destinationPixels);

            for (var i = 0; i < sourcePixels.Length; i++)
            {
                ref var sp = ref Unsafe.Add(ref sourceBaseRef, i);
                ref var dp = ref Unsafe.Add(ref destBaseRef, i);

                dp.ConvertFromRgbaScaledVector4(sp);
            }
        }
    }
}