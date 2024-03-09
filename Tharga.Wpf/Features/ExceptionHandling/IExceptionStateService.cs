namespace Tharga.Wpf.Features.ExceptionHandling;

internal interface IExceptionStateService
{
    void FallbackHandlerInternal(Exception exception);
}