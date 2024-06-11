namespace Tharga.Wpf.WindowLocation;

public class LocationUpdatedEventArgs : EventArgs
{
    public LocationUpdatedEventArgs(Location location, Exception exception)
    {
        Location = location;
        Exception = exception;
    }

    public Location Location { get; }
    public Exception Exception { get; }
}