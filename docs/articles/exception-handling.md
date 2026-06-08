# Exception handling

Unhandled exceptions on the WPF dispatcher, `AppDomain`, or `TaskScheduler` are routed through `IExceptionStateService`. You can:

- Plug in a typed handler that runs *only* for one exception type — `IExceptionHandler<T>`.
- Rely on the built-in fallback, which shows a dialog with a unique **ErrorId** and logs the exception via `ILogger.LogError` so it ships to Application Insights.

## Typed handler — `IExceptionHandler<T>`

Implement the interface for the exception type you care about:

```csharp
public class InvalidOperationExceptionHandler : IExceptionHandler<InvalidOperationException>
{
    public void Handle(Window mainWindow, InvalidOperationException exception)
    {
        MessageBox.Show(
            mainWindow,
            $"Custom handler: '{exception.Message}'.",
            exception.GetType().Name);
    }
}
```

Register it in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.RegisterExceptionHandler<
        InvalidOperationExceptionHandler,
        InvalidOperationException>();
}
```

Behaviour:

- The exception's runtime type is checked against the registered handlers. The most specific match wins.
- The handler runs on the UI thread (the framework marshals to the dispatcher before calling it).
- If no typed handler matches, the fallback handler runs.

## Fallback handler — ErrorId dialog

When no typed handler matches, the framework:

1. Generates a unique `ErrorId` (a GUID).
2. Logs the exception via `ILogger.LogError` with the structured template `ErrorId={ErrorId} ExceptionType={ExceptionType} Message={Message}`. With Application Insights configured, this lands in `AppExceptions` and is queryable by ErrorId.
3. Displays a dialog showing the ErrorId so the user can quote it to support.

The log step is **first** — even if a custom handler later swallows the dialog or replaces the UX, the exception is still recorded in AI. This lets ops trace any cashier-reported ErrorId back to the full stack via:

```
AppExceptions | where customDimensions.ErrorId == "<guid>"
```

## Custom exception-handler service

For more control than `IExceptionHandler<T>` provides — for example, batching, custom telemetry, or completely replacing the dialog — register an `IExceptionHandlerService`:

```csharp
options.RegisterExceptionHandler<MyExceptionHandlerService>();
```

The service's `HandleAsync` runs after typed handlers and before the fallback. Returning `true` (handled) suppresses the fallback dialog; returning `false` lets it run.

## Threading

`IExceptionStateService` factories read `MainWindow` via `Dispatcher.Invoke`, so they are safe to resolve from any thread. The same applies to `IApplicationUpdateStateService`. Tab-navigator methods (`OpenTabAsync`, `CloseTabAsync`, `CloseAllTabsAsync`) also dispatch internally — you do not need to wrap them in `Application.Current.Dispatcher.Invoke` at the call site.
