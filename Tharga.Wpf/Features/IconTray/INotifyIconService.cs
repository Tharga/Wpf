using System.Windows;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.IconTray;

/// <summary>
/// Service for creating and managing a system tray notification icon.
/// </summary>
public interface INotifyIconService
{
    /// <summary>
    /// Creates a system tray icon associated with the specified window.
    /// </summary>
    /// <param name="window">The window to associate with the tray icon.</param>
    /// <param name="name">The name used for window location persistence.</param>
    /// <param name="windowLocationService">The window location service for visibility tracking.</param>
    void Create(Window window, string name, IWindowLocationService windowLocationService);
}