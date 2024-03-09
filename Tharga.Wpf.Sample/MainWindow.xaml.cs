using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    public MainWindow()
    {
        var myService = ApplicationBase.GetService<MyService>();
        ApplicationBase.GetService<IWindowLocationService>().Monitor(this, nameof(MainWindow));

        InitializeComponent();
    }
}