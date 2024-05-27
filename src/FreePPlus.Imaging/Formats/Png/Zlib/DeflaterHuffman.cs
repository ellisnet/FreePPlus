// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png.Zlib;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Zlib;

/// <summary>
///     Performs Deflate Huffman encoding.
/// </summary>
internal sealed unsafe class DeflaterHuffman : IDisposable
{
    private const int BufferSize = 1 << (DeflaterConstants.DEFAULT_MEM_LEVEL + 6);

    // The number of literal codes.
    private const int LiteralNumber = 286;

    // Number of distance codes
    private const int DistanceNumber = 30;

    // Number of codes used to transfer bit lengths
    private const int BitLengthNumber = 19;

    // Repeat previous bit length 3-6 times (2 bits of repeat count)
    private const int Repeat3To6 = 16;

    // Repeat a zero length 3-10 times  (3 bits of repeat count)
    private const int Repeat3To10 = 17;

    // Repeat a zero length 11-138 times  (7 bits of repeat count)
    private const int Repeat11To138 = 18;

    private const int EofSymbol = 256;

    // Buffer for distances
    private readonly IMemoryOwner<short> distanceManagedBuffer;

    private readonly IMemoryOwner<short> literalManagedBuffer;
    private readonly short* pinnedDistanceBuffer;
    private readonly short* pinnedLiteralBuffer;
    private Tree blTree;
    private MemoryHandle distanceBufferHandle;
    private Tree distTree;
    private int extraBits;
    private bool isDisposed;

    private int lastLiteral;
    private MemoryHandle literalBufferHandle;

    private Tree literalTree;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeflaterHuffman" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The memory allocator to use for buffer allocations.</param>
    public DeflaterHuffman(MemoryAllocator memoryAllocator)
    {
        Pending = new DeflaterPendingBuffer(memoryAllocator);

        literalTree = new Tree(memoryAllocator, LiteralNumber, 257, 15);
        distTree = new Tree(memoryAllocator, DistanceNumber, 1, 15);
        blTree = new Tree(memoryAllocator, BitLengthNumber, 4, 7);

        distanceManagedBuffer = memoryAllocator.Allocate<short>(BufferSize);
        distanceBufferHandle = distanceManagedBuffer.Memory.Pin();
        pinnedDistanceBuffer = (short*)distanceBufferHandle.Pointer;

        literalManagedBuffer = memoryAllocator.Allocate<short>(BufferSize);
        literalBufferHandle = literalManagedBuffer.Memory.Pin();
        pinnedLiteralBuffer = (short*)literalBufferHandle.Pointer;
    }

    /// <summary>
    ///     Gets the lengths of the bit length codes are sent in order of decreasing probability, to avoid transmitting the
    ///     lengths for unused bit length codes.
    /// </summary>
    private static ReadOnlySpan<byte> BitLengthOrder => new byte[]
    {
        16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
    };

    private static ReadOnlySpan<byte> Bit4Reverse => new byte[]
    {
        0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15
    };

    /// <summary>
    ///     Gets the pending buffer to use.
    /// </summary>
    public DeflaterPendingBuffer Pending { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!isDisposed)
        {
            Pending.Dispose();
            distanceBufferHandle.Dispose();
            distanceManagedBuffer.Dispose();
            literalBufferHandle.Dispose();
            literalManagedBuffer.Dispose();

            literalTree.Dispose();
            blTree.Dispose();
            distTree.Dispose();

            Pending = null;
            literalTree = null;
            blTree = null;
            distTree = null;
            isDisposed = true;
        }
    }

    /// <summary>
    ///     Reset internal state
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Reset()
    {
        lastLiteral = 0;
        extraBits = 0;
        literalTree.Reset();
        distTree.Reset();
        blTree.Reset();
    }

    /// <summary>
    ///     Write all trees to pending buffer
    /// </summary>
    /// <param name="blTreeCodes">The number/rank of treecodes to send.</param>
    public void SendAllTrees(int blTreeCodes)
    {
        blTree.BuildCodes();
        literalTree.BuildCodes();
        distTree.BuildCodes();
        Pending.WriteBits(literalTree.NumCodes - 257, 5);
        Pending.WriteBits(distTree.NumCodes - 1, 5);
        Pending.WriteBits(blTreeCodes - 4, 4);

        for (var rank = 0; rank < blTreeCodes; rank++) Pending.WriteBits(blTree.Length[BitLengthOrder[rank]], 3);

        literalTree.WriteTree(Pending, blTree);
        distTree.WriteTree(Pending, blTree);
    }

    /// <summary>
    ///     Compress current buffer writing data to pending buffer
    /// </summary>
    public void CompressBlock()
    {
        var pendingBuffer = Pending;
        var pinnedDistance = pinnedDistanceBuffer;
        var pinnedLiteral = pinnedLiteralBuffer;

        for (var i = 0; i < lastLiteral; i++)
        {
            var litlen = pinnedLiteral[i] & 0xFF;
            int dist = pinnedDistance[i];
            if (dist-- != 0)
            {
                var lc = Lcode(litlen);
                literalTree.WriteSymbol(pendingBuffer, lc);

                var bits = (lc - 261) / 4;
                if (bits > 0 && bits <= 5) Pending.WriteBits(litlen & ((1 << bits) - 1), bits);

                var dc = Dcode(dist);
                distTree.WriteSymbol(pendingBuffer, dc);

                bits = (dc >> 1) - 1;
                if (bits > 0) Pending.WriteBits(dist & ((1 << bits) - 1), bits);
            }
            else
            {
                literalTree.WriteSymbol(pendingBuffer, litlen);
            }
        }

        literalTree.WriteSymbol(pendingBuffer, EofSymbol);
    }

    /// <summary>
    ///     Flush block to output with no compression
    /// </summary>
    /// <param name="stored">Data to write</param>
    /// <param name="storedOffset">Index of first byte to write</param>
    /// <param name="storedLength">Count of bytes to write</param>
    /// <param name="lastBlock">True if this is the last block</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
    {
        Pending.WriteBits((DeflaterConstants.STORED_BLOCK << 1) + (lastBlock ? 1 : 0), 3);
        Pending.AlignToByte();
        Pending.WriteShort(storedLength);
        Pending.WriteShort(~storedLength);
        Pending.WriteBlock(stored, storedOffset, storedLength);
        Reset();
    }

    /// <summary>
    ///     Flush block to output with compression
    /// </summary>
    /// <param name="stored">Data to flush</param>
    /// <param name="storedOffset">Index of first byte to flush</param>
    /// <param name="storedLength">Count of bytes to flush</param>
    /// <param name="lastBlock">True if this is the last block</param>
    public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
    {
        literalTree.Frequencies[EofSymbol]++;

        // Build trees
        literalTree.BuildTree();
        distTree.BuildTree();

        // Calculate bitlen frequency
        literalTree.CalcBLFreq(blTree);
        distTree.CalcBLFreq(blTree);

        // Build bitlen tree
        blTree.BuildTree();

        var blTreeCodes = 4;

        for (var i = 18; i > blTreeCodes; i--)
            if (blTree.Length[BitLengthOrder[i]] > 0)
                blTreeCodes = i + 1;

        var opt_len = 14 + blTreeCodes * 3 + blTree.GetEncodedLength()
                      + literalTree.GetEncodedLength() + distTree.GetEncodedLength()
                      + extraBits;

        var static_len = extraBits;
        ref var staticLLengthRef = ref MemoryMarshal.GetReference(StaticLLength);
        for (var i = 0; i < LiteralNumber; i++)
            static_len += literalTree.Frequencies[i] * Unsafe.Add(ref staticLLengthRef, i);

        ref var staticDLengthRef = ref MemoryMarshal.GetReference(StaticDLength);
        for (var i = 0; i < DistanceNumber; i++)
            static_len += distTree.Frequencies[i] * Unsafe.Add(ref staticDLengthRef, i);

        if (opt_len >= static_len)
            // Force static trees
            opt_len = static_len;

        if (storedOffset >= 0 && storedLength + 4 < opt_len >> 3)
        {
            // Store Block
            FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
        }
        else if (opt_len == static_len)
        {
            // Encode with static tree
            Pending.WriteBits((DeflaterConstants.STATIC_TREES << 1) + (lastBlock ? 1 : 0), 3);
            literalTree.SetStaticCodes(StaticLCodes, StaticLLength);
            distTree.SetStaticCodes(StaticDCodes, StaticDLength);
            CompressBlock();
            Reset();
        }
        else
        {
            // Encode with dynamic tree
            Pending.WriteBits((DeflaterConstants.DYN_TREES << 1) + (lastBlock ? 1 : 0), 3);
            SendAllTrees(blTreeCodes);
            CompressBlock();
            Reset();
        }
    }

    /// <summary>
    ///     Get value indicating if internal buffer is full
    /// </summary>
    /// <returns>true if buffer is full</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsFull()
    {
        return lastLiteral >= BufferSize;
    }

    /// <summary>
    ///     Add literal to buffer
    /// </summary>
    /// <param name="literal">Literal value to add to buffer.</param>
    /// <returns>Value indicating internal buffer is full</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool TallyLit(int literal)
    {
        pinnedDistanceBuffer[lastLiteral] = 0;
        pinnedLiteralBuffer[lastLiteral++] = (byte)literal;
        literalTree.Frequencies[literal]++;
        return IsFull();
    }

    /// <summary>
    ///     Add distance code and length to literal and distance trees
    /// </summary>
    /// <param name="distance">Distance code</param>
    /// <param name="length">Length</param>
    /// <returns>Value indicating if internal buffer is full</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool TallyDist(int distance, int length)
    {
        pinnedDistanceBuffer[lastLiteral] = (short)distance;
        pinnedLiteralBuffer[lastLiteral++] = (byte)(length - 3);

        var lc = Lcode(length - 3);
        literalTree.Frequencies[lc]++;
        if (lc >= 265 && lc < 285) extraBits += (lc - 261) / 4;

        var dc = Dcode(distance - 1);
        distTree.Frequencies[dc]++;
        if (dc >= 4) extraBits += (dc >> 1) - 1;

        return IsFull();
    }

    /// <summary>
    ///     Reverse the bits of a 16 bit value.
    /// </summary>
    /// <param name="toReverse">Value to reverse bits</param>
    /// <returns>Value with bits reversed</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static short BitReverse(int toReverse)
    {
        /* Use unsafe offsetting and manually validate the input index to reduce the
         * total number of conditional branches. There are two main cases to test here:
         *   1. In the first 3, the input value (or some combination of it) is combined
         *      with & 0xF, which results in a maximum value of 0xF no matter what the
         *      input value was. That is 15, which is always in range for the target span.
         *      As a result, no input validation is needed at all in this case.
         *   2. There are two cases where the input value might cause an invalid access:
         *      when it is either negative, or greater than 15 << 12. We can test both
         *      conditions in a single pass by casting the input value to uint and right
         *      shifting it by 12, which also preserves the sign. If it is a negative
         *      value (2-complement), the test will fail as the uint cast will result
         *      in a much larger value. If the value was simply too high, the test will
         *      fail as expected. We can't simply check whether the value is lower than
         *      15 << 12, because higher values are acceptable in the first 3 accesses.
         * Doing this reduces the total number of index checks from 4 down to just 1. */
        var toReverseRightShiftBy12 = toReverse >> 12;
        Guard.MustBeLessThanOrEqualTo<uint>((uint)toReverseRightShiftBy12, 15, nameof(toReverse));

        ref var bit4ReverseRef = ref MemoryMarshal.GetReference(Bit4Reverse);

        return (short)((Unsafe.Add(ref bit4ReverseRef, toReverse & 0xF) << 12)
                       | (Unsafe.Add(ref bit4ReverseRef, (toReverse >> 4) & 0xF) << 8)
                       | (Unsafe.Add(ref bit4ReverseRef, (toReverse >> 8) & 0xF) << 4)
                       | Unsafe.Add(ref bit4ReverseRef, toReverseRightShiftBy12));
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static int Lcode(int length)
    {
        if (length == 255) return 285;

        var code = 257;
        while (length >= 8)
        {
            code += 4;
            length >>= 1;
        }

        return code + length;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static int Dcode(int distance)
    {
        var code = 0;
        while (distance >= 4)
        {
            code += 2;
            distance >>= 1;
        }

        return code + distance;
    }

    private sealed class Tree : IDisposable
    {
        private readonly int[] bitLengthCounts;
        private readonly short* codes;

        private readonly int elementCount;
        private readonly int maxLength;

        private readonly MemoryAllocator memoryAllocator;
        private readonly int minNumCodes;
        private MemoryHandle codesMemoryHandle;

        private IMemoryOwner<short> codesMemoryOwner;
        private MemoryHandle frequenciesMemoryHandle;

        private IMemoryOwner<short> frequenciesMemoryOwner;
        private bool isDisposed;
        private MemoryHandle lengthsMemoryHandle;

        private IManagedByteBuffer lengthsMemoryOwner;

        public Tree(MemoryAllocator memoryAllocator, int elements, int minCodes, int maxLength)
        {
            this.memoryAllocator = memoryAllocator;
            elementCount = elements;
            minNumCodes = minCodes;
            this.maxLength = maxLength;

            frequenciesMemoryOwner = memoryAllocator.Allocate<short>(elements);
            frequenciesMemoryHandle = frequenciesMemoryOwner.Memory.Pin();
            Frequencies = (short*)frequenciesMemoryHandle.Pointer;

            lengthsMemoryOwner = memoryAllocator.AllocateManagedByteBuffer(elements);
            lengthsMemoryHandle = lengthsMemoryOwner.Memory.Pin();
            Length = (byte*)lengthsMemoryHandle.Pointer;

            codesMemoryOwner = memoryAllocator.Allocate<short>(elements);
            codesMemoryHandle = codesMemoryOwner.Memory.Pin();
            codes = (short*)codesMemoryHandle.Pointer;

            // Maxes out at 15.
            bitLengthCounts = new int[maxLength];
        }

        public int NumCodes { get; private set; }

        public short* Frequencies { get; }

        public byte* Length { get; }

        public void Dispose()
        {
            if (!isDisposed)
            {
                frequenciesMemoryHandle.Dispose();
                frequenciesMemoryOwner.Dispose();

                lengthsMemoryHandle.Dispose();
                lengthsMemoryOwner.Dispose();

                codesMemoryHandle.Dispose();
                codesMemoryOwner.Dispose();

                frequenciesMemoryOwner = null;
                lengthsMemoryOwner = null;
                codesMemoryOwner = null;

                isDisposed = true;
            }
        }

        /// <summary>
        ///     Resets the internal state of the tree
        /// </summary>
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Reset()
        {
            frequenciesMemoryOwner.Memory.Span.Clear();
            lengthsMemoryOwner.Memory.Span.Clear();
            codesMemoryOwner.Memory.Span.Clear();
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void WriteSymbol(DeflaterPendingBuffer pendingBuffer, int code)
        {
            pendingBuffer.WriteBits(codes[code] & 0xFFFF, Length[code]);
        }

        /// <summary>
        ///     Set static codes and length
        /// </summary>
        /// <param name="staticCodes">new codes</param>
        /// <param name="staticLengths">length for new codes</param>
        [MethodImpl(InliningOptions.ShortMethod)]
        public void SetStaticCodes(ReadOnlySpan<short> staticCodes, ReadOnlySpan<byte> staticLengths)
        {
            staticCodes.CopyTo(codesMemoryOwner.Memory.Span);
            staticLengths.CopyTo(lengthsMemoryOwner.Memory.Span);
        }

        /// <summary>
        ///     Build dynamic codes and lengths
        /// </summary>
        public void BuildCodes()
        {
            // Maxes out at 15 * 4
            Span<int> nextCode = stackalloc int[maxLength];
            ref var nextCodeRef = ref MemoryMarshal.GetReference(nextCode);
            ref var bitLengthCountsRef = ref MemoryMarshal.GetReference<int>(bitLengthCounts);

            var code = 0;
            for (var bits = 0; bits < maxLength; bits++)
            {
                Unsafe.Add(ref nextCodeRef, bits) = code;
                code += Unsafe.Add(ref bitLengthCountsRef, bits) << (15 - bits);
            }

            for (var i = 0; i < NumCodes; i++)
            {
                int bits = Length[i];
                if (bits > 0)
                {
                    codes[i] = BitReverse(Unsafe.Add(ref nextCodeRef, bits - 1));
                    Unsafe.Add(ref nextCodeRef, bits - 1) += 1 << (16 - bits);
                }
            }
        }

        [MethodImpl(InliningOptions.HotPath)]
        public void BuildTree()
        {
            var numSymbols = elementCount;

            // heap is a priority queue, sorted by frequency, least frequent
            // nodes first.  The heap is a binary tree, with the property, that
            // the parent node is smaller than both child nodes.  This assures
            // that the smallest node is the first parent.
            //
            // The binary tree is encoded in an array:  0 is root node and
            // the nodes 2*n+1, 2*n+2 are the child nodes of node n.
            // Maxes out at 286 * 4 so too large for the stack.
            using (var heapMemoryOwner = memoryAllocator.Allocate<int>(numSymbols))
            {
                ref var heapRef = ref MemoryMarshal.GetReference(heapMemoryOwner.Memory.Span);

                var heapLen = 0;
                var maxCode = 0;
                for (var n = 0; n < numSymbols; n++)
                {
                    int freq = Frequencies[n];
                    if (freq != 0)
                    {
                        // Insert n into heap
                        var pos = heapLen++;
                        int ppos;
                        while (pos > 0 && Frequencies[Unsafe.Add(ref heapRef, ppos = (pos - 1) >> 1)] > freq)
                        {
                            Unsafe.Add(ref heapRef, pos) = Unsafe.Add(ref heapRef, ppos);
                            pos = ppos;
                        }

                        Unsafe.Add(ref heapRef, pos) = n;

                        maxCode = n;
                    }
                }

                // We could encode a single literal with 0 bits but then we
                // don't see the literals.  Therefore we force at least two
                // literals to avoid this case.  We don't care about order in
                // this case, both literals get a 1 bit code.
                while (heapLen < 2) Unsafe.Add(ref heapRef, heapLen++) = maxCode < 2 ? ++maxCode : 0;

                NumCodes = Math.Max(maxCode + 1, minNumCodes);

                var numLeafs = heapLen;
                var childrenLength = 4 * heapLen - 2;
                using (var childrenMemoryOwner = memoryAllocator.Allocate<int>(childrenLength))
                using (var valuesMemoryOwner = memoryAllocator.Allocate<int>(2 * heapLen - 1))
                {
                    ref var childrenRef = ref MemoryMarshal.GetReference(childrenMemoryOwner.Memory.Span);
                    ref var valuesRef = ref MemoryMarshal.GetReference(valuesMemoryOwner.Memory.Span);
                    var numNodes = numLeafs;

                    for (var i = 0; i < heapLen; i++)
                    {
                        var node = Unsafe.Add(ref heapRef, i);
                        var i2 = 2 * i;
                        Unsafe.Add(ref childrenRef, i2) = node;
                        Unsafe.Add(ref childrenRef, i2 + 1) = -1;
                        Unsafe.Add(ref valuesRef, i) = Frequencies[node] << 8;
                        Unsafe.Add(ref heapRef, i) = i;
                    }

                    // Construct the Huffman tree by repeatedly combining the least two
                    // frequent nodes.
                    do
                    {
                        var first = Unsafe.Add(ref heapRef, 0);
                        var last = Unsafe.Add(ref heapRef, --heapLen);

                        // Propagate the hole to the leafs of the heap
                        var ppos = 0;
                        var path = 1;

                        while (path < heapLen)
                        {
                            if (path + 1 < heapLen && Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, path)) >
                                Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, path + 1))) path++;

                            Unsafe.Add(ref heapRef, ppos) = Unsafe.Add(ref heapRef, path);
                            ppos = path;
                            path = path * 2 + 1;
                        }

                        // Now propagate the last element down along path.  Normally
                        // it shouldn't go too deep.
                        var lastVal = Unsafe.Add(ref valuesRef, last);
                        while ((path = ppos) > 0
                               && Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, ppos = (path - 1) >> 1)) > lastVal)
                            Unsafe.Add(ref heapRef, path) = Unsafe.Add(ref heapRef, ppos);

                        Unsafe.Add(ref heapRef, path) = last;

                        var second = Unsafe.Add(ref heapRef, 0);

                        // Create a new node father of first and second
                        last = numNodes++;
                        Unsafe.Add(ref childrenRef, 2 * last) = first;
                        Unsafe.Add(ref childrenRef, 2 * last + 1) = second;
                        var mindepth = Math.Min(Unsafe.Add(ref valuesRef, first) & 0xFF,
                            Unsafe.Add(ref valuesRef, second) & 0xFF);
                        Unsafe.Add(ref valuesRef, last) = lastVal = Unsafe.Add(ref valuesRef, first) +
                            Unsafe.Add(ref valuesRef, second) - mindepth + 1;

                        // Again, propagate the hole to the leafs
                        ppos = 0;
                        path = 1;

                        while (path < heapLen)
                        {
                            if (path + 1 < heapLen
                                && Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, path)) >
                                Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, path + 1)))
                                path++;

                            Unsafe.Add(ref heapRef, ppos) = Unsafe.Add(ref heapRef, path);
                            ppos = path;
                            path = ppos * 2 + 1;
                        }

                        // Now propagate the new element down along path
                        while ((path = ppos) > 0 &&
                               Unsafe.Add(ref valuesRef, Unsafe.Add(ref heapRef, ppos = (path - 1) >> 1)) > lastVal)
                            Unsafe.Add(ref heapRef, path) = Unsafe.Add(ref heapRef, ppos);

                        Unsafe.Add(ref heapRef, path) = last;
                    } while (heapLen > 1);

                    if (Unsafe.Add(ref heapRef, 0) != (childrenLength >> 1) - 1) DeflateThrowHelper.ThrowHeapViolated();

                    BuildLength(childrenMemoryOwner.Memory.Span);
                }
            }
        }

        /// <summary>
        ///     Get encoded length
        /// </summary>
        /// <returns>Encoded length, the sum of frequencies * lengths</returns>
        [MethodImpl(InliningOptions.ShortMethod)]
        public int GetEncodedLength()
        {
            var len = 0;
            for (var i = 0; i < elementCount; i++) len += Frequencies[i] * Length[i];

            return len;
        }

        /// <summary>
        ///     Scan a literal or distance tree to determine the frequencies of the codes
        ///     in the bit length tree.
        /// </summary>
        public void CalcBLFreq(Tree blTree)
        {
            int maxCount; // max repeat count
            int minCount; // min repeat count
            int count; // repeat count of the current code
            var curLen = -1; // length of current code

            var i = 0;
            while (i < NumCodes)
            {
                count = 1;
                int nextlen = Length[i];
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else
                {
                    maxCount = 6;
                    minCount = 3;
                    if (curLen != nextlen)
                    {
                        blTree.Frequencies[nextlen]++;
                        count = 0;
                    }
                }

                curLen = nextlen;
                i++;

                while (i < NumCodes && curLen == Length[i])
                {
                    i++;
                    if (++count >= maxCount) break;
                }

                if (count < minCount)
                    blTree.Frequencies[curLen] += (short)count;
                else if (curLen != 0)
                    blTree.Frequencies[Repeat3To6]++;
                else if (count <= 10)
                    blTree.Frequencies[Repeat3To10]++;
                else
                    blTree.Frequencies[Repeat11To138]++;
            }
        }

        /// <summary>
        ///     Write the tree values.
        /// </summary>
        /// <param name="pendingBuffer">The pending buffer.</param>
        /// <param name="bitLengthTree">The tree to write.</param>
        public void WriteTree(DeflaterPendingBuffer pendingBuffer, Tree bitLengthTree)
        {
            int maxCount; // max repeat count
            int minCount; // min repeat count
            int count; // repeat count of the current code
            var curLen = -1; // length of current code

            var i = 0;
            while (i < NumCodes)
            {
                count = 1;
                int nextlen = Length[i];
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else
                {
                    maxCount = 6;
                    minCount = 3;
                    if (curLen != nextlen)
                    {
                        bitLengthTree.WriteSymbol(pendingBuffer, nextlen);
                        count = 0;
                    }
                }

                curLen = nextlen;
                i++;

                while (i < NumCodes && curLen == Length[i])
                {
                    i++;
                    if (++count >= maxCount) break;
                }

                if (count < minCount)
                {
                    while (count-- > 0) bitLengthTree.WriteSymbol(pendingBuffer, curLen);
                }
                else if (curLen != 0)
                {
                    bitLengthTree.WriteSymbol(pendingBuffer, Repeat3To6);
                    pendingBuffer.WriteBits(count - 3, 2);
                }
                else if (count <= 10)
                {
                    bitLengthTree.WriteSymbol(pendingBuffer, Repeat3To10);
                    pendingBuffer.WriteBits(count - 3, 3);
                }
                else
                {
                    bitLengthTree.WriteSymbol(pendingBuffer, Repeat11To138);
                    pendingBuffer.WriteBits(count - 11, 7);
                }
            }
        }

        private void BuildLength(ReadOnlySpan<int> children)
        {
            var lengthPtr = Length;
            ref var childrenRef = ref MemoryMarshal.GetReference(children);
            ref var bitLengthCountsRef = ref MemoryMarshal.GetReference<int>(bitLengthCounts);

            var maxLen = maxLength;
            var numNodes = children.Length >> 1;
            var numLeafs = (numNodes + 1) >> 1;
            var overflow = 0;

            Array.Clear(bitLengthCounts, 0, maxLen);

            // First calculate optimal bit lengths
            using (var lengthsMemoryOwner = memoryAllocator.Allocate<int>(numNodes, AllocationOptions.Clean))
            {
                ref var lengthsRef = ref MemoryMarshal.GetReference(lengthsMemoryOwner.Memory.Span);

                for (var i = numNodes - 1; i >= 0; i--)
                    if (children[2 * i + 1] != -1)
                    {
                        var bitLength = Unsafe.Add(ref lengthsRef, i) + 1;
                        if (bitLength > maxLen)
                        {
                            bitLength = maxLen;
                            overflow++;
                        }

                        Unsafe.Add(ref lengthsRef, Unsafe.Add(ref childrenRef, 2 * i)) =
                            Unsafe.Add(ref lengthsRef, Unsafe.Add(ref childrenRef, 2 * i + 1)) = bitLength;
                    }
                    else
                    {
                        // A leaf node
                        var bitLength = Unsafe.Add(ref lengthsRef, i);
                        Unsafe.Add(ref bitLengthCountsRef, bitLength - 1)++;
                        lengthPtr[Unsafe.Add(ref childrenRef, 2 * i)] = (byte)Unsafe.Add(ref lengthsRef, i);
                    }
            }

            if (overflow == 0) return;

            var incrBitLen = maxLen - 1;
            do
            {
                // Find the first bit length which could increase:
                while (Unsafe.Add(ref bitLengthCountsRef, --incrBitLen) == 0) { }

                // Move this node one down and remove a corresponding
                // number of overflow nodes.
                do
                {
                    Unsafe.Add(ref bitLengthCountsRef, incrBitLen)--;
                    Unsafe.Add(ref bitLengthCountsRef, ++incrBitLen)++;
                    overflow -= 1 << (maxLen - 1 - incrBitLen);
                } while (overflow > 0 && incrBitLen < maxLen - 1);
            } while (overflow > 0);

            // We may have overshot above.  Move some nodes from maxLength to
            // maxLength-1 in that case.
            Unsafe.Add(ref bitLengthCountsRef, maxLen - 1) += overflow;
            Unsafe.Add(ref bitLengthCountsRef, maxLen - 2) -= overflow;

            // Now recompute all bit lengths, scanning in increasing
            // frequency.  It is simpler to reconstruct all lengths instead of
            // fixing only the wrong ones. This idea is taken from 'ar'
            // written by Haruhiko Okumura.
            //
            // The nodes were inserted with decreasing frequency into the childs
            // array.
            var nodeIndex = 2 * numLeafs;
            for (var bits = maxLen; bits != 0; bits--)
            {
                var n = Unsafe.Add(ref bitLengthCountsRef, bits - 1);
                while (n > 0)
                {
                    var childIndex = 2 * Unsafe.Add(ref childrenRef, nodeIndex++);
                    if (Unsafe.Add(ref childrenRef, childIndex + 1) == -1)
                    {
                        // We found another leaf
                        lengthPtr[Unsafe.Add(ref childrenRef, childIndex)] = (byte)bits;
                        n--;
                    }
                }
            }
        }
    }

#pragma warning disable SA1201 // Elements should appear in the correct order

    // See RFC 1951 3.2.6
    // Literal codes
    private static readonly short[] StaticLCodes =
    {
        12, 140, 76, 204, 44, 172, 108, 236, 28, 156, 92, 220, 60, 188, 124, 252,
        2, 130, 66, 194, 34, 162, 98, 226, 18, 146, 82, 210, 50, 178, 114, 242,
        10, 138, 74, 202, 42, 170, 106, 234, 26, 154, 90, 218, 58, 186, 122, 250,
        6, 134, 70, 198, 38, 166, 102, 230, 22, 150, 86, 214, 54, 182, 118, 246,
        14, 142, 78, 206, 46, 174, 110, 238, 30, 158, 94, 222, 62, 190, 126, 254,
        1, 129, 65, 193, 33, 161, 97, 225, 17, 145, 81, 209, 49, 177, 113, 241, 9,
        137, 73, 201, 41, 169, 105, 233, 25, 153, 89, 217, 57, 185, 121, 249, 5,
        133, 69, 197, 37, 165, 101, 229, 21, 149, 85, 213, 53, 181, 117, 245, 13,
        141, 77, 205, 45, 173, 109, 237, 29, 157, 93, 221, 61, 189, 125, 253, 19,
        275, 147, 403, 83, 339, 211, 467, 51, 307, 179, 435, 115, 371, 243, 499,
        11, 267, 139, 395, 75, 331, 203, 459, 43, 299, 171, 427, 107, 363, 235, 491,
        27, 283, 155, 411, 91, 347, 219, 475, 59, 315, 187, 443, 123, 379, 251, 507,
        7, 263, 135, 391, 71, 327, 199, 455, 39, 295, 167, 423, 103, 359, 231, 487,
        23, 279, 151, 407, 87, 343, 215, 471, 55, 311, 183, 439, 119, 375, 247, 503,
        15, 271, 143, 399, 79, 335, 207, 463, 47, 303, 175, 431, 111, 367, 239, 495,
        31, 287, 159, 415, 95, 351, 223, 479, 63, 319, 191, 447, 127, 383, 255, 511,
        0, 64, 32, 96, 16, 80, 48, 112, 8, 72, 40, 104, 24, 88, 56, 120, 4, 68, 36,
        100, 20, 84, 52, 116, 3, 131, 67, 195, 35, 163
    };

    private static ReadOnlySpan<byte> StaticLLength => new byte[]
    {
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
        7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
        7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8
    };

    // Distance codes and lengths.
    private static readonly short[] StaticDCodes =
    {
        0, 16, 8, 24, 4, 20, 12, 28, 2, 18, 10, 26, 6, 22, 14,
        30, 1, 17, 9, 25, 5, 21, 13, 29, 3, 19, 11, 27, 7, 23
    };

    private static ReadOnlySpan<byte> StaticDLength => new byte[]
    {
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5
    };
#pragma warning restore SA1201 // Elements should appear in the correct order
}