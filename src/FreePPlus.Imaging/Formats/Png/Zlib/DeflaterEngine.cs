// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png.Zlib;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Zlib;

/// <summary>
///     Strategies for deflater
/// </summary>
internal enum DeflateStrategy
{
    /// <summary>
    ///     The default strategy
    /// </summary>
    Default = 0,

    /// <summary>
    ///     This strategy will only allow longer string repetitions.  It is
    ///     useful for random data with a small character set.
    /// </summary>
    Filtered = 1,

    /// <summary>
    ///     This strategy will not look for string repetitions at all.  It
    ///     only encodes with Huffman trees (which means, that more common
    ///     characters get a smaller encoding.
    /// </summary>
    HuffmanOnly = 2
}

// DEFLATE ALGORITHM:
//
// The uncompressed stream is inserted into the window array.  When
// the window array is full the first half is thrown away and the
// second half is copied to the beginning.
//
// The head array is a hash table.  Three characters build a hash value
// and they the value points to the corresponding index in window of
// the last string with this hash.  The prev array implements a
// linked list of matches with the same hash: prev[index & WMASK] points
// to the previous index with the same hash.
//

/// <summary>
///     Low level compression engine for deflate algorithm which uses a 32K sliding window
///     with secondary compression from Huffman/Shannon-Fano codes.
/// </summary>
internal sealed unsafe class DeflaterEngine : IDisposable
{
    private const int TooFar = 4096;
    private readonly Memory<short> head;
    private readonly short* pinnedHeadPointer;
    private readonly short* pinnedPrevPointer;
    private readonly byte* pinnedWindowPointer;
    private readonly Memory<short> prev;

    private readonly DeflateStrategy strategy;
    private readonly byte[] window;

    private int blockStart;

    /// <summary>
    ///     The current compression function.
    /// </summary>
    private int compressionFunction;

    private int goodLength;
    private MemoryHandle headMemoryHandle;

    /// <summary>
    ///     Hashtable, hashing three characters to an index for window, so
    ///     that window[index]..window[index+2] have this hash code.
    ///     Note that the array should really be unsigned short, so you need
    ///     to and the values with 0xFFFF.
    /// </summary>
    private IMemoryOwner<short> headMemoryOwner;

    private DeflaterHuffman huffman;

    /// <summary>
    ///     The input data for compression.
    /// </summary>
    private byte[] inputBuf;

    /// <summary>
    ///     The end offset of the input data.
    /// </summary>
    private int inputEnd;

    /// <summary>
    ///     The offset into inputBuf, where input data starts.
    /// </summary>
    private int inputOff;

    // Hash index of string to be inserted
    private int insertHashIndex;
    private bool isDisposed;

    /// <summary>
    ///     lookahead is the number of characters starting at strstart in
    ///     window that are valid.
    ///     So window[strstart] until window[strstart+lookahead-1] are valid
    ///     characters.
    /// </summary>
    private int lookahead;

    // Length of best match
    private int matchLen;

    private int matchStart;

    private int maxChain;
    private int maxLazy;
    private int niceLength;

    // Set if previous match exists
    private bool prevAvailable;
    private MemoryHandle prevMemoryHandle;

    /// <summary>
    ///     <code>prev[index &amp; WMASK]</code> points to the previous index that has the
    ///     same hash code as the string starting at index.  This way
    ///     entries with the same hash code are in a linked list.
    ///     Note that the array should really be unsigned short, so you need
    ///     to and the values with 0xFFFF.
    /// </summary>
    private IMemoryOwner<short> prevMemoryOwner;

    /// <summary>
    ///     Points to the current character in the window.
    /// </summary>
    private int strstart;

    private MemoryHandle windowMemoryHandle;

    /// <summary>
    ///     This array contains the part of the uncompressed stream that
    ///     is of relevance. The current character is indexed by strstart.
    /// </summary>
    private IManagedByteBuffer windowMemoryOwner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeflaterEngine" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The memory allocator to use for buffer allocations.</param>
    /// <param name="strategy">The deflate strategy to use.</param>
    public DeflaterEngine(MemoryAllocator memoryAllocator, DeflateStrategy strategy)
    {
        huffman = new DeflaterHuffman(memoryAllocator);
        Pending = huffman.Pending;
        this.strategy = strategy;

        // Create pinned pointers to the various buffers to allow indexing
        // without bounds checks.
        windowMemoryOwner = memoryAllocator.AllocateManagedByteBuffer(2 * DeflaterConstants.WSIZE);
        window = windowMemoryOwner.Array;
        windowMemoryHandle = windowMemoryOwner.Memory.Pin();
        pinnedWindowPointer = (byte*)windowMemoryHandle.Pointer;

        headMemoryOwner = memoryAllocator.Allocate<short>(DeflaterConstants.HASH_SIZE);
        head = headMemoryOwner.Memory;
        headMemoryHandle = headMemoryOwner.Memory.Pin();
        pinnedHeadPointer = (short*)headMemoryHandle.Pointer;

        prevMemoryOwner = memoryAllocator.Allocate<short>(DeflaterConstants.WSIZE);
        prev = prevMemoryOwner.Memory;
        prevMemoryHandle = prevMemoryOwner.Memory.Pin();
        pinnedPrevPointer = (short*)prevMemoryHandle.Pointer;

        // We start at index 1, to avoid an implementation deficiency, that
        // we cannot build a repeat pattern at index 0.
        blockStart = strstart = 1;
    }

    /// <summary>
    ///     Gets the pending buffer to use.
    /// </summary>
    public DeflaterPendingBuffer Pending { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!isDisposed)
        {
            huffman.Dispose();

            windowMemoryHandle.Dispose();
            windowMemoryOwner.Dispose();

            headMemoryHandle.Dispose();
            headMemoryOwner.Dispose();

            prevMemoryHandle.Dispose();
            prevMemoryOwner.Dispose();

            windowMemoryOwner = null;
            headMemoryOwner = null;
            prevMemoryOwner = null;
            huffman = null;

            isDisposed = true;
        }
    }

    /// <summary>
    ///     Deflate drives actual compression of data
    /// </summary>
    /// <param name="flush">True to flush input buffers</param>
    /// <param name="finish">Finish deflation with the current input.</param>
    /// <returns>Returns true if progress has been made.</returns>
    public bool Deflate(bool flush, bool finish)
    {
        var progress = false;
        do
        {
            FillWindow();
            var canFlush = flush && inputOff == inputEnd;

            switch (compressionFunction)
            {
                case DeflaterConstants.DEFLATE_STORED:
                    progress = DeflateStored(canFlush, finish);
                    break;

                case DeflaterConstants.DEFLATE_FAST:
                    progress = DeflateFast(canFlush, finish);
                    break;

                case DeflaterConstants.DEFLATE_SLOW:
                    progress = DeflateSlow(canFlush, finish);
                    break;

                default:
                    DeflateThrowHelper.ThrowUnknownCompression();
                    break;
            }
        } while (Pending.IsFlushed && progress); // repeat while we have no pending output and progress was made

        return progress;
    }

    /// <summary>
    ///     Sets input data to be deflated.  Should only be called when <see cref="NeedsInput" />
    ///     returns true
    /// </summary>
    /// <param name="buffer">The buffer containing input data.</param>
    /// <param name="offset">The offset of the first byte of data.</param>
    /// <param name="count">The number of bytes of data to use as input.</param>
    public void SetInput(byte[] buffer, int offset, int count)
    {
        if (buffer is null) DeflateThrowHelper.ThrowNull(nameof(buffer));

        if (offset < 0) DeflateThrowHelper.ThrowOutOfRange(nameof(offset));

        if (count < 0) DeflateThrowHelper.ThrowOutOfRange(nameof(count));

        if (inputOff < inputEnd) DeflateThrowHelper.ThrowNotProcessed();

        var end = offset + count;

        // We want to throw an ArgumentOutOfRangeException early.
        // The check is very tricky: it also handles integer wrap around.
        if (offset > end || end > buffer.Length) DeflateThrowHelper.ThrowOutOfRange(nameof(count));

        inputBuf = buffer;
        inputOff = offset;
        inputEnd = end;
    }

    /// <summary>
    ///     Determines if more <see cref="SetInput">input</see> is needed.
    /// </summary>
    /// <returns>Return true if input is needed via <see cref="SetInput">SetInput</see></returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool NeedsInput()
    {
        return inputEnd == inputOff;
    }

    /// <summary>
    ///     Reset internal state
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Reset()
    {
        huffman.Reset();
        blockStart = strstart = 1;
        lookahead = 0;
        prevAvailable = false;
        matchLen = DeflaterConstants.MIN_MATCH - 1;
        head.Span.Slice(0, DeflaterConstants.HASH_SIZE).Clear();
        prev.Span.Slice(0, DeflaterConstants.WSIZE).Clear();
    }

    /// <summary>
    ///     Set the deflate level (0-9)
    /// </summary>
    /// <param name="level">The value to set the level to.</param>
    public void SetLevel(int level)
    {
        if (level < 0 || level > 9) DeflateThrowHelper.ThrowOutOfRange(nameof(level));

        goodLength = DeflaterConstants.GOOD_LENGTH[level];
        maxLazy = DeflaterConstants.MAX_LAZY[level];
        niceLength = DeflaterConstants.NICE_LENGTH[level];
        maxChain = DeflaterConstants.MAX_CHAIN[level];

        if (DeflaterConstants.COMPR_FUNC[level] != compressionFunction)
        {
            switch (compressionFunction)
            {
                case DeflaterConstants.DEFLATE_STORED:
                    if (strstart > blockStart)
                    {
                        huffman.FlushStoredBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }

                    UpdateHash();
                    break;

                case DeflaterConstants.DEFLATE_FAST:
                    if (strstart > blockStart)
                    {
                        huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }

                    break;

                case DeflaterConstants.DEFLATE_SLOW:
                    if (prevAvailable) huffman.TallyLit(pinnedWindowPointer[strstart - 1] & 0xFF);

                    if (strstart > blockStart)
                    {
                        huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }

                    prevAvailable = false;
                    matchLen = DeflaterConstants.MIN_MATCH - 1;
                    break;
            }

            compressionFunction = DeflaterConstants.COMPR_FUNC[level];
        }
    }

    /// <summary>
    ///     Fill the window
    /// </summary>
    public void FillWindow()
    {
        // If the window is almost full and there is insufficient lookahead,
        // move the upper half to the lower one to make room in the upper half.
        if (strstart >= DeflaterConstants.WSIZE + DeflaterConstants.MAX_DIST) SlideWindow();

        // If there is not enough lookahead, but still some input left, read in the input.
        if (lookahead < DeflaterConstants.MIN_LOOKAHEAD && inputOff < inputEnd)
        {
            var more = 2 * DeflaterConstants.WSIZE - lookahead - strstart;

            if (more > inputEnd - inputOff) more = inputEnd - inputOff;

            Buffer.BlockCopy(inputBuf, inputOff, window, strstart + lookahead, more);

            inputOff += more;
            lookahead += more;
        }

        if (lookahead >= DeflaterConstants.MIN_MATCH) UpdateHash();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private void UpdateHash()
    {
        var pinned = pinnedWindowPointer;
        insertHashIndex = (pinned[strstart] << DeflaterConstants.HASH_SHIFT) ^ pinned[strstart + 1];
    }

    /// <summary>
    ///     Inserts the current string in the head hash and returns the previous
    ///     value for this hash.
    /// </summary>
    /// <returns>The previous hash value</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private int InsertString()
    {
        short match;
        var hash = ((insertHashIndex << DeflaterConstants.HASH_SHIFT) ^
                    pinnedWindowPointer[strstart + (DeflaterConstants.MIN_MATCH - 1)]) & DeflaterConstants.HASH_MASK;

        var pinnedHead = pinnedHeadPointer;
        pinnedPrevPointer[strstart & DeflaterConstants.WMASK] = match = pinnedHead[hash];
        pinnedHead[hash] = unchecked((short)strstart);
        insertHashIndex = hash;
        return match & 0xFFFF;
    }

    private void SlideWindow()
    {
        Unsafe.CopyBlockUnaligned(ref window[0], ref window[DeflaterConstants.WSIZE], DeflaterConstants.WSIZE);
        matchStart -= DeflaterConstants.WSIZE;
        strstart -= DeflaterConstants.WSIZE;
        blockStart -= DeflaterConstants.WSIZE;

        // Slide the hash table (could be avoided with 32 bit values
        // at the expense of memory usage).
        var pinnedHead = pinnedHeadPointer;
        for (var i = 0; i < DeflaterConstants.HASH_SIZE; ++i)
        {
            var m = pinnedHead[i] & 0xFFFF;
            pinnedHead[i] = (short)(m >= DeflaterConstants.WSIZE ? m - DeflaterConstants.WSIZE : 0);
        }

        // Slide the prev table.
        var pinnedPrev = pinnedPrevPointer;
        for (var i = 0; i < DeflaterConstants.WSIZE; i++)
        {
            var m = pinnedPrev[i] & 0xFFFF;
            pinnedPrev[i] = (short)(m >= DeflaterConstants.WSIZE ? m - DeflaterConstants.WSIZE : 0);
        }
    }

    /// <summary>
    ///     <para>
    ///         Find the best (longest) string in the window matching the
    ///         string starting at strstart.
    ///     </para>
    ///     <para>
    ///         Preconditions:
    ///         <code>
    /// strstart + DeflaterConstants.MAX_MATCH &lt;= window.length.</code>
    ///     </para>
    /// </summary>
    /// <param name="curMatch">The current match.</param>
    /// <returns>True if a match greater than the minimum length is found</returns>
    [MethodImpl(InliningOptions.HotPath)]
    private bool FindLongestMatch(int curMatch)
    {
        int match;
        var scan = strstart;

        // scanMax is the highest position that we can look at
        var scanMax = scan + Math.Min(DeflaterConstants.MAX_MATCH, lookahead) - 1;
        var limit = Math.Max(scan - DeflaterConstants.MAX_DIST, 0);

        var chainLength = maxChain;
        var niceLength = Math.Min(this.niceLength, lookahead);

        var matchStrt = matchStart;
        matchLen = Math.Max(matchLen, DeflaterConstants.MIN_MATCH - 1);
        var matchLength = matchLen;

        if (scan + matchLength > scanMax) return false;

        var pinnedWindow = pinnedWindowPointer;
        var scanStart = strstart;
        var scanEnd1 = pinnedWindow[scan + matchLength - 1];
        var scanEnd = pinnedWindow[scan + matchLength];

        // Do not waste too much time if we already have a good match:
        if (matchLength >= goodLength) chainLength >>= 2;

        var pinnedPrev = pinnedPrevPointer;
        do
        {
            match = curMatch;
            scan = scanStart;

            if (pinnedWindow[match + matchLength] != scanEnd
                || pinnedWindow[match + matchLength - 1] != scanEnd1
                || pinnedWindow[match] != pinnedWindow[scan]
                || pinnedWindow[++match] != pinnedWindow[++scan])
                continue;

            // scan is set to strstart+1 and the comparison passed, so
            // scanMax - scan is the maximum number of bytes we can compare.
            // below we compare 8 bytes at a time, so first we compare
            // (scanMax - scan) % 8 bytes, so the remainder is a multiple of 8
            // n & (8 - 1) == n % 8.
            switch ((scanMax - scan) & 7)
            {
                case 1:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]) break;

                    break;

                case 2:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;

                case 3:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;

                case 4:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;

                case 5:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;

                case 6:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;

                case 7:
                    if (pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match]
                        && pinnedWindow[++scan] == pinnedWindow[++match])
                        break;

                    break;
            }

            if (pinnedWindow[scan] == pinnedWindow[match])
                // We check for insufficient lookahead only every 8th comparison;
                // the 256th check will be made at strstart + 258 unless lookahead is
                // exhausted first.
                do
                {
                    if (scan == scanMax)
                    {
                        ++scan; // advance to first position not matched
                        ++match;

                        break;
                    }
                } while (pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]
                         && pinnedWindow[++scan] == pinnedWindow[++match]);

            if (scan - scanStart > matchLength)
            {
                matchStrt = curMatch;
                matchLength = scan - scanStart;

                if (matchLength >= niceLength) break;

                scanEnd1 = pinnedWindow[scan - 1];
                scanEnd = pinnedWindow[scan];
            }
        } while ((curMatch = pinnedPrev[curMatch & DeflaterConstants.WMASK] & 0xFFFF) > limit && --chainLength != 0);

        matchStart = matchStrt;
        matchLen = matchLength;
        return matchLength >= DeflaterConstants.MIN_MATCH;
    }

    private bool DeflateStored(bool flush, bool finish)
    {
        if (!flush && lookahead == 0) return false;

        strstart += lookahead;
        lookahead = 0;

        var storedLength = strstart - blockStart;

        if (storedLength >= DeflaterConstants.MAX_BLOCK_SIZE || // Block is full
            (blockStart < DeflaterConstants.WSIZE &&
             storedLength >= DeflaterConstants.MAX_DIST) || // Block may move out of window
            flush)
        {
            var lastBlock = finish;
            if (storedLength > DeflaterConstants.MAX_BLOCK_SIZE)
            {
                storedLength = DeflaterConstants.MAX_BLOCK_SIZE;
                lastBlock = false;
            }

            huffman.FlushStoredBlock(window, blockStart, storedLength, lastBlock);
            blockStart += storedLength;
            return !(lastBlock || storedLength == 0);
        }

        return true;
    }

    private bool DeflateFast(bool flush, bool finish)
    {
        if (lookahead < DeflaterConstants.MIN_LOOKAHEAD && !flush) return false;

        while (lookahead >= DeflaterConstants.MIN_LOOKAHEAD || flush)
        {
            if (lookahead == 0)
            {
                // We are flushing everything
                huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
                blockStart = strstart;
                return false;
            }

            if (strstart > 2 * DeflaterConstants.WSIZE - DeflaterConstants.MIN_LOOKAHEAD)
                // slide window, as FindLongestMatch needs this.
                // This should only happen when flushing and the window
                // is almost full.
                SlideWindow();

            int hashHead;
            if (lookahead >= DeflaterConstants.MIN_MATCH &&
                (hashHead = InsertString()) != 0 &&
                strategy != DeflateStrategy.HuffmanOnly &&
                strstart - hashHead <= DeflaterConstants.MAX_DIST &&
                FindLongestMatch(hashHead))
            {
                // longestMatch sets matchStart and matchLen
                var full = huffman.TallyDist(strstart - matchStart, matchLen);

                lookahead -= matchLen;
                if (matchLen <= maxLazy && lookahead >= DeflaterConstants.MIN_MATCH)
                {
                    while (--matchLen > 0)
                    {
                        ++strstart;
                        InsertString();
                    }

                    ++strstart;
                }
                else
                {
                    strstart += matchLen;
                    if (lookahead >= DeflaterConstants.MIN_MATCH - 1) UpdateHash();
                }

                matchLen = DeflaterConstants.MIN_MATCH - 1;
                if (!full) continue;
            }
            else
            {
                // No match found
                huffman.TallyLit(pinnedWindowPointer[strstart] & 0xff);
                ++strstart;
                --lookahead;
            }

            if (huffman.IsFull())
            {
                var lastBlock = finish && lookahead == 0;
                huffman.FlushBlock(window, blockStart, strstart - blockStart, lastBlock);
                blockStart = strstart;
                return !lastBlock;
            }
        }

        return true;
    }

    private bool DeflateSlow(bool flush, bool finish)
    {
        if (lookahead < DeflaterConstants.MIN_LOOKAHEAD && !flush) return false;

        while (lookahead >= DeflaterConstants.MIN_LOOKAHEAD || flush)
        {
            if (lookahead == 0)
            {
                if (prevAvailable) huffman.TallyLit(pinnedWindowPointer[strstart - 1] & 0xff);

                prevAvailable = false;

                // We are flushing everything
                huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
                blockStart = strstart;
                return false;
            }

            if (strstart >= 2 * DeflaterConstants.WSIZE - DeflaterConstants.MIN_LOOKAHEAD)
                // slide window, as FindLongestMatch needs this.
                // This should only happen when flushing and the window
                // is almost full.
                SlideWindow();

            var prevMatch = matchStart;
            var prevLen = matchLen;
            if (lookahead >= DeflaterConstants.MIN_MATCH)
            {
                var hashHead = InsertString();

                if (strategy != DeflateStrategy.HuffmanOnly &&
                    hashHead != 0 &&
                    strstart - hashHead <= DeflaterConstants.MAX_DIST &&
                    FindLongestMatch(hashHead))
                    // longestMatch sets matchStart and matchLen
                    // Discard match if too small and too far away
                    if (matchLen <= 5 && (strategy == DeflateStrategy.Filtered ||
                                          (matchLen == DeflaterConstants.MIN_MATCH && strstart - matchStart > TooFar)))
                        matchLen = DeflaterConstants.MIN_MATCH - 1;
            }

            // previous match was better
            if (prevLen >= DeflaterConstants.MIN_MATCH && matchLen <= prevLen)
            {
                huffman.TallyDist(strstart - 1 - prevMatch, prevLen);
                prevLen -= 2;
                do
                {
                    strstart++;
                    lookahead--;
                    if (lookahead >= DeflaterConstants.MIN_MATCH) InsertString();
                } while (--prevLen > 0);

                strstart++;
                lookahead--;
                prevAvailable = false;
                matchLen = DeflaterConstants.MIN_MATCH - 1;
            }
            else
            {
                if (prevAvailable) huffman.TallyLit(pinnedWindowPointer[strstart - 1] & 0xff);

                prevAvailable = true;
                strstart++;
                lookahead--;
            }

            if (huffman.IsFull())
            {
                var len = strstart - blockStart;
                if (prevAvailable) len--;

                var lastBlock = finish && lookahead == 0 && !prevAvailable;
                huffman.FlushBlock(window, blockStart, len, lastBlock);
                blockStart += len;
                return !lastBlock;
            }
        }

        return true;
    }
}