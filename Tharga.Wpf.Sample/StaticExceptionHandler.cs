using System.IO;

namespace Tharga.Wpf.Sample;

internal static class StaticExceptionHandler
{
    public static event EventHandler<ErrorEventArgs> ErrorEvent;

    public static void Handle(Exception e, object sender = default)
    {
        ErrorEvent?.Invoke(sender, new ErrorEventArgs(e));
    }
}