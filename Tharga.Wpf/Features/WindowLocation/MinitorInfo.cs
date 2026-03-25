namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Contains information about a monitored window, including the file used for persistence
/// and the initially loaded location.
/// </summary>
public record MinitorInfo
{
    /// <summary>The file path where the window location is persisted.</summary>
    public string FileLocation { get; init; }

    /// <summary>The location that was loaded from disk when monitoring started.</summary>
    public Location LoadLocation { get; init; }

    /// <summary>Raised when the monitored window's location is saved or an error occurs.</summary>
    public event EventHandler<LocationUpdatedEventArgs> LocationUpdatedEvent;

    internal void OnLocationUpdatedEvent(object sender, LocationUpdatedEventArgs locationUpdatedEventArgs)
    {
        LocationUpdatedEvent?.Invoke(sender, locationUpdatedEventArgs);
    }
}