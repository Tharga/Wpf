using System.Windows;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Features.WindowLocation;

public class LocationFormatterTests
{
    [Fact]
    public void Location_Round_Trips()
    {
        var location = new Location
        {
            WindowState = WindowState.Normal,
            Visibility = Visibility.Visible,
            Left = 100,
            Top = 200,
            Width = 800,
            Height = 600
        };

        var result = LocationFormatter.Parse("Main", LocationFormatter.Serialize("Main", location));

        Assert.Equal(location.WindowState, result.WindowState);
        Assert.Equal(location.Visibility, result.Visibility);
        Assert.Equal(location.Left, result.Left);
        Assert.Equal(location.Top, result.Top);
        Assert.Equal(location.Width, result.Width);
        Assert.Equal(location.Height, result.Height);
    }

    [Fact]
    public void Negative_Coordinates_Round_Trip()
    {
        var location = new Location { Left = -1800, Top = -900, Width = 800, Height = 450 };

        var result = LocationFormatter.Parse("Main", LocationFormatter.Serialize("Main", location));

        Assert.Equal(-1800, result.Left);
        Assert.Equal(-900, result.Top);
    }

    [Fact]
    public void Metadata_Round_Trips()
    {
        var location = new Location { Left = 100, Top = 200, Width = 800, Height = 600 };
        location.Metadata["key"] = "value";
        location.Metadata["other"] = "42";

        var result = LocationFormatter.Parse("Main", LocationFormatter.Serialize("Main", location));

        Assert.Equal("value", result.Metadata["key"]);
        Assert.Equal("42", result.Metadata["other"]);
    }

    [Fact]
    public void Wrong_Name_Throws()
    {
        var data = LocationFormatter.Serialize("Main", new Location());

        Assert.Throws<NotImplementedException>(() => LocationFormatter.Parse("Other", data));
    }

    [Fact]
    public void Too_Few_Segments_Returns_Null()
    {
        var result = LocationFormatter.Parse("Main", "Main;Normal;Visible;1;2;3");

        Assert.Null(result);
    }
}
