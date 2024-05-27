// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Infrastructure;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors;

internal static partial class Guard
{
    /// <summary>
    ///     Ensures that the value is a value type.
    /// </summary>
    /// <param name="value">The target object, which cannot be null.</param>
    /// <param name="parameterName">The name of the parameter that is to be checked.</param>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <exception cref="ArgumentException"><paramref name="value" /> is not a value type.</exception>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void MustBeValueType<TValue>(TValue value, string parameterName)
    {
        if (!value.GetType().GetTypeInfo().IsValueType)
            ThrowHelper.ThrowArgumentException("Type must be a struct.", parameterName);
    }
}