# Tharga Wpf
[![NuGet](https://img.shields.io/nuget/v/Tharga.Wpf)](https://www.nuget.org/packages/Tharga.Wpf)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Wpf)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![GitHub repo Issues](https://img.shields.io/github/issues/Tharga/Wpf?style=flat&logo=github&logoColor=red&label=Issues)](https://github.com/Tharga/Wpf/issues?q=is%3Aopen)

This package is a basic toolset for WPF applications.

## Features
- [MVVM binding helper](#mvvm-binding-helper)
- [Built in .NET IOC](#built-in-net-ioc)
- [Single Instance](#single-instance)
- [Window Management](#window-management)
- [Remember window location](#remember-window-location)
- [TabNavigator](#tabnavigator)
- [Splash Screen](#splash-screen)
- [*ClickOnce* application update](#clickonce-application-update)
- [Custom Exception handling](#custom-exception-handling)
- [License Server Check](#license-server-check)

The sample uses *Fluent.Ribbon* but any package can be used since MainWindow does not need any base class.

## Testing
The solution includes xUnit test projects for both `Tharga.Wpf` and `Tharga.License`. Run all tests with:

```bash
dotnet test -c Release
```

## Get started
Set App.xaml to inherit from `ApplicationBase`.

```
<wpf:ApplicationBase
    ...
    xmlns:wpf="clr-namespace:Tharga.Wpf;assembly=Tharga.Wpf"
>
```

Override the methods *Register* to register services in the IOC.
Also override *Options* to configure options.

```
public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions thargaWpfOptions)
    {
        thargaWpfOptions.ApplicationShortName = "Sample";
        thargaWpfOptions.ApplicationFullName = "Tharga Wpf Sample Application";
    }
}
```

The *MainWindow* can inherit from any class like for instance *RibbonWindow*.

## MVVM binding helper
To bind a *View* with a *ViewModel* add this section in the View.xaml.

`DataContext="{wpf:ViewModelProvider local:MyViewModel}"`

## Built in .NET IOC
Tharga.WPF uses the built in .NET IOC with IServiceCollection and IServiceProvider.

### View Model dependency injection
Injection can be done in the ViewModel constructor for all registered service. To have the ViewModel it self registered, do it manually in the *Register* in *App.xaml.cs* or by adding the interface *Tharga.Wpf.IViewModel*.

### View dependency injection
There is no support for injecting services into the View directly. To use resources this method can be used.

`var myService = ApplicationBase.GetService<MyService>();`

## Single Instance

Prevent multiple instances of the application from running. When a second instance is started, it signals the existing instance to show itself (even if hidden to the system tray) and then exits.

```csharp
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.AllowMultipleApplications = false;
}
```

This works with Squirrel and Velopack update systems — the single-instance lock is automatically released before an update restart.

## Window Management

### Close behavior

By default, closing the window (X button) and calling `ApplicationBase.Close()` exits the application. To have the X button hide the window to the system tray instead:

```csharp
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.HideOnClose = true;
}
```

| Action | HideOnClose = false | HideOnClose = true |
|--------|--------------------|--------------------|
| X button | Exit | Hide to tray |
| `ApplicationBase.Close(CloseMode.Soft)` | Exit gracefully | Exit gracefully |
| `ApplicationBase.Close(CloseMode.Force)` | Exit immediately | Exit immediately |
| `ApplicationBase.Hide()` | Hide to tray | Hide to tray |

`ApplicationBase.Hide()` is always available regardless of `HideOnClose` — use it for a "Minimize to tray" menu item.

### Startup window state

Control how the window appears on startup:

```csharp
thargaWpfOptions.StartupWindowState = StartupWindowState.Last; // default
```

| Value | Behavior |
|-------|----------|
| `Last` | Restore the saved state, including hidden (tray) |
| `Normal` | Always start in normal window state |
| `Maximized` | Always start maximized |
| `Minimized` | Always start minimized |
| `Hidden` | Always start hidden in the system tray |

### Window size and position safety

When restoring a saved window location, the framework validates that:
- Width and height are at least 200x150 pixels — smaller values are replaced with defaults
- The window is visible on at least one connected monitor — if not, it is repositioned to the center of the primary screen

This prevents issues when monitors are disconnected or the saved position is invalid.

## Remember window location
To remember the location of any window, simply add this statement in the constructor of the window.
`ApplicationBase.GetService<IWindowLocationService>().Monitor(this, nameof(MyWindow));`

The state and location of the window will be saved and restored when the window is reopened.

## TabNavigator
The *TabNavigatorView* is a container view that holds tab view items. It is built for the main window to easily display tab views, handle *title*, *CanClose* and manage rules for multiple tabs of the same types.

Add this control on the main window.
`<tabNavigator:TabNavigatorView />`

To create a tab, start by creating a *User Control* and change the base class from *UserControl* to *TabView* (in namespace Tharga.Wpf.TabNavigator) in the xaml-file.

When running the application, instances of this *TabView* can be added dynamically to the *TabNavigatorView*.

Example of opening a tab view in the main view model at startup.
```
public MainWindowViewModel(ITabNavigationStateService tabNavigationStateService)
{
    tabNavigationStateService.OpenTab<MyTabView>();
}
```

You can also bind a command to open tabs. (You have to inject an instance of ITabNavigationStateService to the command.)
```
public ICommand NewTabCommand => new OpenTabComamnd<MyTabView>(_tabNavigationStateService);
```

The *TabView* have some properties and methods that can be overridden...
```
public partial class MyTabView
{
    public override string DefaultTitle => "Some title";
    public override bool AllowMultiple => true;
    public override bool AllowClose => true;
```
The properties *CanClose* and *Title* can be changed at any time.

The option *AllowTabsWithSameTitles* can be set in *App.xaml.cs*. It will block tabs with the same titles from beeing created.
For this to work you will have to provice the name of the title when creating the tab. Ex `tabNavigationStateService.OpenTab<MyTabView>("My title here");`.
Therefore it currently does not work with the `OpenTabComamnd<T>`, you will have to use the *RelayCommand* of you want to use this feature.

Example of relay command with title.
```
public ICommand NewTabCommand => new RelayCommand(_ => _tabNavigationStateService.OpenTab<MyTabView>("My Tab"), _ => true);
```

## Splash Screen
A splash screen is shown during application startup and update checks. By default it uses a built-in teal image.

### Choosing a built-in image
Set `SplashCreator` to return a `Splash` with a different image from `SplashImageLibrary`:

```csharp
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.SplashCreator = data => new Splash(data with
    {
        ImagePath = SplashImageLibrary.Blue
    });
}
```

Available built-in images: `Blue`, `Green`, `GreenTransparent`, `Orange`, `Red`, `RedTransparent`, `Teal`, `TealTransparent`, `White`, `Yellow`.

### Using a custom image
Point `ImagePath` to any image file. The image should be a **PNG** at **500 x 309 pixels**. Use a transparent PNG if you want the splash window background to show through.

To use an image bundled in your application, add the image file to your project (e.g. `Images/splash.png`) with **Build Action** set to **Resource**, then reference it with a pack URI:

```csharp
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.SplashCreator = data => new Splash(data with
    {
        ImagePath = "pack://application:,,,/MyApp;component/Images/splash.png"
    });
}
```

### Replacing the splash window entirely
To use a completely custom splash window, implement `ISplash` and return it from `SplashCreator`:

```csharp
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.SplashCreator = data => new MyCustomSplash(data);
}
```

### Update behavior
When an application update is detected, the splash screen stays open with a progress bar and the close button hidden, so the user knows an update is in progress. If the update fails, the close button reappears with an error message.

## ClickOnce application update
This features uses [Squirrel](https://www.nuget.org/packages/Clowd.Squirrel) or [Velopack](https://www.nuget.org/packages/Velopack/0.0.1350-g3ba32af) for updates.

To get started register the options *UpdateSystem*, *UpdateIntervalCheck* and *UpdateLocation*.

#### UpdateSystem
Select between None, Squirrel and Velopack.

#### UpdateIntervalCheck
The interval between checks. If the interval is set to 1 hour, the first check will be done after one hour.
If the checks should be performed at startup, use the *checkForUpdates* in *ShowSplashAsync* or call *CheckForUpdateAsync* on application startup.

#### UpdateLocation
This address should point to the location of the Squirrel/Velopack release (the location of the *RELEASES*-file).

Example of registration.
```
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.UpdateSystem = UpdateSystem.Velopack;
    thargaWpfOptions.UpdateIntervalCheck = TimeSpan.Zero;
    thargaWpfOptions.UpdateLocation += _ => new Uri("https://server-address.com/api/ClientUpdate");
}
```

Checks for updates can also be made manually by calling *CheckForUpdateAsync* in *IApplicationUpdateStateService*.

## License Server Check
Make api calls to a license server to check if the application has a valid license.

#### LicenseServerLocation
The endpoints that will be called are *license/check* and *license/key*. You can implement them yourself, or add *Tharga.License* in your API and inject into a controller.

Example of registration.
```
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.LicenseServerLocation += _ => new Uri("https://server-address.com/api");
}
```

## Custom Exception handling
Custom exception handlers should implement `IExceptionHandler<T>` where T is an Exception.
It is registered in the *Options* method of *App.xaml.cs*.

Example of a handler for the exception type *InvalidOperationException*.
```
public class InvalidOperationExceptionHandler : IExceptionHandler<InvalidOperationException>
{
    public void Handle(Window mainWindow, InvalidOperationException exception)
    {
        MessageBox.Show(mainWindow, $"This is a custom error handler for '{exception.Message}'.", exception.GetType().Name);
    }
}
```

Example of registration.
```
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();
}
```