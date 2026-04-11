using System.Windows;
using Tharga.Wpf.IconTray;

namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Extension methods for registering windows with the Tharga WPF framework.
/// </summary>
public static class WindowExtensions
{
    /// <summary>
    /// Registers the main application window for position/size persistence, close behavior, and optional tray icon.
    /// <c>HideOnClose</c> and <c>StartupWindowState</c> options apply to this window.
    /// </summary>
    /// <param name="window">The main window to register.</param>
    /// <param name="showTrayIcon">If true, creates a system tray icon with restore and exit functionality.</param>
    /// <returns>A window monitor for the tracked window.</returns>
    public static IWindowMonitor RegisterMainWindow(this Window window, bool showTrayIcon = false)
    {
        var windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        var name = string.IsNullOrEmpty(window.Name) ? window.GetType().Name : window.Name;
        var monitor = windowLocationService.Monitor(window, name, isMainWindow: true);

        if (showTrayIcon)
        {
            var notifyIconService = ApplicationBase.GetService<INotifyIconService>();
            notifyIconService.Create(window, name, windowLocationService);
        }

        return monitor;
    }

    /// <summary>
    /// Registers a child or dialog window for position and size persistence.
    /// <c>HideOnClose</c> and <c>StartupWindowState</c> options do not apply to this window.
    /// </summary>
    /// <param name="window">The window to register.</param>
    /// <returns>A window monitor for the tracked window.</returns>
    public static IWindowMonitor RegisterWindow(this Window window)
    {
        var windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        var name = string.IsNullOrEmpty(window.Name) ? window.GetType().Name : window.Name;
        var monitor = windowLocationService.Monitor(window, name);
        return monitor;
    }
}
