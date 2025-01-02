namespace Tharga.Wpf.ExceptionHandling;

public interface IExceptionStateService
{
    Task FallbackHandlerInternalAsync(Exception exception);
}