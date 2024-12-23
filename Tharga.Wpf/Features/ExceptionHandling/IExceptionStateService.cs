namespace Tharga.Wpf.ExceptionHandling;

internal interface IExceptionStateService
{
    Task FallbackHandlerInternalAsync(Exception exception);
}