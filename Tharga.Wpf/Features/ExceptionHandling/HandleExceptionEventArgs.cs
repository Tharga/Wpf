namespace Tharga.Wpf.Features.ExceptionHandling;

internal class HandleExceptionEventArgs : EventArgs
{
    public Exception Exception { get; }

    public HandleExceptionEventArgs(Exception exception)
    {
        Exception = exception;
    }
}