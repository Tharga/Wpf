# Single instance

By default, Tharga.Wpf allows multiple instances of an application to run side-by-side. Set `AllowMultipleApplications = false` to enforce a single instance:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.AllowMultipleApplications = false;
}
```

## What happens on second launch

1. A second instance starts.
2. It detects the running instance via a named-mutex lock.
3. It signals the running instance to show itself — even if hidden in the system tray, the main window is restored and brought to the foreground.
4. The second instance exits without showing UI.

The user perceives a "click the shortcut to bring my app back" experience instead of a second window opening.

## Interaction with Velopack updates

The single-instance lock is automatically **released before a Velopack restart**. This avoids a deadlock where the updated process tries to start while the old process still holds the lock. The release happens in `ApplicationUpdateStateServiceBase.BeforeRestartAsync()` — you don't need to do anything special when you opt into single-instance mode plus Velopack updates.

## Choosing the lock name

The lock is keyed off `ApplicationShortName`. Two applications with the same short name (e.g. two builds of the same product) will see each other; two applications with different short names will not. Set this in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.ApplicationShortName = "MyApp";
    options.AllowMultipleApplications = false;
}
```
