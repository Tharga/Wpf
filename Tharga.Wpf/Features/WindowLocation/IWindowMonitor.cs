namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Represents a monitored window with persisted location state.
/// </summary>
public interface IWindowMonitor
{
    /// <summary>The file path where the window location is persisted.</summary>
    string FileLocation { get; }

    /// <summary>The location that was loaded from disk when monitoring started.</summary>
    Location LoadLocation { get; }

    /// <summary>Raised when the monitored window's location is saved or an error occurs.</summary>
    event EventHandler<LocationUpdatedEventArgs> LocationUpdatedEvent;
}
