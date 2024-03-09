namespace Tharga.Wpf.ExceptionHandling;

internal interface IExceptionStateService
{
    void FallbackHandlerInternal(Exception exception);
}