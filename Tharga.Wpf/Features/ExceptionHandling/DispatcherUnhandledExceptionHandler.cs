using System.Diagnostics;
using System.Windows.Threading;

namespace Tharga.Wpf.ExceptionHandling;

/// <summary>
/// Handles <see cref="System.Windows.Application.DispatcherUnhandledException"/> through <see cref="IExceptionStateService"/>.
/// WPF reads <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> as soon as the handler returns, so it is set before the asynchronous fallback handler starts.
/// </summary>
internal sealed class DispatcherUnhandledExceptionHandler
{
    private readonly Func<IExceptionStateService> _getExceptionStateService;

    public DispatcherUnhandledExceptionHandler(Func<IExceptionStateService> getExceptionStateService)
    {
        _getExceptionStateService = getExceptionStateService;
    }

    public void Handle(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        IExceptionStateService exceptionStateService;
        try
        {
            exceptionStateService = _getExceptionStateService();
        }
        catch (Exception exception)
        {
            Report(exception);
            return;
        }

        e.Handled = true;
        _ = RunFallbackHandlerAsync(exceptionStateService, e.Exception);
    }

    private static async Task RunFallbackHandlerAsync(IExceptionStateService exceptionStateService, Exception exception)
    {
        try
        {
            await exceptionStateService.FallbackHandlerInternalAsync(exception);
        }
        catch (Exception handlerException)
        {
            Report(handlerException);
        }
    }

    private static void Report(Exception exception)
    {
        Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
        Debugger.Break();
    }
}
