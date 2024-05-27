// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections;
using System.Collections.Generic;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Encapsulates a pixel-specific collection of <see cref="ImageFrame{T}" /> instances
///     that make up an <see cref="Image{T}" />.
/// </summary>
/// <typeparam name="TPixel">The type of the pixel.</typeparam>
public sealed class ImageFrameCollection<TPixel> : ImageFrameCollection, IEnumerable<ImageFrame<TPixel>>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly IList<ImageFrame<TPixel>> frames = new List<ImageFrame<TPixel>>();
    private readonly Image<TPixel> parent;

    internal ImageFrameCollection(Image<TPixel> parent, int width, int height, TPixel backgroundColor,
        IImageFormat format)
    {
        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));

        // Frames are already cloned within the caller
        frames.Add(new ImageFrame<TPixel>(parent.GetConfiguration(), width, height, backgroundColor));
        Format = format;
    }

    internal ImageFrameCollection(Image<TPixel> parent, int width, int height, MemoryGroup<TPixel> memorySource,
        IImageFormat format)
    {
        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));

        // Frames are already cloned within the caller
        frames.Add(new ImageFrame<TPixel>(parent.GetConfiguration(), width, height, memorySource));
        Format = format;
    }

    internal ImageFrameCollection(Image<TPixel> parent, IEnumerable<ImageFrame<TPixel>> frames, IImageFormat format)
    {
        Guard.NotNull(parent, nameof(parent));
        Guard.NotNull(frames, nameof(frames));

        this.parent = parent;

        // Frames are already cloned by the caller
        foreach (var f in frames)
        {
            ValidateFrame(f);
            this.frames.Add(f);
        }

        // Ensure at least 1 frame was added to the frames collection
        if (this.frames.Count == 0) throw new ArgumentException("Must not be empty.", nameof(frames));
        Format = format;
    }

    /// <summary>
    ///     Gets the number of frames.
    /// </summary>
    public override int Count => frames.Count;

    public override IImageFormat Format { get; }

    /// <summary>
    ///     Gets the root frame.
    /// </summary>
    public new ImageFrame<TPixel> RootFrame => frames.Count > 0 ? frames[0] : null;

    /// <inheritdoc />
    protected override ImageFrame NonGenericRootFrame => RootFrame;

    /// <summary>
    ///     Gets the <see cref="ImageFrame{TPixel}" /> at the specified index.
    /// </summary>
    /// <value>
    ///     The <see cref="ImageFrame{TPixel}" />.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns>The <see cref="ImageFrame{TPixel}" /> at the specified index.</returns>
    public new ImageFrame<TPixel> this[int index] => frames[index];

    /// <inheritdoc />
    IEnumerator<ImageFrame<TPixel>> IEnumerable<ImageFrame<TPixel>>.GetEnumerator()
    {
        return frames.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)frames).GetEnumerator();
    }

    /// <inheritdoc />
    public override int IndexOf(ImageFrame frame)
    {
        return frame is ImageFrame<TPixel> specific ? IndexOf(specific) : -1;
    }

    /// <summary>
    ///     Determines the index of a specific <paramref name="frame" /> in the <seealso cref="ImageFrameCollection{TPixel}" />
    ///     .
    /// </summary>
    /// <param name="frame">
    ///     The <seealso cref="ImageFrame{TPixel}" /> to locate in the
    ///     <seealso cref="ImageFrameCollection{TPixel}" />.
    /// </param>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(ImageFrame<TPixel> frame)
    {
        return frames.IndexOf(frame);
    }

    /// <summary>
    ///     Clones and inserts the <paramref name="source" /> into the <seealso cref="ImageFrameCollection{TPixel}" /> at the
    ///     specified <paramref name="index" />.
    /// </summary>
    /// <param name="index">The zero-based index to insert the frame at.</param>
    /// <param name="source">
    ///     The <seealso cref="ImageFrame{TPixel}" /> to clone and insert into the
    ///     <seealso cref="ImageFrameCollection{TPixel}" />.
    /// </param>
    /// <exception cref="ArgumentException">Frame must have the same dimensions as the image.</exception>
    /// <returns>The cloned <see cref="ImageFrame{TPixel}" />.</returns>
    public ImageFrame<TPixel> InsertFrame(int index, ImageFrame<TPixel> source)
    {
        ValidateFrame(source);
        var clonedFrame = source.Clone(parent.GetConfiguration());
        frames.Insert(index, clonedFrame);
        return clonedFrame;
    }

    /// <summary>
    ///     Clones the <paramref name="source" /> frame and appends the clone to the end of the collection.
    /// </summary>
    /// <param name="source">The raw pixel data to generate the <seealso cref="ImageFrame{TPixel}" /> from.</param>
    /// <returns>The cloned <see cref="ImageFrame{TPixel}" />.</returns>
    public ImageFrame<TPixel> AddFrame(ImageFrame<TPixel> source)
    {
        ValidateFrame(source);
        var clonedFrame = source.Clone(parent.GetConfiguration());
        frames.Add(clonedFrame);
        return clonedFrame;
    }

    /// <summary>
    ///     Creates a new frame from the pixel data with the same dimensions as the other frames and inserts the
    ///     new frame at the end of the collection.
    /// </summary>
    /// <param name="source">The raw pixel data to generate the <seealso cref="ImageFrame{TPixel}" /> from.</param>
    /// <returns>The new <see cref="ImageFrame{TPixel}" />.</returns>
    public ImageFrame<TPixel> AddFrame(ReadOnlySpan<TPixel> source)
    {
        var frame = ImageFrame.LoadPixelData(
            parent.GetConfiguration(),
            source,
            RootFrame.Width,
            RootFrame.Height);
        frames.Add(frame);
        return frame;
    }

    /// <summary>
    ///     Creates a new frame from the pixel data with the same dimensions as the other frames and inserts the
    ///     new frame at the end of the collection.
    /// </summary>
    /// <param name="source">The raw pixel data to generate the <seealso cref="ImageFrame{TPixel}" /> from.</param>
    /// <returns>The new <see cref="ImageFrame{TPixel}" />.</returns>
    public ImageFrame<TPixel> AddFrame(TPixel[] source)
    {
        Guard.NotNull(source, nameof(source));
        return AddFrame(source.AsSpan());
    }

    /// <summary>
    ///     Removes the frame at the specified index and frees all freeable resources associated with it.
    /// </summary>
    /// <param name="index">The zero-based index of the frame to remove.</param>
    /// <exception cref="InvalidOperationException">Cannot remove last frame.</exception>
    public override void RemoveFrame(int index)
    {
        if (index == 0 && Count == 1) throw new InvalidOperationException("Cannot remove last frame.");

        var frame = frames[index];
        frames.RemoveAt(index);
        frame.Dispose();
    }

    /// <inheritdoc />
    public override bool Contains(ImageFrame frame)
    {
        return frame is ImageFrame<TPixel> specific && Contains(specific);
    }

    /// <summary>
    ///     Determines whether the <seealso cref="ImageFrameCollection{TPixel}" /> contains the <paramref name="frame" />.
    /// </summary>
    /// <param name="frame">The frame.</param>
    /// <returns>
    ///     <c>true</c> if the <seealso cref="ImageFrameCollection{TPixel}" /> contains the specified frame; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Contains(ImageFrame<TPixel> frame)
    {
        return frames.Contains(frame);
    }

    /// <summary>
    ///     Moves an <seealso cref="ImageFrame{TPixel}" /> from <paramref name="sourceIndex" /> to
    ///     <paramref name="destinationIndex" />.
    /// </summary>
    /// <param name="sourceIndex">The zero-based index of the frame to move.</param>
    /// <param name="destinationIndex">The index to move the frame to.</param>
    public override void MoveFrame(int sourceIndex, int destinationIndex)
    {
        if (sourceIndex == destinationIndex) return;

        var frameAtIndex = frames[sourceIndex];
        frames.RemoveAt(sourceIndex);
        frames.Insert(destinationIndex, frameAtIndex);
    }

    /// <summary>
    ///     Removes the frame at the specified index and creates a new image with only the removed frame
    ///     with the same metadata as the original image.
    /// </summary>
    /// <param name="index">The zero-based index of the frame to export.</param>
    /// <exception cref="InvalidOperationException">Cannot remove last frame.</exception>
    /// <returns>The new <see cref="Image{TPixel}" /> with the specified frame.</returns>
    public new Image<TPixel> ExportFrame(int index)
    {
        var frame = this[index];

        if (Count == 1 && frames.Contains(frame)) throw new InvalidOperationException("Cannot remove last frame.");

        frames.Remove(frame);

        return new Image<TPixel>(parent.GetConfiguration(), parent.Metadata.DeepClone(), new[] { frame }, Format);
    }

    /// <summary>
    ///     Creates an <see cref="Image{T}" /> with only the frame at the specified index
    ///     with the same metadata as the original image.
    /// </summary>
    /// <param name="index">The zero-based index of the frame to clone.</param>
    /// <returns>The new <see cref="Image{TPixel}" /> with the specified frame.</returns>
    public new Image<TPixel> CloneFrame(int index)
    {
        var frame = this[index];
        var clonedFrame = frame.Clone();
        return new Image<TPixel>(parent.GetConfiguration(), parent.Metadata.DeepClone(), new[] { clonedFrame }, Format);
    }

    /// <summary>
    ///     Creates a new <seealso cref="ImageFrame{TPixel}" /> and appends it to the end of the collection.
    /// </summary>
    /// <returns>
    ///     The new <see cref="ImageFrame{TPixel}" />.
    /// </returns>
    public new ImageFrame<TPixel> CreateFrame()
    {
        var frame = new ImageFrame<TPixel>(
            parent.GetConfiguration(),
            RootFrame.Width,
            RootFrame.Height);
        frames.Add(frame);
        return frame;
    }

    /// <inheritdoc />
    protected override IEnumerator<ImageFrame> NonGenericGetEnumerator()
    {
        return frames.GetEnumerator();
    }

    /// <inheritdoc />
    protected override ImageFrame NonGenericGetFrame(int index)
    {
        return this[index];
    }

    /// <inheritdoc />
    protected override ImageFrame NonGenericInsertFrame(int index, ImageFrame source)
    {
        Guard.NotNull(source, nameof(source));

        if (source is ImageFrame<TPixel> compatibleSource) return InsertFrame(index, compatibleSource);

        var result = CopyNonCompatibleFrame(source);
        frames.Insert(index, result);
        return result;
    }

    /// <inheritdoc />
    protected override ImageFrame NonGenericAddFrame(ImageFrame source)
    {
        Guard.NotNull(source, nameof(source));

        if (source is ImageFrame<TPixel> compatibleSource) return AddFrame(compatibleSource);

        var result = CopyNonCompatibleFrame(source);
        frames.Add(result);
        return result;
    }

    /// <inheritdoc />
    protected override Image NonGenericExportFrame(int index)
    {
        return ExportFrame(index);
    }

    /// <inheritdoc />
    protected override Image NonGenericCloneFrame(int index)
    {
        return CloneFrame(index);
    }

    /// <inheritdoc />
    protected override ImageFrame NonGenericCreateFrame(Color backgroundColor)
    {
        return CreateFrame(backgroundColor.ToPixel<TPixel>());
    }

    /// <inheritdoc />
    protected override ImageFrame NonGenericCreateFrame()
    {
        return CreateFrame();
    }

    /// <summary>
    ///     Creates a new <seealso cref="ImageFrame{TPixel}" /> and appends it to the end of the collection.
    /// </summary>
    /// <param name="backgroundColor">The background color to initialize the pixels with.</param>
    /// <returns>
    ///     The new <see cref="ImageFrame{TPixel}" />.
    /// </returns>
    public ImageFrame<TPixel> CreateFrame(TPixel backgroundColor)
    {
        var frame = new ImageFrame<TPixel>(
            parent.GetConfiguration(),
            RootFrame.Width,
            RootFrame.Height,
            backgroundColor);
        frames.Add(frame);
        return frame;
    }

    private void ValidateFrame(ImageFrame<TPixel> frame)
    {
        Guard.NotNull(frame, nameof(frame));

        if (Count != 0)
            if (RootFrame.Width != frame.Width || RootFrame.Height != frame.Height)
                throw new ArgumentException("Frame must have the same dimensions as the image.", nameof(frame));
    }

    internal void Dispose()
    {
        foreach (var f in frames) f.Dispose();

        frames.Clear();
    }

    private ImageFrame<TPixel> CopyNonCompatibleFrame(ImageFrame source)
    {
        var result = new ImageFrame<TPixel>(
            parent.GetConfiguration(),
            source.Size(),
            source.Metadata.DeepClone());
        source.CopyPixelsTo(result.PixelBuffer.FastMemoryGroup);
        return result;
    }
}