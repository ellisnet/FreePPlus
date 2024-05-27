// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     ICC Profile description
/// </summary>
internal readonly struct IccProfileDescription : IEquatable<IccProfileDescription>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccProfileDescription" /> struct.
    /// </summary>
    /// <param name="deviceManufacturer">Device Manufacturer</param>
    /// <param name="deviceModel">Device Model</param>
    /// <param name="deviceAttributes">Device Attributes</param>
    /// <param name="technologyInformation">Technology Information</param>
    /// <param name="deviceManufacturerInfo">Device Manufacturer Info</param>
    /// <param name="deviceModelInfo">Device Model Info</param>
    public IccProfileDescription(
        uint deviceManufacturer,
        uint deviceModel,
        IccDeviceAttribute deviceAttributes,
        IccProfileTag technologyInformation,
        IccLocalizedString[] deviceManufacturerInfo,
        IccLocalizedString[] deviceModelInfo)
    {
        DeviceManufacturer = deviceManufacturer;
        DeviceModel = deviceModel;
        DeviceAttributes = deviceAttributes;
        TechnologyInformation = technologyInformation;
        DeviceManufacturerInfo =
            deviceManufacturerInfo ?? throw new ArgumentNullException(nameof(deviceManufacturerInfo));
        DeviceModelInfo = deviceModelInfo ?? throw new ArgumentNullException(nameof(deviceModelInfo));
    }

    /// <summary>
    ///     Gets the device manufacturer.
    /// </summary>
    public uint DeviceManufacturer { get; }

    /// <summary>
    ///     Gets the device model.
    /// </summary>
    public uint DeviceModel { get; }

    /// <summary>
    ///     Gets the device attributes.
    /// </summary>
    public IccDeviceAttribute DeviceAttributes { get; }

    /// <summary>
    ///     Gets the technology information.
    /// </summary>
    public IccProfileTag TechnologyInformation { get; }

    /// <summary>
    ///     Gets the device manufacturer info.
    /// </summary>
    public IccLocalizedString[] DeviceManufacturerInfo { get; }

    /// <summary>
    ///     Gets the device model info.
    /// </summary>
    public IccLocalizedString[] DeviceModelInfo { get; }

    /// <inheritdoc />
    public bool Equals(IccProfileDescription other)
    {
        return DeviceManufacturer == other.DeviceManufacturer
               && DeviceModel == other.DeviceModel
               && DeviceAttributes == other.DeviceAttributes
               && TechnologyInformation == other.TechnologyInformation
               && DeviceManufacturerInfo.AsSpan().SequenceEqual(other.DeviceManufacturerInfo)
               && DeviceModelInfo.AsSpan().SequenceEqual(other.DeviceModelInfo);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccProfileDescription other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            DeviceManufacturer,
            DeviceModel,
            DeviceAttributes,
            TechnologyInformation,
            DeviceManufacturerInfo,
            DeviceModelInfo);
    }
}