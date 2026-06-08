# Articles

Guides for building WPF applications with Tharga.Wpf.

- **[Getting started](getting-started.md)** — install, `ApplicationBase` setup, registering services, and showing the main window.
- **[ApplicationBase and IOC](application-base.md)** — host-based DI, `GetService<T>()`, the `ViewModelProvider` MVVM markup extension, and `IViewModel` auto-registration.
- **[Single instance](single-instance.md)** — block multiple instances; the second launch signals the running one and exits. Releases the lock automatically before a Velopack update restart.
- **[Window management](window-management.md)** — `HideOnClose`, `StartupWindowState`, safe window-location restore (off-screen / undersized protection), persisting any window via `IWindowLocationService.Monitor`.
- **[Tab navigator](tab-navigator.md)** — `TabNavigatorView`, `TabView` base class, `OpenTabCommand<T>`, `AllowMultiple` / `AllowClose` / `AllowTabsWithSameTitles`.
- **[Splash screen](splash.md)** — three ways to trigger it, built-in image library (light + dark), `SplashForeground` for dark-image readability, fully custom `ISplash`.
- **[Application update](application-update.md)** — Velopack integration, periodic + on-demand checks, splash behavior during install, restart hooks.
- **[Exception handling](exception-handling.md)** — typed `IExceptionHandler<T>`, the fallback ErrorId dialog, and how `ILogger.LogError` routes ErrorId-displayed exceptions to Application Insights.
- **[License server](license-server.md)** — `Tharga.License` client, `LicenseServerLocation`, and `checkForLicense` on the splash.
