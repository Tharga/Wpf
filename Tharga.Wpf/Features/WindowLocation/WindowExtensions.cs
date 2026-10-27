using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Tharga.Wpf.Framework;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Extension methods for registering windows with the Tharga WPF framework.
/// </summary>
public static class WindowExtensions
{
    private static readonly ConditionalWeakTable<Window, AsyncCloseGate> CloseGates = new();

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

    /// <summary>
    /// Handles the full close lifecycle: hide-on-close check, tab closing, and base OnClosing.
    /// Call this from <c>OnClosing</c> to replace all close boilerplate.
    /// </summary>
    /// <param name="window">The window being closed.</param>
    /// <param name="e">The closing event args.</param>
    /// <param name="baseOnClosing">Pass <c>base.OnClosing</c> to allow the framework to raise the Closing event.</param>
    /// <remarks>
    /// When a tab's <c>OnCloseAsync</c> does not complete synchronously, the close is cancelled while the tabs close,
    /// and the window is closed again once every tab has agreed. That second close goes straight to <paramref name="baseOnClosing"/>.
    /// </remarks>
    public static async Task HandleCloseAsync(this Window window, CancelEventArgs e, Action<CancelEventArgs> baseOnClosing)
    {
        var gate = CloseGates.GetValue(window, _ => new AsyncCloseGate());
        if (gate.TryConsumeConfirmed())
        {
            baseOnClosing(e);
            return;
        }

        if (ApplicationBase.HandleClose(e)) return;

        var tabNavigationStateService = ApplicationBase.GetService<ITabNavigationStateService>();
        var force = ApplicationBase.CloseMode == CloseMode.Force;
        await gate.RunAsync(e, () => tabNavigationStateService.CloseAllTabsAsync(force), () => baseOnClosing(e), window.Close);
    }
}
