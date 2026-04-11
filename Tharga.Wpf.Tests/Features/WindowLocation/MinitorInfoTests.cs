using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Features.WindowLocation;

#pragma warning disable CS0618 // MinitorInfo is obsolete — testing backwards compatibility
public class MinitorInfoTests
{
    [Fact]
    public void Can_Set_FileLocation()
    {
        var info = new MinitorInfo { FileLocation = "C:\\data\\location.json" };

        Assert.Equal("C:\\data\\location.json", info.FileLocation);
    }

    [Fact]
    public void Can_Set_LoadLocation()
    {
        var location = new Location { Left = 100, Top = 200, Width = 800, Height = 600 };

        var info = new MinitorInfo { LoadLocation = location };

        Assert.Equal(location, info.LoadLocation);
    }

    [Fact]
    public void LocationUpdatedEvent_Is_Raised()
    {
        var info = new MinitorInfo();
        var location = new Location { Left = 50 };
        var eventArgs = new LocationUpdatedEventArgs(location, null);
        LocationUpdatedEventArgs received = null;
        info.LocationUpdatedEvent += (_, e) => received = e;

        info.OnLocationUpdatedEvent(this, eventArgs);

        Assert.NotNull(received);
        Assert.Equal(location, received.Location);
    }

    [Fact]
    public void LocationUpdatedEvent_Without_Subscribers_Does_Not_Throw()
    {
        var info = new MinitorInfo();
        var eventArgs = new LocationUpdatedEventArgs(new Location(), null);

        info.OnLocationUpdatedEvent(this, eventArgs);
    }
}
