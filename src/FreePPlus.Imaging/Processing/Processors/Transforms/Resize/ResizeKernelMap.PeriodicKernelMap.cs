// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal partial class ResizeKernelMap
{
    /// <summary>
    ///     Memory-optimized <see cref="ResizeKernelMap" /> where repeating rows are stored only once.
    /// </summary>
    private sealed class PeriodicKernelMap : ResizeKernelMap
    {
        private readonly int cornerInterval;
        private readonly int period;

        public PeriodicKernelMap(
            MemoryAllocator memoryAllocator,
            int sourceLength,
            int destinationLength,
            double ratio,
            double scale,
            int radius,
            int period,
            int cornerInterval)
            : base(
                memoryAllocator,
                sourceLength,
                destinationLength,
                cornerInterval * 2 + period,
                ratio,
                scale,
                radius)
        {
            this.cornerInterval = cornerInterval;
            this.period = period;
        }

        internal override string Info => base.Info + $"|period:{period}|cornerInterval:{cornerInterval}";

        protected internal override void Initialize<TResampler>(in TResampler sampler)
        {
            // Build top corner data + one period of the mosaic data:
            var startOfFirstRepeatedMosaic = cornerInterval + period;

            for (var i = 0; i < startOfFirstRepeatedMosaic; i++) kernels[i] = BuildKernel(in sampler, i, i);

            // Copy the mosaics:
            var bottomStartDest = DestinationLength - cornerInterval;
            for (var i = startOfFirstRepeatedMosaic; i < bottomStartDest; i++)
            {
                var center = (i + .5) * ratio - .5;
                var left = (int)TolerantMath.Ceiling(center - radius);
                var kernel = kernels[i - period];
                kernels[i] = kernel.AlterLeftValue(left);
            }

            // Build bottom corner data:
            var bottomStartData = cornerInterval + period;
            for (var i = 0; i < cornerInterval; i++)
                kernels[bottomStartDest + i] = BuildKernel(in sampler, bottomStartDest + i, bottomStartData + i);
        }
    }
}