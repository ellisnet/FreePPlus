// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Encodes and compresses the image data using dynamic Lempel-Ziv compression.
/// </summary>
/// <remarks>
///     Adapted from Jef Poskanzer's Java port by way of J. M. G. Elliott. K Weiner 12/00
///     <para>
///         GIFCOMPR.C       - GIF Image compression routines
///     </para>
///     <para>
///         Lempel-Ziv compression based on 'compress'.  GIF modifications by
///         David Rowley (mgardi@watdcsu.waterloo.edu)
///     </para>
///     GIF Image compression - modified 'compress'
///     <para>
///         Based on: compress.c - File compression ala IEEE Computer, June 1984.
///         By Authors:  Spencer W. Thomas      (decvax!harpo!utah-cs!utah-gr!thomas)
///         Jim McKie              (decvax!mcvax!jim)
///         Steve Davies           (decvax!vax135!petsd!peora!srd)
///         Ken Turkowski          (decvax!decwrl!turtlevax!ken)
///         James A. Woods         (decvax!ihnp4!ames!jaw)
///         Joe Orost              (decvax!vax135!petsd!joe)
///     </para>
/// </remarks>
internal sealed class LzwEncoder : IDisposable
{
    /// <summary>
    ///     80% occupancy
    /// </summary>
    private const int HashSize = 5003;

    /// <summary>
    ///     The amount to shift each code.
    /// </summary>
    private const int HashShift = 4;

    /// <summary>
    ///     The maximum number of bits/code.
    /// </summary>
    private const int MaxBits = 12;

    /// <summary>
    ///     Should NEVER generate this code.
    /// </summary>
    private const int MaxMaxCode = 1 << MaxBits;

    /// <summary>
    ///     Mask used when shifting pixel values
    /// </summary>
    private static readonly int[] Masks =
    {
        0b0,
        0b1,
        0b11,
        0b111,
        0b1111,
        0b11111,
        0b111111,
        0b1111111,
        0b11111111,
        0b111111111,
        0b1111111111,
        0b11111111111,
        0b111111111111,
        0b1111111111111,
        0b11111111111111,
        0b111111111111111,
        0b1111111111111111
    };

    /// <summary>
    ///     Define the storage for the packet accumulator.
    /// </summary>
    private readonly byte[] accumulators = new byte[256];

    /// <summary>
    ///     The code table.
    /// </summary>
    private readonly IMemoryOwner<int> codeTable;

    /// <summary>
    ///     The hash table.
    /// </summary>
    private readonly IMemoryOwner<int> hashTable;

    /// <summary>
    ///     The initial code size.
    /// </summary>
    private readonly int initialCodeSize;

    /// <summary>
    ///     Number of characters so far in this 'packet'
    /// </summary>
    private int accumulatorCount;

    /// <summary>
    ///     Number of bits/code
    /// </summary>
    private int bitCount;

    /// <summary>
    ///     The clear code.
    /// </summary>
    private int clearCode;

    /// <summary>
    ///     Block compression parameters -- after all codes are used up,
    ///     and compression rate changes, start over.
    /// </summary>
    private bool clearFlag;

    /// <summary>
    ///     Output the given code.
    ///     Inputs:
    ///     code:   A bitCount-bit integer.  If == -1, then EOF.  This assumes
    ///     that bitCount =&lt; wordsize - 1.
    ///     Outputs:
    ///     Outputs code to the file.
    ///     Assumptions:
    ///     Chars are 8 bits long.
    ///     Algorithm:
    ///     Maintain a BITS character long buffer (so that 8 codes will
    ///     fit in it exactly).  Use the VAX insv instruction to insert each
    ///     code in turn.  When the buffer fills up empty it and start over.
    /// </summary>
    private int currentAccumulator;

    /// <summary>
    ///     The current bits.
    /// </summary>
    private int currentBits;

    /// <summary>
    ///     The end-of-file code.
    /// </summary>
    private int eofCode;

    /// <summary>
    ///     First unused entry
    /// </summary>
    private int freeEntry;

    /// <summary>
    ///     Algorithm:  use open addressing double hashing (no chaining) on the
    ///     prefix code / next character combination.  We do a variant of Knuth's
    ///     algorithm D (vol. 3, sec. 6.4) along with G. Knott's relatively-prime
    ///     secondary probe.  Here, the modular division first probe is gives way
    ///     to a faster exclusive-or manipulation.  Also do block compression with
    ///     an adaptive reset, whereby the code table is cleared when the compression
    ///     ratio decreases, but after the table fills.  The variable-length output
    ///     codes are re-sized at this point, and a special CLEAR code is generated
    ///     for the decompressor.  Late addition:  construct the table according to
    ///     file size for noticeable speed improvement on small files.  Please direct
    ///     questions about this implementation to ames!jaw.
    /// </summary>
    private int globalInitialBits;

    /// <summary>
    ///     maximum code, given bitCount
    /// </summary>
    private int maxCode;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LzwEncoder" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The <see cref="MemoryAllocator" /> to use for buffer allocations.</param>
    /// <param name="colorDepth">The color depth in bits.</param>
    public LzwEncoder(MemoryAllocator memoryAllocator, int colorDepth)
    {
        initialCodeSize = Math.Max(2, colorDepth);
        hashTable = memoryAllocator.Allocate<int>(HashSize, AllocationOptions.Clean);
        codeTable = memoryAllocator.Allocate<int>(HashSize, AllocationOptions.Clean);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        hashTable?.Dispose();
        codeTable?.Dispose();
    }

    /// <summary>
    ///     Encodes and compresses the indexed pixels to the stream.
    /// </summary>
    /// <param name="indexedPixels">The 2D buffer of indexed pixels.</param>
    /// <param name="stream">The stream to write to.</param>
    public void Encode(Buffer2D<byte> indexedPixels, Stream stream)
    {
        // Write "initial code size" byte
        stream.WriteByte((byte)initialCodeSize);

        // Compress and write the pixel data
        Compress(indexedPixels, initialCodeSize + 1, stream);

        // Write block terminator
        stream.WriteByte(GifConstants.Terminator);
    }

    /// <summary>
    ///     Gets the maximum code value.
    /// </summary>
    /// <param name="bitCount">The number of bits</param>
    /// <returns>See <see cref="int" /></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMaxcode(int bitCount)
    {
        return (1 << bitCount) - 1;
    }

    /// <summary>
    ///     Add a character to the end of the current packet, and if it is 254 characters,
    ///     flush the packet to disk.
    /// </summary>
    /// <param name="c">The character to add.</param>
    /// <param name="accumulatorsRef">The reference to the storage for packet accumulators</param>
    /// <param name="stream">The stream to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddCharacter(byte c, ref byte accumulatorsRef, Stream stream)
    {
        Unsafe.Add(ref accumulatorsRef, accumulatorCount++) = c;
        if (accumulatorCount >= 254) FlushPacket(stream);
    }

    /// <summary>
    ///     Table clear for block compress.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearBlock(Stream stream)
    {
        ResetCodeTable();
        freeEntry = clearCode + 2;
        clearFlag = true;

        Output(clearCode, stream);
    }

    /// <summary>
    ///     Reset the code table.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetCodeTable()
    {
        hashTable.GetSpan().Fill(-1);
    }

    /// <summary>
    ///     Compress the packets to the stream.
    /// </summary>
    /// <param name="indexedPixels">The 2D buffer of indexed pixels.</param>
    /// <param name="initialBits">The initial bits.</param>
    /// <param name="stream">The stream to write to.</param>
    private void Compress(Buffer2D<byte> indexedPixels, int initialBits, Stream stream)
    {
        // Set up the globals: globalInitialBits - initial number of bits
        globalInitialBits = initialBits;

        // Set up the necessary values
        clearFlag = false;
        bitCount = globalInitialBits;
        maxCode = GetMaxcode(bitCount);
        clearCode = 1 << (initialBits - 1);
        eofCode = clearCode + 1;
        freeEntry = clearCode + 2;
        accumulatorCount = 0; // Clear packet

        ResetCodeTable(); // Clear hash table
        Output(clearCode, stream);

        ref var hashTableRef = ref MemoryMarshal.GetReference(hashTable.GetSpan());
        ref var codeTableRef = ref MemoryMarshal.GetReference(codeTable.GetSpan());

        int entry = indexedPixels[0, 0];

        for (var y = 0; y < indexedPixels.Height; y++)
        {
            ref var rowSpanRef = ref MemoryMarshal.GetReference(indexedPixels.GetRowSpan(y));
            var offsetX = y == 0 ? 1 : 0;

            for (var x = offsetX; x < indexedPixels.Width; x++)
            {
                int code = Unsafe.Add(ref rowSpanRef, x);
                var freeCode = (code << MaxBits) + entry;
                var hashIndex = (code << HashShift) ^ entry;

                if (Unsafe.Add(ref hashTableRef, hashIndex) == freeCode)
                {
                    entry = Unsafe.Add(ref codeTableRef, hashIndex);
                    continue;
                }

                // Non-empty slot
                if (Unsafe.Add(ref hashTableRef, hashIndex) >= 0)
                {
                    var disp = 1;
                    if (hashIndex != 0) disp = HashSize - hashIndex;

                    do
                    {
                        if ((hashIndex -= disp) < 0) hashIndex += HashSize;

                        if (Unsafe.Add(ref hashTableRef, hashIndex) == freeCode)
                        {
                            entry = Unsafe.Add(ref codeTableRef, hashIndex);
                            break;
                        }
                    } while (Unsafe.Add(ref hashTableRef, hashIndex) >= 0);

                    if (Unsafe.Add(ref hashTableRef, hashIndex) == freeCode) continue;
                }

                Output(entry, stream);
                entry = code;
                if (freeEntry < MaxMaxCode)
                {
                    Unsafe.Add(ref codeTableRef, hashIndex) = freeEntry++; // code -> hashtable
                    Unsafe.Add(ref hashTableRef, hashIndex) = freeCode;
                }
                else
                {
                    ClearBlock(stream);
                }
            }
        }

        // Output the final code.
        Output(entry, stream);
        Output(eofCode, stream);
    }

    /// <summary>
    ///     Flush the packet to disk and reset the accumulator.
    /// </summary>
    /// <param name="outStream">The output stream.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FlushPacket(Stream outStream)
    {
        outStream.WriteByte((byte)accumulatorCount);
        outStream.Write(accumulators, 0, accumulatorCount);
        accumulatorCount = 0;
    }

    /// <summary>
    ///     Output the current code to the stream.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="outs">The stream to write to.</param>
    private void Output(int code, Stream outs)
    {
        ref var accumulatorsRef = ref MemoryMarshal.GetReference(accumulators.AsSpan());
        currentAccumulator &= Masks[currentBits];

        if (currentBits > 0)
            currentAccumulator |= code << currentBits;
        else
            currentAccumulator = code;

        currentBits += bitCount;

        while (currentBits >= 8)
        {
            AddCharacter((byte)(currentAccumulator & 0xFF), ref accumulatorsRef, outs);
            currentAccumulator >>= 8;
            currentBits -= 8;
        }

        // If the next entry is going to be too big for the code size,
        // then increase it, if possible.
        if (freeEntry > maxCode || clearFlag)
        {
            if (clearFlag)
            {
                maxCode = GetMaxcode(bitCount = globalInitialBits);
                clearFlag = false;
            }
            else
            {
                ++bitCount;
                maxCode = bitCount == MaxBits
                    ? MaxMaxCode
                    : GetMaxcode(bitCount);
            }
        }

        if (code == eofCode)
        {
            // At EOF, write the rest of the buffer.
            while (currentBits > 0)
            {
                AddCharacter((byte)(currentAccumulator & 0xFF), ref accumulatorsRef, outs);
                currentAccumulator >>= 8;
                currentBits -= 8;
            }

            if (accumulatorCount > 0) FlushPacket(outs);
        }
    }
}