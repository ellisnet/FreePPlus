// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

internal abstract partial class MemoryGroup<T>
{
    /// <summary>
    ///     A <see cref="MemoryGroup{T}" /> implementation that owns the underlying memory buffers.
    /// </summary>
    public sealed class Owned : MemoryGroup<T>, IEnumerable<Memory<T>>
    {
        private IMemoryOwner<T>[] memoryOwners;

        public Owned(IMemoryOwner<T>[] memoryOwners, int bufferLength, long totalLength, bool swappable)
            : base(bufferLength, totalLength)
        {
            this.memoryOwners = memoryOwners;
            Swappable = swappable;
            View = new MemoryGroupView<T>(this);
        }

        public bool Swappable { get; }

        private bool IsDisposed => memoryOwners == null;

        public override int Count
        {
            [MethodImpl(InliningOptions.ShortMethod)]
            get
            {
                EnsureNotDisposed();
                return memoryOwners.Length;
            }
        }

        public override Memory<T> this[int index]
        {
            get
            {
                EnsureNotDisposed();
                return memoryOwners[index].Memory;
            }
        }

        /// <inheritdoc />
        IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
        {
            EnsureNotDisposed();
            return memoryOwners.Select(mo => mo.Memory).GetEnumerator();
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public override MemoryGroupEnumerator<T> GetEnumerator()
        {
            return new MemoryGroupEnumerator<T>(this);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;

            View.Invalidate();

            foreach (var memoryOwner in memoryOwners) memoryOwner.Dispose();

            memoryOwners = null;
            IsValid = false;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        private void EnsureNotDisposed()
        {
            if (memoryOwners is null) ThrowObjectDisposedException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(nameof(MemoryGroup<T>));
        }

        internal static void SwapContents(Owned a, Owned b)
        {
            a.EnsureNotDisposed();
            b.EnsureNotDisposed();

            var tempOwners = a.memoryOwners;
            var tempTotalLength = a.TotalLength;
            var tempBufferLength = a.BufferLength;

            a.memoryOwners = b.memoryOwners;
            a.TotalLength = b.TotalLength;
            a.BufferLength = b.BufferLength;

            b.memoryOwners = tempOwners;
            b.TotalLength = tempTotalLength;
            b.BufferLength = tempBufferLength;

            a.View.Invalidate();
            b.View.Invalidate();
            a.View = new MemoryGroupView<T>(a);
            b.View = new MemoryGroupView<T>(b);
        }
    }
}