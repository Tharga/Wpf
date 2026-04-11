using System.ComponentModel;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    public MainWindow()
    {
        IWindowLocationService windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        MinitorInfo monitor = windowLocationService.Monitor(this, nameof(MainWindow));

        InitializeComponent();

        ApplicationBase.GetService<INotifyIconService>().Create(this, nameof(MainWindow), windowLocationService);
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