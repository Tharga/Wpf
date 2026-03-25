using System.Windows;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Features.WindowLocation;

public class LocationTests
{
    [Fact]
    public void Default_Values()
    {
        var location = new Location();

        Assert.Equal(WindowState.Normal, location.WindowState);
        Assert.Equal(Visibility.Visible, location.Visibility);
        Assert.Equal(0, location.Left);
        Assert.Equal(0, location.Top);
        Assert.Equal(0, location.Width);
        Assert.Equal(0, location.Height);
        Assert.NotNull(location.Metadata);
        Assert.Empty(location.Metadata);
    }

    [Fact]
    public void Can_Set_All_Properties()
    {
        var location = new Location
        {
            WindowState = WindowState.Maximized,
            Visibility = Visibility.Hidden,
            Left = 100,
            Top = 200,
            Width = 800,
            Height = 600
        };

        Assert.Equal(WindowState.Maximized, location.WindowState);
        Assert.Equal(Visibility.Hidden, location.Visibility);
        Assert.Equal(100, location.Left);
        Assert.Equal(200, location.Top);
        Assert.Equal(800, location.Width);
        Assert.Equal(600, location.Height);
    }

    [Fact]
    public void Metadata_Can_Be_Used()
    {
        var location = new Location();
        location.Metadata["key"] = "value";

        Assert.Equal("value", location.Metadata["key"]);
    }
}
