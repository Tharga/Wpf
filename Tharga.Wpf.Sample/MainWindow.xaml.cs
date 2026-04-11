using System.ComponentModel;
using System.Diagnostics;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    private readonly IWindowLocationService _windowLocationService;

    public MainWindow()
    {
        _windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        var m = _windowLocationService.Monitor(this, nameof(MainWindow));

        m.LocationUpdatedEvent += (s, e) =>
        {
            Debug.WriteLine("LocationUpdatedEvent");
        };

        var myService = ApplicationBase.GetService<MyService>();

        InitializeComponent();

        ApplicationBase.GetService<INotifyIconService>().Create(this, nameof(MainWindow), _windowLocationService);
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        //NOTE: Close down tabs before closing the application.
        var tabNavigationStateService = ApplicationBase.GetService<ITabNavigationStateService>();
        var force = ApplicationBase.CloseMode == CloseMode.Force;
        if (!await tabNavigationStateService.CloseAllTabsAsync(force))
        {
            e.Cancel = true;
            return;
        }

        base.OnClosing(e);
    }
}