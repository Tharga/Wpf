namespace Tharga.Wpf;

/// <summary>
/// Specifies the window state to use when the application starts.
/// </summary>
public enum StartupWindowState
{
    /// <summary>Restore the last saved state, including hidden (tray). This is the default.</summary>
    Last,

    /// <summary>Always start in normal window state.</summary>
    Normal,

    /// <summary>Always start maximized.</summary>
    Maximized,

    /// <summary>Always start minimized.</summary>
    Minimized,

    /// <summary>Always start hidden in the system tray.</summary>
    Hidden
}
