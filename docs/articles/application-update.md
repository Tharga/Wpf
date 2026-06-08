# Application update

Tharga.Wpf integrates [Velopack](https://www.nuget.org/packages/Velopack) to keep installed copies of an application current. Updates can run on startup (via the splash), on a recurring timer, or on demand from your own UI.

## Configuration

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.UpdateSystem = UpdateSystem.Velopack;
    options.UpdateIntervalCheck = TimeSpan.FromHours(1);
    options.UpdateLocation += config => new Uri(config["UpdateUrl"]).ToString();
}
```

| Option | Meaning |
|---|---|
| `UpdateSystem` | `None` (default) or `Velopack`. When `None`, no update checks happen — the splash still shows for branding. |
| `UpdateIntervalCheck` | Time between background checks. `TimeSpan.Zero` (or `null`) disables the timer; updates still happen on startup if you pass `checkForUpdates: true`. The first check fires after one interval (e.g. an hour after launch, not immediately). |
| `UpdateLocation` | A delegate that resolves the URL the Velopack release lives at (the directory containing `RELEASES`). Receives `IConfiguration` so it can read `appsettings.json`. |

## Startup check via splash

```csharp
this.StartMainWindow<MainWindow>(showSplash: true, checkForUpdates: true);
```

The splash appears, Velopack queries the update URL, and if a new release is available the splash stays open (close button hidden, progress bar shown) until the install completes. On success the application restarts into the new version. On failure the close button reappears with an error message so the user can dismiss the splash and continue running the current version.

## On-demand check

Call `CheckForUpdateAsync` to trigger a silent check from your own code — e.g. from a "Check for updates" menu item:

```csharp
var update = ApplicationBase.GetService<IApplicationUpdateStateService>();
await update.CheckForUpdateAsync(source: "menu");
```

Subscribe to `UpdateInfoEvent` if you want to show your own UI when a new version is found:

```csharp
update.UpdateInfoEvent += (_, args) =>
{
    // args carries version info — show a dialog, a tray balloon, etc.
};
```

## Periodic timer

When `UpdateIntervalCheck` is set, a background timer calls `UpdateClientApplication` with `silent: true`. The check runs without UI; the splash appears only when an actual update needs to download. If you do not want timer-driven updates, leave `UpdateIntervalCheck` at `TimeSpan.Zero` (or `null`) and use `CheckForUpdateAsync` from your menu.

## Pre-restart cleanup

Before a Velopack restart, Tharga.Wpf does two things automatically:

1. **Closes all open tabs** via `ITabNavigationStateService.CloseAllTabsAsync`. Tabs that override `OnCloseAsync` get to run their close handlers before the restart. The dispatch happens on the UI thread.
2. **Releases the single-instance lock** if `AllowMultipleApplications = false` so the updated process can acquire it cleanly.

You don't have to wire either step manually — `ApplicationUpdateStateServiceBase.BeforeRestartAsync` handles both.

## About menu pattern

To show version info + "Check for updates" in one panel, open the splash with `checkForUpdates: true, showCloseButton: true`:

```csharp
public ICommand AboutCommand => new RelayCommand(async _ =>
{
    var update = ApplicationBase.GetService<IApplicationUpdateStateService>();
    await update.ShowSplashAsync(checkForUpdates: true, showCloseButton: true);
});
```

The user sees the splash with version + environment, the update check runs, and the close button stays visible so they can dismiss the splash whenever they like.
