// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable InconsistentNaming
namespace FreePPlus.Imaging.Formats.Jpeg.Components;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components;

internal partial struct Block8x8F
{
    /// <summary>
    ///     Copy block data into the destination color buffer pixel area with the provided horizontal and vertical scale
    ///     factors.
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ScaledCopyTo(in Buffer2DRegion<float> region, int horizontalScale, int verticalScale)
    {
        ref var areaOrigin = ref region.GetReferenceToOrigin();
        ScaledCopyTo(ref areaOrigin, region.Stride, horizontalScale, verticalScale);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public void ScaledCopyTo(ref float areaOrigin, int areaStride, int horizontalScale, int verticalScale)
    {
        if (horizontalScale == 1 && verticalScale == 1)
        {
            Copy1x1Scale(ref areaOrigin, areaStride);
            return;
        }

        if (horizontalScale == 2 && verticalScale == 2)
        {
            Copy2x2Scale(ref areaOrigin, areaStride);
            return;
        }

        // TODO: Optimize: implement all cases with scale-specific, loopless code!
        CopyArbitraryScale(ref areaOrigin, areaStride, horizontalScale, verticalScale);
    }

    public void Copy1x1Scale(ref float areaOrigin, int areaStride)
    {
        ref var selfBase = ref Unsafe.As<Block8x8F, byte>(ref this);
        ref var destBase = ref Unsafe.As<float, byte>(ref areaOrigin);
        var destStride = areaStride * sizeof(float);

        CopyRowImpl(ref selfBase, ref destBase, destStride, 0);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 1);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 2);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 3);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 4);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 5);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 6);
        CopyRowImpl(ref selfBase, ref destBase, destStride, 7);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyRowImpl(ref byte selfBase, ref byte destBase, int destStride, int row)
    {
        ref var s = ref Unsafe.Add(ref selfBase, row * 8 * sizeof(float));
        ref var d = ref Unsafe.Add(ref destBase, row * destStride);
        Unsafe.CopyBlock(ref d, ref s, 8 * sizeof(float));
    }

    private void Copy2x2Scale(ref float areaOrigin, int areaStride)
    {
        ref var destBase = ref Unsafe.As<float, Vector2>(ref areaOrigin);
        var destStride = areaStride / 2;

        WidenCopyRowImpl2x2(ref destBase, 0, destStride);
        WidenCopyRowImpl2x2(ref destBase, 1, destStride);
        WidenCopyRowImpl2x2(ref destBase, 2, destStride);
        WidenCopyRowImpl2x2(ref destBase, 3, destStride);
        WidenCopyRowImpl2x2(ref destBase, 4, destStride);
        WidenCopyRowImpl2x2(ref destBase, 5, destStride);
        WidenCopyRowImpl2x2(ref destBase, 6, destStride);
        WidenCopyRowImpl2x2(ref destBase, 7, destStride);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WidenCopyRowImpl2x2(ref Vector2 destBase, int row, int destStride)
    {
        ref var sLeft = ref Unsafe.Add(ref V0L, 2 * row);
        ref var sRight = ref Unsafe.Add(ref sLeft, 1);

        var offset = 2 * row * destStride;
        ref var dTopLeft = ref Unsafe.As<Vector2, Vector4>(ref Unsafe.Add(ref destBase, offset));
        ref var dBottomLeft = ref Unsafe.As<Vector2, Vector4>(ref Unsafe.Add(ref destBase, offset + destStride));

        var xyLeft = new Vector4(sLeft.X);
        xyLeft.Z = sLeft.Y;
        xyLeft.W = sLeft.Y;

        var zwLeft = new Vector4(sLeft.Z);
        zwLeft.Z = sLeft.W;
        zwLeft.W = sLeft.W;

        var xyRight = new Vector4(sRight.X);
        xyRight.Z = sRight.Y;
        xyRight.W = sRight.Y;

        var zwRight = new Vector4(sRight.Z);
        zwRight.Z = sRight.W;
        zwRight.W = sRight.W;

        dTopLeft = xyLeft;
        Unsafe.Add(ref dTopLeft, 1) = zwLeft;
        Unsafe.Add(ref dTopLeft, 2) = xyRight;
        Unsafe.Add(ref dTopLeft, 3) = zwRight;

        dBottomLeft = xyLeft;
        Unsafe.Add(ref dBottomLeft, 1) = zwLeft;
        Unsafe.Add(ref dBottomLeft, 2) = xyRight;
        Unsafe.Add(ref dBottomLeft, 3) = zwRight;
    }

    [MethodImpl(InliningOptions.ColdPath)]
    private void CopyArbitraryScale(ref float areaOrigin, int areaStride, int horizontalScale, int verticalScale)
    {
        for (var y = 0; y < 8; y++)
        {
            var yy = y * verticalScale;
            var y8 = y * 8;

            for (var x = 0; x < 8; x++)
            {
                var xx = x * horizontalScale;

                var value = this[y8 + x];

                for (var i = 0; i < verticalScale; i++)
                {
                    var baseIdx = (yy + i) * areaStride + xx;

                    for (var j = 0; j < horizontalScale; j++)
                        // area[xx + j, yy + i] = value;
                        Unsafe.Add(ref areaOrigin, baseIdx + j) = value;
                }
            }
        }
    }
}