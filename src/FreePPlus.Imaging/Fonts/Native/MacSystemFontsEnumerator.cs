// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static FreePPlus.Imaging.Fonts.Native.CoreFoundation;
using static FreePPlus.Imaging.Fonts.Native.CoreText;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Native;

//was previously: namespace SixLabors.Fonts.Native;

/// <summary>
///     An enumerator that enumerates over available macOS system fonts.
///     The enumerated strings are the absolute paths to the font files.
/// </summary>
/// <remarks>
///     Internally, it calls the native CoreText's <see cref="CTFontManagerCopyAvailableFontURLs" /> method to retrieve
///     the list of fonts so using this class must be guarded by <c>RuntimeInformation.IsOSPlatform(OSPlatform.OSX)</c>.
/// </remarks>
internal class MacSystemFontsEnumerator : IEnumerable<string>, IEnumerator<string>
{
    private static readonly ArrayPool<byte> BytePool = ArrayPool<byte>.Shared;

    private readonly IntPtr fontUrls;
    private readonly bool releaseFontUrls;
    private int fontIndex;

    public MacSystemFontsEnumerator()
        : this(CTFontManagerCopyAvailableFontURLs(), true, 0) { }

    private MacSystemFontsEnumerator(IntPtr fontUrls, bool releaseFontUrls, int fontIndex)
    {
        if (fontUrls == IntPtr.Zero)
            throw new ArgumentException($"The {nameof(fontUrls)} must not be NULL.", nameof(fontUrls));

        this.fontUrls = fontUrls;
        this.releaseFontUrls = releaseFontUrls;
        this.fontIndex = fontIndex;

        Current = null!;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return new MacSystemFontsEnumerator(fontUrls, false, fontIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        Debug.Assert(CFGetTypeID(fontUrls) == CFArrayGetTypeID(), "The fontUrls array must be a CFArrayRef");
        if (fontIndex < CFArrayGetCount(fontUrls))
        {
            var fontUrl = CFArrayGetValueAtIndex(fontUrls, fontIndex);
            Debug.Assert(CFGetTypeID(fontUrl) == CFURLGetTypeID(),
                "The elements of the fontUrls array must be a CFURLRef");
            var fontPath = CFURLCopyFileSystemPath(fontUrl, CFURLPathStyle.kCFURLPOSIXPathStyle);

#if !NETSTANDARD2_0
            var current =
                Marshal.PtrToStringUTF8(CFStringGetCStringPtr(fontPath, CFStringEncoding.kCFStringEncodingUTF8));
            if (current is not null)
            {
                Current = current;
            }
            else
#endif
            {
                var fontPathLength = (int)CFStringGetLength(fontPath);
                var fontPathBufferSize = (fontPathLength + 1) * 2; // +1 for the NULL byte and *2 for UTF-16
                var fontPathBuffer = BytePool.Rent(fontPathBufferSize);
                CFStringGetCString(fontPath, fontPathBuffer, fontPathBufferSize,
                    CFStringEncoding.kCFStringEncodingUTF16LE);
                Current = Encoding.Unicode.GetString(fontPathBuffer, 0,
                    fontPathBufferSize - 2); // -2 for the UTF-16 NULL
                BytePool.Return(fontPathBuffer);
            }

            CFRelease(fontPath);

            fontIndex++;

            return true;
        }

        return false;
    }

    public void Reset()
    {
        fontIndex = 0;
    }

    public void Dispose()
    {
        if (releaseFontUrls) CFRelease(fontUrls);
    }
}