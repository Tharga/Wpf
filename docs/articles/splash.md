# Splash screen

The built-in splash screen displays during application startup, application updates, and any time the consumer calls `ShowSplashAsync` (e.g. an About menu). It overlays the application name, environment, version, exe location, and a message list on top of a background image.

## Triggering the splash

The splash is **not** shown automatically. Three ways to trigger it:

### 1. Canonical — `StartMainWindow<T>`

The recommended path. Does everything: window registration, splash, tray icon, close behaviour, startup visibility.

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    this.StartMainWindow<MainWindow>(
        showSplash: true,
        checkForUpdates: true);
}
```

### 2. Manual

For full control over when the splash appears:

```csharp
var update = ApplicationBase.GetService<IApplicationUpdateStateService>();
await update.ShowSplashAsync(checkForUpdates: true);
// Auto-closes after a short delay when no update is found.
```

### 3. About-style

Pass `showCloseButton: true` so the splash stays open until the user dismisses it — typical for an "About" menu item:

```csharp
await update.ShowSplashAsync(
    checkForUpdates: false,
    showCloseButton: true,
    checkForLicense: true);
```

## Choosing a built-in image

Set `SplashCreator` to return a `Splash` with a different image from `SplashImageLibrary`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.SplashCreator = data => new Splash(data with
    {
        ImagePath = SplashImageLibrary.Blue
    });
}
```

Available built-in images:

| Light backgrounds | Dark backgrounds (need a light foreground) |
|---|---|
| `Blue`, `Green`, `GreenTransparent`, `Orange`, `Red`, `RedTransparent`, `Teal`, `TealTransparent`, `White`, `Yellow` | `Colours`, `Fire`, `Mosaik`, `Prism`, `Silicon` |

## Foreground color

Overlaid text uses WPF's default (dark) foreground, which is unreadable on a dark image. Set `SplashForeground` to a bright brush:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.SplashCreator = data => new Splash(data with
    {
        ImagePath = SplashImageLibrary.Silicon
    });
    options.SplashForeground = Brushes.White;
}
```

The same effect can be achieved per-splash by setting `SplashData.Foreground` inside `SplashCreator`. When neither is set, WPF's default foreground is used (no behaviour change for existing consumers on light images).

## Using a custom image

Point `ImagePath` to any image file. The image should be a **PNG** at **500 x 309 pixels**. Use a transparent PNG if you want the splash window background to show through.

For a project-bundled image, add it as a `Resource` and reference via pack URI:

```csharp
options.SplashCreator = data => new Splash(data with
{
    ImagePath = "pack://application:,,,/MyApp;component/Images/splash.png"
});
```

## Replacing the splash entirely

Implement `ISplash` and return it from `SplashCreator` to use a completely custom splash window:

```csharp
options.SplashCreator = data => new MyCustomSplash(data);
```

You lose the built-in update / license / progress UX — implement them yourself or skip them.

## Update behaviour

When an application update is detected, the splash stays open with a progress bar and the close button **hidden**, so the user knows an update is in progress. If the update fails, the close button reappears with an error message. See [Application update](application-update.md).
