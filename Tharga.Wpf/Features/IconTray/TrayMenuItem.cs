using System.Drawing;

namespace Tharga.Wpf.IconTray;

/// <summary>
/// Represents a menu item in the system tray context menu.
/// </summary>
public record TrayMenuItem
{
    /// <summary>The display text of the menu item.</summary>
    public string Text { get; init; }

    /// <summary>An optional image displayed next to the menu item text.</summary>
    public Image Image { get; init; }

    /// <summary>The action invoked when the menu item is clicked.</summary>
    public Action<object, EventArgs> Action { get; init; }
}