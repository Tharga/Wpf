# ApplicationBase and IOC

`ApplicationBase` is the entry point for any Tharga.Wpf application. It hosts a `Microsoft.Extensions.Hosting` builder, configures the dependency-injection container, and exposes a static `GetService<T>()` accessor for code paths where constructor injection is not available.

## Registering services

Override `Register` in your `App` class:

```csharp
protected override void Register(HostBuilderContext context, IServiceCollection services)
{
    services.AddTransient<IOrderService, OrderService>();
    services.AddSingleton<ICustomerCache, CustomerCache>();
}
```

`HostBuilderContext` exposes `Configuration` (an `IConfiguration`) so you can register against `appsettings.json`-style configuration the same way an ASP.NET host does.

## Resolving services

Two paths:

**Constructor injection** — ViewModels (anything registered or marked with `IViewModel`) receive registered services through their constructors:

```csharp
public class OrderViewModel(IOrderService orderService) : ViewModelBase
{
    private readonly IOrderService _orderService = orderService;
}
```

**Static accessor** — for code-behind or static helpers:

```csharp
var orderService = ApplicationBase.GetService<IOrderService>();
```

If the requested type was not registered, `GetService<T>` throws `TypeNotRegisteredException`. The accessor is safe to call from any thread; the underlying provider is the same instance that was built during startup.

## Shutdown

When the application exits, `ApplicationBase.OnExit` cancels `ICancellationService.Token`, releases the single-instance lock, stops and disposes the host, and then raises the application's `Exit` event. Stopping the host stops your hosted services and flushes logging providers. All of this finishes before `OnExit` returns, so exit takes as long as your hosted services need to stop. Once the host's `ShutdownTimeout` has passed, the token given to their `StopAsync` is cancelled. `Exit` is raised even if a step before it fails.

> **Changed in 3.1.0.** Before that, `OnExit` returned at the first step that did not complete at once, for example cancelling a token that a pending `Task.Delay` was using. WPF then shut the dispatcher down, and the remaining steps were skipped: hosted services were not stopped, the last log entries could be lost, and the `Exit` event was not raised.

## MVVM binding helper — `ViewModelProvider`

Wire a `View` to its `ViewModel` declaratively in XAML without manually constructing or setting `DataContext`:

```xml
<UserControl
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:wpf="clr-namespace:Tharga.Wpf;assembly=Tharga.Wpf"
    xmlns:local="clr-namespace:MyApp.Views"
    DataContext="{wpf:ViewModelProvider local:OrderViewModel}">
</UserControl>
```

The `ViewModelProvider` markup extension constructs the ViewModel through the DI container so any injected dependencies resolve. Pair this with `IViewModel` (see below) so the ViewModel does not need an explicit `services.AddTransient<OrderViewModel>()` line.

## `IViewModel` — auto-registration

Any type that implements the marker interface `Tharga.Wpf.IViewModel` is auto-registered in DI during host build, so you do not need to add it to `services` manually.

```csharp
public class OrderViewModel : ViewModelBase, IViewModel
{
    public OrderViewModel(IOrderService orderService) { ... }
}
```

If your ViewModels live in a different assembly than the `App` class, register that assembly in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.UseAssembly(typeof(OrderViewModel).Assembly);
}
```

`ApplicationBase`'s startup scans both its own assembly and any assemblies added via `UseAssembly` for `IViewModel` implementations.

## ViewModel base class

`ViewModelBase` provides:
- `INotifyPropertyChanged` plumbing with a `SetField` helper.
- `RelayCommand` / `RelayCommand<T>` for binding commands without writing custom command classes.

```csharp
public class CounterViewModel : ViewModelBase
{
    private int _count;
    public int Count
    {
        get => _count;
        set => SetField(ref _count, value);
    }

    public ICommand IncrementCommand => new RelayCommand(_ => Count++);
}
```
