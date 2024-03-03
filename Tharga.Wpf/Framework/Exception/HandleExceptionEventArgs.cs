namespace Tharga.Wpf.Framework.Exception;

internal class HandleExceptionEventArgs : EventArgs
{
    public System.Exception Exception { get; }

    public HandleExceptionEventArgs(System.Exception exception)
    {
        Exception = exception;
    }
}