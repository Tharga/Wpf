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
    /// <returns>Information about the monitoring state and persisted location.</returns>
    MinitorInfo Monitor(Window window, string name = default, string environment = default);

    /// <summary>
    /// Sets the persisted visibility for the specified window.
    /// </summary>
    /// <param name="name">The window name.</param>
    /// <param name="visibility">The visibility to persist.</param>
    void SetVisibility(string name, Visibility visibility);

    /// <summary>
    /// Returns whether the window should be shown on startup based on saved state and startup options.
    /// </summary>
    /// <param name="name">The window name.</param>
    /// <returns><c>true</c> if the window should be shown; <c>false</c> if it should stay hidden.</returns>
    bool ShouldShowOnStartup(string name);

    /// <summary>
    /// Gets the folder path used for storing window location data.
    /// </summary>
    /// <param name="environment">The environment name.</param>
    /// <returns>The folder path.</returns>
    string GetFolder(string environment);
}