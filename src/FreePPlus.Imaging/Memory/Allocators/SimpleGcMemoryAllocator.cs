// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using FreePPlus.Imaging.Memory.Internals;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Implements <see cref="MemoryAllocator" /> by newing up managed arrays on every allocation request.
/// </summary>
public sealed class SimpleGcMemoryAllocator : MemoryAllocator
{
    /// <inheritdoc />
    protected internal override int GetBufferCapacityInBytes()
    {
        return int.MaxValue;
    }

    /// <inheritdoc />
    public override IMemoryOwner<T> Allocate<T>(int length, AllocationOptions options = AllocationOptions.None)
    {
        Guard.MustBeGreaterThanOrEqualTo(length, 0, nameof(length));

        return new BasicArrayBuffer<T>(new T[length]);
    }

    /// <inheritdoc />
    public override IManagedByteBuffer AllocateManagedByteBuffer(int length,
        AllocationOptions options = AllocationOptions.None)
    {
        Guard.MustBeGreaterThanOrEqualTo(length, 0, nameof(length));

        return new BasicByteBuffer(new byte[length]);
    }
}