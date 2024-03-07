namespace Tharga.Wpf.Framework.Exception;

internal interface IExceptionStateService
{
    void FallbackHandlerInternal(System.Exception exception);
}