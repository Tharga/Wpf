# Window management

Tharga.Wpf manages the main window's close behaviour, startup state, and persisted location. Any number of additional windows can opt into location persistence via `IWindowLocationService.Monitor`.

## Close behaviour

By default, both the X button and `ApplicationBase.Close()` exit the application. Set `HideOnClose = true` to make the X button hide the window to the system tray instead:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.HideOnClose = true;
}
```

| Action | `HideOnClose = false` | `HideOnClose = true` |
|---|---|---|
| X button | Exit | Hide to tray |
| `ApplicationBase.Close(CloseMode.Soft)` | Exit gracefully | Exit gracefully |
| `ApplicationBase.Close(CloseMode.Force)` | Exit immediately | Exit immediately |
| `ApplicationBase.Hide()` | Hide to tray | Hide to tray |

`Hide()` is always available regardless of `HideOnClose` — use it for an explicit "Minimize to tray" menu item.

A `Soft` close runs through the registered close handlers (including any pending tab close confirmations); a `Force` close bypasses them.

## Startup window state

```csharp
options.StartupWindowState = StartupWindowState.Last; // default
```

| Value | Behaviour on startup |
|---|---|
| `Last` | Restore the saved state, including hidden-in-tray |
| `Normal` | Always start in normal window state |
| `Maximized` | Always start maximized |
| `Minimized` | Always start minimized |
| `Hidden` | Always start hidden in the system tray |

`Hidden` is useful for background tools that should sit in the tray until the user opens them.

## Safe location restore

When restoring a saved window location, the framework guards against two failure modes:

- **Too small** — width or height below 200x150 pixels are replaced with defaults. Prevents a misconfigured save from leaving the window invisible-but-present.
- **Off-screen** — if the saved rectangle isn't on any currently connected monitor (laptop dock removed, second monitor disconnected, resolution changed), the window is repositioned to the center of the primary screen.

You don't have to opt into these guards — they apply automatically to any window monitored by `IWindowLocationService`.

## Persisting other windows

The main window is monitored automatically when you use `StartMainWindow<T>`. For any additional window — dialogs, tool windows, secondary documents — call `Monitor` in the window's constructor:

```csharp
public MyDialogWindow()
{
    InitializeComponent();
    ApplicationBase.GetService<IWindowLocationService>()
        .Monitor(this, nameof(MyDialogWindow));
}
```

The window's position and size are saved when it closes and restored when it reopens. If the user creates several distinct "instances" of the same window class that should remember separate locations, pass a unique `name` per instance instead of `nameof(Type)`.

### Per-environment storage

Pass an `environment` to keep separate location data per environment (e.g. Dev / Test / Prod):

```csharp
locationService.Monitor(this, nameof(MyDialogWindow), environment: "Prod");
```

Locations are stored under `%AppData%/<CompanyName>/<ApplicationShortName>/<environment>/`.
