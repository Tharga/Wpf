using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Features.WindowLocation;

public class LocationUpdatedEventArgsTests
{
    [Fact]
    public void Constructor_Sets_Location()
    {
        var location = new Location { Left = 10, Top = 20 };

        var args = new LocationUpdatedEventArgs(location, null);

        Assert.Equal(location, args.Location);
        Assert.Null(args.Exception);
    }

    [Fact]
    public void Constructor_Sets_Exception()
    {
        var exception = new InvalidOperationException("test error");

        var args = new LocationUpdatedEventArgs(null, exception);

        Assert.Null(args.Location);
        Assert.Equal(exception, args.Exception);
    }

    [Fact]
    public void Constructor_Sets_Both()
    {
        var location = new Location();
        var exception = new Exception("error");

        var args = new LocationUpdatedEventArgs(location, exception);

        Assert.Equal(location, args.Location);
        Assert.Equal(exception, args.Exception);
    }
}
