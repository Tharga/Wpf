using System.ComponentModel;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    public MainWindow()
    {
        this.RegisterMainWindow(showTrayIcon: true);

        InitializeComponent();
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