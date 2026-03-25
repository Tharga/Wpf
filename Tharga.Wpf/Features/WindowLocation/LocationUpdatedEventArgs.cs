namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Event arguments raised when a monitored window's location is updated or an error occurs during persistence.
/// </summary>
public class LocationUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationUpdatedEventArgs"/> class.
    /// </summary>
    /// <param name="location">The updated location, or <c>null</c> if an error occurred.</param>
    /// <param name="exception">The exception that occurred, or <c>null</c> if the update was successful.</param>
    public LocationUpdatedEventArgs(Location location, Exception exception)
    {
        Location = location;
        Exception = exception;
    }

    /// <summary>Gets the updated window location.</summary>
    public Location Location { get; }

    /// <summary>Gets the exception that occurred during the location update, if any.</summary>
    public Exception Exception { get; }
}