using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UseObjectOrCollectionInitializer

namespace FreePPlus.Imaging.Tests.Colors;

public class ColorTests
{
    private ITestOutputHelper _output;

    public ColorTests(ITestOutputHelper output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    private void ReportColors(IList<Color> colors, ITestOutputHelper output)
    {
        if (colors is { Count: > 0 } && output != null)
        {
            foreach (var color in colors)
            {
                var rgba = color.ToRgba32();
                var hex = color.ToHex();
                output.WriteLine($"Color with ARGB ({rgba.A},{rgba.R},{rgba.G},{rgba.B}) is hex: #{hex}");
            }
        }
    }

    [Fact]
    public void FromArgb_returns_color()
    {
        //Arrange
        var colors = new List<Color>();
        
        //Act
        colors.Add(Color.FromArgb(128, 128, 128, 128));
        colors.Add(Color.FromArgb(10, colors[0]));
        colors.Add(Color.FromArgb(colors[0].ToArgbInt32()));
        colors.Add(Color.FromArgb(100, 100, 100));

        //Assert
        colors.Should().NotBeNullOrEmpty();

        //Output
        ReportColors(colors, _output);
    }
}
