using System.Windows;

namespace Tharga.Wpf.ExceptionHandling;

/// <summary>
/// A service that handles exceptions asynchronously, returning whether the exception was handled.
/// </summary>
public interface IExceptionHandlerService
{
    /// <summary>
    /// Handles the specified exception asynchronously.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="mainWindow">The application's main window.</param>
    /// <returns><c>true</c> if the exception was handled; otherwise, <c>false</c>.</returns>
    Task<bool> HandleExceptionAsync(Exception exception, Window mainWindow);
}