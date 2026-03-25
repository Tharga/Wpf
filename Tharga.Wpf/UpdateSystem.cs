namespace Tharga.Wpf;

/// <summary>
/// Specifies which update system the application uses for automatic updates.
/// </summary>
public enum UpdateSystem
{
    /// <summary>No automatic update system.</summary>
    None,

    /// <summary>Use the Squirrel update framework.</summary>
    Squirrel,

    /// <summary>Use the Velopack update framework.</summary>
    Velopack
}