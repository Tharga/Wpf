using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace Tharga.Wpf.Framework.Exception;

internal class ExceptionStateService : IExceptionStateService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly IDictionary<Type, Type> _exceptionHandlers;
    private Window _mainWindow;

    public ExceptionStateService(IServiceProvider serviceProvider, ILogger logger, IDictionary<Type, Type> exceptionHandlers)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _exceptionHandlers = exceptionHandlers;
    }

    private static event EventHandler<HandleExceptionEventArgs> HandleExceptionEvent;

    public void AttachMainWindow(Window mainWindow)
    {
        if (_mainWindow != null) throw new InvalidOperationException("The main window has already been attached.");

        HandleExceptionEvent += (_, e) => { FallbackHandlerInternal(e.Exception); };

        _mainWindow = mainWindow;
    }

    internal static void FallbackHandler(object sender, System.Exception exception)
    {
        HandleExceptionEvent?.Invoke(sender, new HandleExceptionEventArgs(exception));
    }

    private void FallbackHandlerInternal(System.Exception exception)
    {
        if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handlerType))
        {
            var handler = _serviceProvider.GetService(handlerType);
            var method = handler.GetType().GetMethod(nameof(IExceptionHandler<System.Exception>.Handle));
            method?.Invoke(handler, [_mainWindow, exception]);
            return;
        }

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
}