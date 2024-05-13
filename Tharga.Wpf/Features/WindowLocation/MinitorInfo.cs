namespace Tharga.Wpf.WindowLocation;

public record MinitorInfo
{
    public string FileLocation { get; init; }
    public Location LoadLocation { get; init; }
    public event EventHandler<LocationUpdatedEventArgs> LocationUpdatedEvent;

    internal void OnLocationUpdatedEvent(object sender, LocationUpdatedEventArgs locationUpdatedEventArgs)
    {
        LocationUpdatedEvent?.Invoke(sender, locationUpdatedEventArgs);
    }
}