using System.ComponentModel;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    private readonly IWindowLocationService _windowLocationService;

    public MainWindow()
    {
        _windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        _windowLocationService.Monitor(this, nameof(MainWindow));

        var myService = ApplicationBase.GetService<MyService>();

        InitializeComponent();

        //TODO: Create a tray Icon.
        ApplicationBase.GetService<INotifyIconService>().Create(this, nameof(MainWindow), _windowLocationService);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        //NOTE: Interceot the 'default' close (pressing of 'x') and hide the application.
        if (ApplicationBase.CloseMode == CloseMode.Default)
        {
            Hide();
            _windowLocationService.SetVisibility(nameof(MainWindow), Visibility);
            e.Cancel = true;
            return;
        }

        base.OnClosing(e);
    }
}