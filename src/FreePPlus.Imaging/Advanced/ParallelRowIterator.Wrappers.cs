// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FreePPlus.Imaging.Memory;

#pragma warning disable IDE0290

// ReSharper disable InconsistentNaming

namespace FreePPlus.Imaging.Advanced;

//was previously: namespace SixLabors.ImageSharp.Advanced;

/// <content>
///     Utility methods for batched processing of pixel row intervals.
///     Parallel execution is optimized for image processing based on values defined
///     <see cref="ParallelExecutionSettings" /> or <see cref="Configuration" />.
///     Using this class is preferred over direct usage of <see cref="Parallel" /> utility methods.
/// </content>
public static partial class ParallelRowIterator
{
    private readonly struct RowOperationWrapper<T>
        where T : struct, IRowOperation
    {
        private readonly int minY;
        private readonly int maxY;
        private readonly int stepY;
        private readonly T action;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperationWrapper(
            int minY,
            int maxY,
            int stepY,
            in T action)
        {
            this.minY = minY;
            this.maxY = maxY;
            this.stepY = stepY;
            this.action = action;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int i)
        {
            var yMin = minY + i * stepY;

            if (yMin >= maxY) return;

            var yMax = Math.Min(yMin + stepY, maxY);

            for (var y = yMin; y < yMax; y++)
            {
                // Skip the safety copy when invoking a potentially impure method on a readonly field
                Unsafe.AsRef(in action).Invoke(y);
            }
        }
    }

    private readonly struct RowOperationWrapper<T, TBuffer>
        where T : struct, IRowOperation<TBuffer>
        where TBuffer : unmanaged
    {
        private readonly int minY;
        private readonly int maxY;
        private readonly int stepY;
        private readonly int width;
        private readonly MemoryAllocator allocator;
        private readonly T action;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperationWrapper(
            int minY,
            int maxY,
            int stepY,
            int width,
            MemoryAllocator allocator,
            in T action)
        {
            this.minY = minY;
            this.maxY = maxY;
            this.stepY = stepY;
            this.width = width;
            this.allocator = allocator;
            this.action = action;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int i)
        {
            var yMin = minY + i * stepY;

            if (yMin >= maxY) return;

            var yMax = Math.Min(yMin + stepY, maxY);

            using var buffer = allocator.Allocate<TBuffer>(width);

            var span = buffer.Memory.Span;

            for (var y = yMin; y < yMax; y++) { Unsafe.AsRef(in action).Invoke(y, span); }
        }
    }

    private readonly struct RowIntervalOperationWrapper<T>
        where T : struct, IRowIntervalOperation
    {
        private readonly int minY;
        private readonly int maxY;
        private readonly int stepY;
        private readonly T operation;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowIntervalOperationWrapper(
            int minY,
            int maxY,
            int stepY,
            in T operation)
        {
            this.minY = minY;
            this.maxY = maxY;
            this.stepY = stepY;
            this.operation = operation;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int i)
        {
            var yMin = minY + i * stepY;

            if (yMin >= maxY) return;

            var yMax = Math.Min(yMin + stepY, maxY);
            var rows = new RowInterval(yMin, yMax);

            // Skip the safety copy when invoking a potentially impure method on a readonly field
            Unsafe.AsRef(in operation).Invoke(in rows);
        }
    }

    private readonly struct RowIntervalOperationWrapper<T, TBuffer>
        where T : struct, IRowIntervalOperation<TBuffer>
        where TBuffer : unmanaged
    {
        private readonly int minY;
        private readonly int maxY;
        private readonly int stepY;
        private readonly int width;
        private readonly MemoryAllocator allocator;
        private readonly T operation;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowIntervalOperationWrapper(
            int minY,
            int maxY,
            int stepY,
            int width,
            MemoryAllocator allocator,
            in T operation)
        {
            this.minY = minY;
            this.maxY = maxY;
            this.stepY = stepY;
            this.width = width;
            this.allocator = allocator;
            this.operation = operation;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int i)
        {
            var yMin = minY + i * stepY;

            if (yMin >= maxY) return;

            var yMax = Math.Min(yMin + stepY, maxY);
            var rows = new RowInterval(yMin, yMax);

            using var buffer = allocator.Allocate<TBuffer>(width);

            Unsafe.AsRef(in operation).Invoke(in rows, buffer.Memory.Span);
        }
    }
}