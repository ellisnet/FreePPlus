// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type contains the PostScript product name to which this profile
///     corresponds and the names of the companion CRDs
/// </summary>
internal sealed class IccCrdInfoTagDataEntry : IccTagDataEntry, IEquatable<IccCrdInfoTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCrdInfoTagDataEntry" /> class.
    /// </summary>
    /// <param name="postScriptProductName">the PostScript product name</param>
    /// <param name="renderingIntent0Crd">the rendering intent 0 CRD name</param>
    /// <param name="renderingIntent1Crd">the rendering intent 1 CRD name</param>
    /// <param name="renderingIntent2Crd">the rendering intent 2 CRD name</param>
    /// <param name="renderingIntent3Crd">the rendering intent 3 CRD name</param>
    public IccCrdInfoTagDataEntry(
        string postScriptProductName,
        string renderingIntent0Crd,
        string renderingIntent1Crd,
        string renderingIntent2Crd,
        string renderingIntent3Crd)
        : this(
            postScriptProductName,
            renderingIntent0Crd,
            renderingIntent1Crd,
            renderingIntent2Crd,
            renderingIntent3Crd,
            IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCrdInfoTagDataEntry" /> class.
    /// </summary>
    /// <param name="postScriptProductName">the PostScript product name</param>
    /// <param name="renderingIntent0Crd">the rendering intent 0 CRD name</param>
    /// <param name="renderingIntent1Crd">the rendering intent 1 CRD name</param>
    /// <param name="renderingIntent2Crd">the rendering intent 2 CRD name</param>
    /// <param name="renderingIntent3Crd">the rendering intent 3 CRD name</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccCrdInfoTagDataEntry(
        string postScriptProductName,
        string renderingIntent0Crd,
        string renderingIntent1Crd,
        string renderingIntent2Crd,
        string renderingIntent3Crd,
        IccProfileTag tagSignature)
        : base(IccTypeSignature.CrdInfo, tagSignature)
    {
        PostScriptProductName = postScriptProductName;
        RenderingIntent0Crd = renderingIntent0Crd;
        RenderingIntent1Crd = renderingIntent1Crd;
        RenderingIntent2Crd = renderingIntent2Crd;
        RenderingIntent3Crd = renderingIntent3Crd;
    }

    /// <summary>
    ///     Gets the PostScript product name
    /// </summary>
    public string PostScriptProductName { get; }

    /// <summary>
    ///     Gets the rendering intent 0 CRD name
    /// </summary>
    public string RenderingIntent0Crd { get; }

    /// <summary>
    ///     Gets the rendering intent 1 CRD name
    /// </summary>
    public string RenderingIntent1Crd { get; }

    /// <summary>
    ///     Gets the rendering intent 2 CRD name
    /// </summary>
    public string RenderingIntent2Crd { get; }

    /// <summary>
    ///     Gets the rendering intent 3 CRD name
    /// </summary>
    public string RenderingIntent3Crd { get; }

    /// <inheritdoc />
    public bool Equals(IccCrdInfoTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && string.Equals(PostScriptProductName, other.PostScriptProductName)
               && string.Equals(RenderingIntent0Crd, other.RenderingIntent0Crd)
               && string.Equals(RenderingIntent1Crd, other.RenderingIntent1Crd)
               && string.Equals(RenderingIntent2Crd, other.RenderingIntent2Crd)
               && string.Equals(RenderingIntent3Crd, other.RenderingIntent3Crd);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccCrdInfoTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccCrdInfoTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            PostScriptProductName,
            RenderingIntent0Crd,
            RenderingIntent1Crd,
            RenderingIntent2Crd,
            RenderingIntent3Crd);
    }
}