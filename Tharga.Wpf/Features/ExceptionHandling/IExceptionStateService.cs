namespace Tharga.Wpf.ExceptionHandling;

/// <summary>
/// Internal exception state service that provides a fallback handler for unhandled exceptions.
/// </summary>
public interface IExceptionStateService
{
    /// <summary>
    /// Handles an exception using the fallback handler chain.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    Task FallbackHandlerInternalAsync(Exception exception);
}