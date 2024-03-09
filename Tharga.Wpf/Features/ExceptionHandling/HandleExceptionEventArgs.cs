namespace Tharga.Wpf.ExceptionHandling;

internal class HandleExceptionEventArgs : EventArgs
{
    public Exception Exception { get; }

    public HandleExceptionEventArgs(Exception exception)
    {
        Exception = exception;
    }
}