using System.Windows;

namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Represents a persisted window location including position, size, state, and custom metadata.
/// </summary>
public record Location
{
    /// <summary>The window state (Normal, Minimized, Maximized).</summary>
    public WindowState WindowState { get; init; }

    /// <summary>The window visibility.</summary>
    public Visibility Visibility { get; init; }

    /// <summary>The left position of the window in pixels.</summary>
    public int Left { get; init; }

    /// <summary>The top position of the window in pixels.</summary>
    public int Top { get; init; }

    /// <summary>The width of the window in pixels.</summary>
    public int Width { get; init; }

    /// <summary>The height of the window in pixels.</summary>
    public int Height { get; init; }

    /// <summary>Custom metadata associated with the window location.</summary>
    public readonly Dictionary<string, string> Metadata = new();
}