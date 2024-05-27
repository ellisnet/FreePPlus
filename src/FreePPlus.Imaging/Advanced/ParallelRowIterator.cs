// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FreePPlus.Imaging.Memory;

// ReSharper disable UnusedMember.Global

namespace FreePPlus.Imaging.Advanced;

//was previously: namespace SixLabors.ImageSharp.Advanced;

/// <summary>
///     Utility methods for batched processing of pixel row intervals.
///     Parallel execution is optimized for image processing based on values defined
///     <see cref="ParallelExecutionSettings" /> or <see cref="Configuration" />.
///     Using this class is preferred over direct usage of <see cref="Parallel" /> utility methods.
/// </summary>
public static partial class ParallelRowIterator
{
    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /> to get the parallel settings from.</param>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single row.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void IterateRows<T>(Configuration configuration, Rectangle rectangle, in T operation)
        where T : struct, IRowOperation
    {
        var parallelSettings = ParallelExecutionSettings.FromConfiguration(configuration);
        IterateRows(rectangle, in parallelSettings, in operation);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="parallelSettings">The <see cref="ParallelExecutionSettings" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single row.</param>
    public static void IterateRows<T>(
        Rectangle rectangle,
        in ParallelExecutionSettings parallelSettings,
        in T operation)
        where T : struct, IRowOperation
    {
        ValidateRectangle(rectangle);

        var top = rectangle.Top;
        var bottom = rectangle.Bottom;
        var width = rectangle.Width;
        var height = rectangle.Height;

        var maxSteps = DivideCeil(width * height, parallelSettings.MinimumPixelsProcessedPerTask);
        var numOfSteps = Math.Min(parallelSettings.MaxDegreeOfParallelism, maxSteps);

        // Avoid TPL overhead in this trivial case:
        if (numOfSteps == 1)
        {
            for (var y = top; y < bottom; y++) { Unsafe.AsRef(in operation).Invoke(y); }
            return;
        }

        var verticalStep = DivideCeil(rectangle.Height, numOfSteps);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = numOfSteps };
        var wrappingOperation = new RowOperationWrapper<T>(top, bottom, verticalStep, in operation);

        Parallel.For(
            0,
            numOfSteps,
            parallelOptions,
            wrappingOperation.Invoke);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches.
    ///     instantiating a temporary buffer for each <paramref name="operation" /> invocation.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <typeparam name="TBuffer">The type of buffer elements.</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /> to get the parallel settings from.</param>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single row.</param>
    public static void IterateRows<T, TBuffer>(Configuration configuration, Rectangle rectangle, in T operation)
        where T : struct, IRowOperation<TBuffer>
        where TBuffer : unmanaged
    {
        var parallelSettings = ParallelExecutionSettings.FromConfiguration(configuration);
        IterateRows<T, TBuffer>(rectangle, in parallelSettings, in operation);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches.
    ///     instantiating a temporary buffer for each <paramref name="operation" /> invocation.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <typeparam name="TBuffer">The type of buffer elements.</typeparam>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="parallelSettings">The <see cref="ParallelExecutionSettings" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single row.</param>
    public static void IterateRows<T, TBuffer>(
        Rectangle rectangle,
        in ParallelExecutionSettings parallelSettings,
        in T operation)
        where T : struct, IRowOperation<TBuffer>
        where TBuffer : unmanaged
    {
        ValidateRectangle(rectangle);

        var top = rectangle.Top;
        var bottom = rectangle.Bottom;
        var width = rectangle.Width;
        var height = rectangle.Height;

        var maxSteps = DivideCeil(width * height, parallelSettings.MinimumPixelsProcessedPerTask);
        var numOfSteps = Math.Min(parallelSettings.MaxDegreeOfParallelism, maxSteps);
        var allocator = parallelSettings.MemoryAllocator;

        // Avoid TPL overhead in this trivial case:
        if (numOfSteps == 1)
        {
            using var buffer = allocator.Allocate<TBuffer>(width);
            var span = buffer.Memory.Span;

            for (var y = top; y < bottom; y++) { Unsafe.AsRef(in operation).Invoke(y, span); }

            return;
        }

        var verticalStep = DivideCeil(height, numOfSteps);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = numOfSteps };
        var wrappingOperation =
            new RowOperationWrapper<T, TBuffer>(top, bottom, verticalStep, width, allocator, in operation);

        Parallel.For(
            0,
            numOfSteps,
            parallelOptions,
            wrappingOperation.Invoke);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches defined by <see cref="RowInterval" />-s.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /> to get the parallel settings from.</param>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single <see cref="RowInterval" />.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void IterateRowIntervals<T>(Configuration configuration, Rectangle rectangle, in T operation)
        where T : struct, IRowIntervalOperation
    {
        var parallelSettings = ParallelExecutionSettings.FromConfiguration(configuration);
        IterateRowIntervals(rectangle, in parallelSettings, in operation);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches defined by <see cref="RowInterval" />-s.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="parallelSettings">The <see cref="ParallelExecutionSettings" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single <see cref="RowInterval" />.</param>
    public static void IterateRowIntervals<T>(
        Rectangle rectangle,
        in ParallelExecutionSettings parallelSettings,
        in T operation)
        where T : struct, IRowIntervalOperation
    {
        ValidateRectangle(rectangle);

        var top = rectangle.Top;
        var bottom = rectangle.Bottom;
        var width = rectangle.Width;
        var height = rectangle.Height;

        var maxSteps = DivideCeil(width * height, parallelSettings.MinimumPixelsProcessedPerTask);
        var numOfSteps = Math.Min(parallelSettings.MaxDegreeOfParallelism, maxSteps);

        // Avoid TPL overhead in this trivial case:
        if (numOfSteps == 1)
        {
            var rows = new RowInterval(top, bottom);
            Unsafe.AsRef(in operation).Invoke(in rows);
            return;
        }

        var verticalStep = DivideCeil(rectangle.Height, numOfSteps);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = numOfSteps };
        var wrappingOperation = new RowIntervalOperationWrapper<T>(top, bottom, verticalStep, in operation);

        Parallel.For(
            0,
            numOfSteps,
            parallelOptions,
            wrappingOperation.Invoke);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches defined by <see cref="RowInterval" />-s
    ///     instantiating a temporary buffer for each <paramref name="operation" /> invocation.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <typeparam name="TBuffer">The type of buffer elements.</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /> to get the parallel settings from.</param>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single <see cref="RowInterval" />.</param>
    public static void IterateRowIntervals<T, TBuffer>(Configuration configuration, Rectangle rectangle, in T operation)
        where T : struct, IRowIntervalOperation<TBuffer>
        where TBuffer : unmanaged
    {
        var parallelSettings = ParallelExecutionSettings.FromConfiguration(configuration);
        IterateRowIntervals<T, TBuffer>(rectangle, in parallelSettings, in operation);
    }

    /// <summary>
    ///     Iterate through the rows of a rectangle in optimized batches defined by <see cref="RowInterval" />-s
    ///     instantiating a temporary buffer for each <paramref name="operation" /> invocation.
    /// </summary>
    /// <typeparam name="T">The type of row operation to perform.</typeparam>
    /// <typeparam name="TBuffer">The type of buffer elements.</typeparam>
    /// <param name="rectangle">The <see cref="Rectangle" />.</param>
    /// <param name="parallelSettings">The <see cref="ParallelExecutionSettings" />.</param>
    /// <param name="operation">The operation defining the iteration logic on a single <see cref="RowInterval" />.</param>
    public static void IterateRowIntervals<T, TBuffer>(
        Rectangle rectangle,
        in ParallelExecutionSettings parallelSettings,
        in T operation)
        where T : struct, IRowIntervalOperation<TBuffer>
        where TBuffer : unmanaged
    {
        ValidateRectangle(rectangle);

        var top = rectangle.Top;
        var bottom = rectangle.Bottom;
        var width = rectangle.Width;
        var height = rectangle.Height;

        var maxSteps = DivideCeil(width * height, parallelSettings.MinimumPixelsProcessedPerTask);
        var numOfSteps = Math.Min(parallelSettings.MaxDegreeOfParallelism, maxSteps);
        var allocator = parallelSettings.MemoryAllocator;

        // Avoid TPL overhead in this trivial case:
        if (numOfSteps == 1)
        {
            var rows = new RowInterval(top, bottom);
            using var buffer = allocator.Allocate<TBuffer>(width);

            Unsafe.AsRef(in operation).Invoke(in rows, buffer.Memory.Span);

            return;
        }

        var verticalStep = DivideCeil(height, numOfSteps);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = numOfSteps };
        var wrappingOperation =
            new RowIntervalOperationWrapper<T, TBuffer>(top, bottom, verticalStep, width, allocator, in operation);

        Parallel.For(
            0,
            numOfSteps,
            parallelOptions,
            wrappingOperation.Invoke);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static int DivideCeil(int dividend, int divisor)
    {
        return 1 + (dividend - 1) / divisor;
    }

    private static void ValidateRectangle(Rectangle rectangle)
    {
        Guard.MustBeGreaterThan(
            rectangle.Width,
            0,
            $"{nameof(rectangle)}.{nameof(rectangle.Width)}");

        Guard.MustBeGreaterThan(
            rectangle.Height,
            0,
            $"{nameof(rectangle)}.{nameof(rectangle.Height)}");
    }
}