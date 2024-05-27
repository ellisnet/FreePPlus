// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

// ReSharper disable RedundantAttributeUsageProperty

#if SUPPORTS_CODECOVERAGE
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(ExcludeFromCodeCoverageAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the attributed code should be excluded from code coverage information.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited
 = false, AllowMultiple = false)]
internal sealed class ExcludeFromCodeCoverageAttribute : Attribute { }
#endif