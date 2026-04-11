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
        //NOTE: Default (X button) and Soft close hide the application to the tray.
        if (ApplicationBase.CloseMode is CloseMode.Default or CloseMode.Soft)
        {
            Hide();
            _windowLocationService.SetVisibility(nameof(MainWindow), Visibility);
            e.Cancel = true;
            return;
        }

        //NOTE: Force close — close down tabs and exit the application.
        var tabNavigationStateService = ApplicationBase.GetService<ITabNavigationStateService>();
        if (!await tabNavigationStateService.CloseAllTabsAsync(true))
        {
            e.Cancel = true;
            return;
        }

        //NOTE: Save visibility as Visible so the window is shown on next startup.
        _windowLocationService.SetVisibility(nameof(MainWindow), System.Windows.Visibility.Visible);

        base.OnClosing(e);
    }
}