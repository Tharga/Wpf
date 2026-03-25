using System.Windows;

namespace Tharga.Wpf.ExceptionHandling;

/// <summary>
/// Handles exceptions of a specific type, allowing custom error handling UI or logic.
/// </summary>
/// <typeparam name="T">The type of exception to handle.</typeparam>
public interface IExceptionHandler<in T>
    where T : Exception
{
    /// <summary>
    /// Handles the specified exception.
    /// </summary>
    /// <param name="mainWindow">The application's main window.</param>
    /// <param name="exception">The exception to handle.</param>
    public void Handle(Window mainWindow, T exception);
}