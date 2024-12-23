using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.ExceptionHandling;

internal class ExceptionStateService : IExceptionStateService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExceptionStateService> _logger;
    private readonly IDictionary<Type, Type> _exceptionHandlers;
    private readonly Window _mainWindow;

    public ExceptionStateService(IServiceProvider serviceProvider, Window mainWindow, ILogger<ExceptionStateService> logger, IDictionary<Type, Type> exceptionHandlers)
    {
        _serviceProvider = serviceProvider;
        _mainWindow = mainWindow ?? Application.Current.MainWindow;
        _logger = logger;
        _exceptionHandlers = exceptionHandlers;
    }

    public async Task FallbackHandlerInternalAsync(Exception exception)
    {
        try
        {
            if (exception is XamlParseException && exception.InnerException != null) exception = exception.InnerException;

            if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handlerType))
            {
                var handler = _serviceProvider.GetService(handlerType) ?? throw new NullReferenceException($"Cannot find error handler type '{handlerType.Name}'.");
                var method = handler.GetType().GetMethod(nameof(IExceptionHandler<Exception>.Handle));
                method?.Invoke(handler, [_mainWindow, exception]);
                return;
            }

            var exceptionHandlerService = _serviceProvider.GetService<IExceptionHandlerService>();
            if (exceptionHandlerService != null)
            {
                var result = await exceptionHandlerService.HandleExceptionAsync(exception, _mainWindow);
                if (result) return;
            }

            var exceptionTypeName = exception.GetType().Name;
            var message = exception.InnerException?.Message.NullIfEmpty() ?? exception.Message.NullIfEmpty() ?? exceptionTypeName;
            if (_mainWindow != null)
            {
                _logger?.LogError(exception, exception.Message);

                switch (exceptionTypeName)
                {
                    case nameof(NotImplementedException):
                    case nameof(InvalidOperationException):
                    case nameof(NotSupportedException):
                    case nameof(TypeNotRegisteredException):
                        MessageBox.Show(_mainWindow, message, exceptionTypeName, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        Debugger.Break();
                        MessageBox.Show(_mainWindow, $"{message}\n\n@{exception.StackTrace}", $"Unexpected {exceptionTypeName}.", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                var caption = "Fatal (No Main Window)";
                _logger?.LogCritical(exception, $"{caption}. {exception.Message}");
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        catch (Exception e)
        {
            var msg = "Error in the error handler.";
            _logger?.LogCritical(e, $"{msg} {e.Message}");
            Debugger.Break();
            MessageBox.Show($"{msg} {e.Message}", e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}