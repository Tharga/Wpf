---
_layout: landing
---

# Tharga.Wpf

A toolkit for building **WPF desktop applications** on .NET 10. Provides an `ApplicationBase` with built-in dependency injection, a tab navigator, a configurable splash screen, Velopack-based application updates, single-instance enforcement, window-location persistence, custom exception handling, and a paired license-server client.

## Packages

| Package | What it does |
|---|---|
| [Tharga.Wpf](https://www.nuget.org/packages/Tharga.Wpf) | `ApplicationBase`, `TabNavigatorView`, splash screen, Velopack update plumbing, single-instance lock, window-location persistence, custom exception handlers. |
| [Tharga.License](https://www.nuget.org/packages/Tharga.License) | License-server client used by Tharga.Wpf's license check (and reusable standalone). Targets `net8.0` / `net9.0` / `net10.0`. |

## Quick start

```
dotnet add package Tharga.Wpf
```

Inherit from `ApplicationBase` in `App.xaml`:

```xml
<wpf:ApplicationBase
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:wpf="clr-namespace:Tharga.Wpf;assembly=Tharga.Wpf"
    x:Class="MyApp.App"
    StartupUri="MainWindow.xaml">
</wpf:ApplicationBase>
```

Register services and options in `App.xaml.cs`:

```csharp
public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions options)
    {
        options.ApplicationShortName = "Sample";
        options.ApplicationFullName = "My Sample Application";
    }
}
```

See [Getting started](articles/getting-started.md) for the full setup walkthrough.

## What's in the box

- **[ApplicationBase + IOC](articles/application-base.md)** — host-based DI, `GetService<T>()`, MVVM `ViewModelProvider` markup extension.
- **[Single instance](articles/single-instance.md)** — block multiple instances; second launch signals the running one and exits.
- **[Window management](articles/window-management.md)** — close-to-tray, startup window state, safe size/position restore.
- **[Tab navigator](articles/tab-navigator.md)** — `TabView`, `OpenTabCommand<T>`, `AllowMultiple`, `AllowClose`, title-collision blocking.
- **[Splash screen](articles/splash.md)** — built-in image library (light + dark), `SplashForeground`, `SplashCreator` for custom images, `ISplash` for full replacement.
- **[Application update](articles/application-update.md)** — Velopack-based, periodic + on-demand, splash integration during install.
- **[Exception handling](articles/exception-handling.md)** — typed `IExceptionHandler<T>`, fallback dialog with ErrorId, AI-loggable.
- **[License server](articles/license-server.md)** — `Tharga.License` integration with the WPF host's license check.

## Repo

[github.com/Tharga/Wpf](https://github.com/Tharga/Wpf) — source, issues, releases.
