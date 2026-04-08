namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Represents a splash screen that displays startup progress, update information, and error messages.
/// </summary>
public interface ISplash
{
    /// <summary>Displays an informational message on the splash screen.</summary>
    /// <param name="message">The message to display.</param>
    void UpdateInfo(string message);

    /// <summary>Shows the splash screen.</summary>
    void Show();

    /// <summary>Hides the splash screen.</summary>
    void Hide();

    /// <summary>Displays an error message on the splash screen.</summary>
    /// <param name="message">The error message to display.</param>
    void SetErrorMessage(string message);

    /// <summary>Makes the close button visible on the splash screen.</summary>
    void ShowCloseButton();

    /// <summary>Hides the close button on the splash screen, preventing the user from closing it.</summary>
    void HideCloseButton();

    /// <summary>Shows an indeterminate progress bar indicating a long-running operation.</summary>
    void ShowProgress();

    /// <summary>Hides the progress bar.</summary>
    void HideProgress();

    /// <summary>Closes and disposes the splash screen.</summary>
    void Close();

    /// <summary>Gets a value indicating whether the close button is currently visible.</summary>
    bool IsCloseButtonVisible { get; }

    /// <summary>Clears all displayed messages from the splash screen.</summary>
    void ClearMessages();
}