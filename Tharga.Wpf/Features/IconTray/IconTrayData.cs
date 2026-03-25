using System.Drawing;

namespace Tharga.Wpf.IconTray;

/// <summary>
/// Configuration data for the system tray icon, including the icon and context menu items.
/// </summary>
public record IconTrayData
{
    /// <summary>The icon displayed in the system tray.</summary>
    public Icon Icon { get; init; }

    /// <summary>The context menu items shown when right-clicking the tray icon.</summary>
    public TrayMenuItem[] Menu { get; init; }
}