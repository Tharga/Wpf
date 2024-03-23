# Tharga Wpf
This package is a basic toolset for WPF applications.

## Features
- [MVVM binding helper](#mvvm-binding-helper)
- [Built in .NET IOC](#ioc)
- [Remember window location](#remember-window-location)
- [TabNavigator](#tabnavigator)
- [*ClickOnce* application update](#clickonce-application-update)
- [Custom Exception handling](#custom-exception-handling)

The sample uses *Fluent.Ribbon* but any package can be used since MainWindow does not need any base class.

## Get started
Set App.xaml to inherit from `Tharga.Wpf.ApplicationBase`.

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

## MVVM binding helper
To bind a *View* with a *ViewModel* add this section in the View.xaml.

`DataContext="{wpf:ViewModelProvider local:MyViewModel}"`

## <a name="ioc">Built in .NET IOC</a>
Tharga.WPF uses the built in .NET IOC with IServiceCollection and IServiceProvider.

### View Model dependency injection
Injection can be done in the ViewModel constructor for all registered service. To have the ViewModel it self registered, do it manually in the *Register* in *App.xaml.cs* or by adding the interface *Tharga.Wpf.IViewModel*.

### View dependency injection
There is no support for injecting services into the View directly. To use resources this method can be used.

`var myService = ApplicationBase.GetService<MyService>();`

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

## ClickOnce application update
This features uses [Squirrel](https://www.nuget.org/packages/Clowd.Squirrel) for updates.

To get started reguster the options *CheckForUpdateInterval* and *ApplicationDownloadLocationLoader*.

#### CheckForUpdateInterval
When set to null, this feature is not enabled (default). If set to Zero, updates are only performed at startup. If there is an interval it wil be regularly checked.

#### ApplicationDownloadLocationLoader
This address should point to the location of the Squirrel release (the location of the *RELEASES*-file).

Example of registration.
```
protected override void Options(ThargaWpfOptions thargaWpfOptions)
{
    thargaWpfOptions.CheckForUpdateInterval = TimeSpan.Zero;
    thargaWpfOptions.ApplicationDownloadLocationLoader += _ => new Uri("https://server-address.com/api/ClientUpdate");
}
```

Checks for updates can also be made manually by calling *CheckForUpdateAsync* in *IApplicationUpdateStateService*.

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