using System.Windows;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Closes a window and reports whether it actually closed.
/// <see cref="Window.Close"/> returns normally when a <c>Closing</c> handler cancels, so the outcome is observed through <see cref="Window.Closed"/>.
/// </summary>
internal static class WindowCloser
{
    /// <summary>
    /// Closes the window.
    /// </summary>
    /// <param name="window">The window to close.</param>
    /// <returns><c>true</c> if the window had closed when this method returned; <c>false</c> if the close was cancelled or deferred.</returns>
    public static bool Close(Window window)
    {
        var closed = false;
        void OnClosed(object sender, EventArgs e) => closed = true;

        window.Closed += OnClosed;
        try
        {
            window.Close();
        }
        finally
        {
            window.Closed -= OnClosed;
        }

        return closed;
    }
}
