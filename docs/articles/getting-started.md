# Getting started

## Install

```
dotnet add package Tharga.Wpf
```

Tharga.Wpf targets **`net10.0-windows`** (WPF). Tharga.License (a separate package referenced by Tharga.Wpf) is multi-targeted to `net8.0` / `net9.0` / `net10.0`.

## Step 1 — Inherit from `ApplicationBase`

Change `App.xaml` so the application root is `Tharga.Wpf.ApplicationBase` instead of `System.Windows.Application`:

```xml
<wpf:ApplicationBase
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:wpf="clr-namespace:Tharga.Wpf;assembly=Tharga.Wpf"
    x:Class="MyApp.App"
    StartupUri="MainWindow.xaml">
</wpf:ApplicationBase>
```

## Step 2 — Register services and options

In `App.xaml.cs`, override `Register` and `Options`:

```csharp
public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions options)
    {
        options.CompanyName = "Contoso";
        options.ApplicationShortName = "Sample";
        options.ApplicationFullName = "Contoso Sample Application";
    }
}
```

`CompanyName` and `ApplicationShortName` are used for the folder that stores window-location data; `ApplicationFullName` appears on the splash screen and update shortcut.

## Step 3 — Use `StartMainWindow<T>` for the full lifecycle (optional but recommended)

Instead of letting `StartupUri` show the window directly, drive startup through the `StartMainWindow<T>` extension. It registers the main window, wires up close-to-tab-close, optionally shows the splash (and runs an update + license check while it's visible):

```csharp
public partial class App
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        this.StartMainWindow<MainWindow>(
            showSplash: true,
            checkForUpdates: true,
            checkForLicense: false);
    }
}
```

When using `StartMainWindow`, remove `StartupUri` from `App.xaml`.

## Step 4 — Resolve services anywhere

`ApplicationBase.GetService<T>()` is available statically. The recommended path is constructor injection into ViewModels (see [ApplicationBase and IOC](application-base.md) for the `ViewModelProvider` markup extension), but for code-behind or places where you can't inject:

```csharp
var myService = ApplicationBase.GetService<MyService>();
```

## Next steps

- [ApplicationBase and IOC](application-base.md) — register ViewModels with `IViewModel` and bind via `ViewModelProvider`.
- [Splash screen](splash.md) — choose a built-in image, set the foreground color, or replace the splash window entirely.
- [Application update](application-update.md) — set `UpdateSystem = Velopack` and `UpdateLocation` so installed copies stay current.
- [Tab navigator](tab-navigator.md) — add the `TabNavigatorView` control and open tabs from commands.
