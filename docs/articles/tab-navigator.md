# Tab navigator

`TabNavigatorView` is a tab-strip control intended for the main window. It manages a collection of `TabView` user controls — each tab carries its own title, can refuse close, and can optionally allow or block multiple instances.

## Step 1 — Add the navigator to the main window

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:tabNavigator="clr-namespace:Tharga.Wpf.TabNavigator;assembly=Tharga.Wpf">
    <tabNavigator:TabNavigatorView />
</Window>
```

## Step 2 — Create a `TabView`

Create a `UserControl`, then change its base class to `Tharga.Wpf.TabNavigator.TabView` in both the XAML and the code-behind:

```xml
<tabNav:TabView
    xmlns="..."
    xmlns:tabNav="clr-namespace:Tharga.Wpf.TabNavigator;assembly=Tharga.Wpf"
    x:Class="MyApp.Views.OrdersTabView">
    <!-- content -->
</tabNav:TabView>
```

```csharp
public partial class OrdersTabView : TabView
{
    public override string DefaultTitle => "Orders";
    public override bool AllowMultiple => false;
    public override bool AllowClose => true;
}
```

## Step 3 — Open a tab

Inject `ITabNavigationStateService` anywhere — typically the main `ViewModel`'s constructor — and call `OpenTabAsync`:

```csharp
public MainWindowViewModel(ITabNavigationStateService tabs)
{
    tabs.OpenTabAsync<OrdersTabView>();
}
```

Bind a command from the menu / ribbon:

```csharp
public ICommand OpenOrdersCommand
    => new OpenTabComamnd<OrdersTabView>(_tabs);
```

The view's `LoadActionAsync(object parameter)` runs on open — override it to do async initialization (load data, wire events) without blocking the navigator.

## TabView overrideables

| Member | Purpose |
|---|---|
| `DefaultTitle` | Title used when none is provided. Defaults to the type name. |
| `AllowMultiple` | When `false`, opening the same `TabView` type twice focuses the existing tab instead of creating a new one. |
| `AllowClose` | When `false`, the tab cannot be closed by the user. |
| `Title` | Mutable at runtime — fires `TitleChangedEvent`. |
| `CanClose` | Mutable; combined with `AllowClose` to gate the close button. Fires `CanCloseChangedEvent`. |
| `LoadActionAsync(parameter)` | Async initialization hook called on open. |
| `OnCloseAsync()` | Override to add a confirmation dialog; return `false` to cancel the close. |
| `Select()` | Make this tab the active one. |

## Title-collision blocking

The `AllowTabsWithSameTitles` option blocks creating a second tab with a title that matches an existing one — useful when each tab represents a different document/customer/file:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.AllowTabsWithSameTitles = false;
}
```

This check runs against the **title**, not the type — `AllowMultiple` is the type-level rule. For collision-blocking to apply you must pass a title at open time. `OpenTabCommand<T>` does not support passing a title — use a `RelayCommand` instead:

```csharp
public ICommand OpenCustomerCommand => new RelayCommand(
    parameter =>
    {
        var customer = (Customer)parameter;
        _tabs.OpenTabAsync<CustomerTabView>(customer.Name, customer);
    });
```
