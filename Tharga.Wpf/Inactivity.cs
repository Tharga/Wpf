namespace Tharga.Wpf;

public record Inactivity
{
    /// <summary>
    /// Set the time for when inatcivity action should be triggered.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Callback method for when inactivity has occured.
    /// </summary>
    public Action<TimeSpan> Action { get; set; }
}