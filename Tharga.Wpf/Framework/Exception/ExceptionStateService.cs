using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace Tharga.Wpf.Framework.Exception;

internal class ExceptionStateService : IExceptionStateService
{
    private readonly ILogger _logger;
    private Window _mainWindow;

    public ExceptionStateService(ILogger logger)
    {
        _logger = logger;
    }

    private static event EventHandler<HandleExceptionEventArgs> HandleExceptionEvent;

    public static void FallbackHandler(object sender, System.Exception exception)
    {
        HandleExceptionEvent?.Invoke(sender, new HandleExceptionEventArgs(exception));
    }

    public void FallbackHandler(System.Exception exception)
    {
        var exceptionTypeName = exception.GetType().Name;
        var message = exception.InnerException?.Message.NullIfEmpty() ?? exception.Message.NullIfEmpty() ?? exceptionTypeName;
        if (_mainWindow != null)
        {
            switch (exceptionTypeName)
            {
                case nameof(NotImplementedException):
                case nameof(InvalidOperationException):
                    MessageBox.Show(_mainWindow, message, "Oups", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                //TODO: Must be able to implement different cases here in local application here
                //case nameof(FortnoxException):
                //    var fortnoxException = exception as FortnoxException;
                //    //TODO: If fortnoxException is of type 'RefreshFortnoxLinkException', then help the user to create the connection
                //    message = message.Replace("Request failed: ", "");
                //    Debugger.Break();
                //    MessageBox.Show(_mainWindow, message, "Fortnox", MessageBoxButton.OK, MessageBoxImage.Error);
                //    break;
                default:
                    Debugger.Break();
                    MessageBox.Show(_mainWindow, $"{message}\n\n@{exception.StackTrace}", $"Unhandled ({exceptionTypeName})", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        else if (Application.Current.MainWindow != null)
        {
            MessageBox.Show(Application.Current.MainWindow, message, "Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show(message, "Fatal (No Main Window)", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        _logger?.LogError(exception, exception.Message);
    }

    public void AttachMainWindow(Window mainWindow)
    {
        if (_mainWindow != null) throw new InvalidOperationException("The main window has already been attached.");

        HandleExceptionEvent += (_, e) => { FallbackHandler(e.Exception); };

        _mainWindow = mainWindow;
    }
}