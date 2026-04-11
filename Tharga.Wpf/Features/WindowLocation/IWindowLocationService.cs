using System.Windows;

namespace Tharga.Wpf.WindowLocation;

/// <summary>
/// Service for persisting and restoring window position, size, and state.
/// </summary>
public interface IWindowLocationService
{
    /// <summary>
    /// Starts monitoring the specified window, saving and restoring its location.
    /// </summary>
    /// <param name="window">The window to monitor.</param>
    /// <param name="name">A name identifying the window for persistence.</param>
    /// <param name="environment">An optional environment name for separate location storage.</param>
    /// <param name="isMainWindow">If true, HideOnClose and StartupWindowState options apply to this window.</param>
    /// <returns>A window monitor for the tracked window.</returns>
    IWindowMonitor Monitor(Window window, string name = default, string environment = default, bool isMainWindow = false);

    /// <summary>
    /// Sets the persisted visibility for the specified window.
    /// </summary>
    /// <param name="name">The window name.</param>
    /// <param name="visibility">The visibility to persist.</param>
    void SetVisibility(string name, Visibility visibility);

    /// <summary>
    /// Gets the folder path used for storing window location data.
    /// </summary>
    /// <param name="environment">The environment name.</param>
    /// <returns>The folder path.</returns>
    string GetFolder(string environment);
}